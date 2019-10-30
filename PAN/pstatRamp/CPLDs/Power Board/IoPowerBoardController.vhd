-------------------------------------------------------------------------------
-- FILE: IoPowerBoardController.vhd
-- 
-- Copyright 2013, The Technology Partnership Ltd
-- 
-- DATE: 22/02/13 
--
-- AUTHOR: Phil Duffy
--
-- DESCRIPTION:
-- Single-file definition for the CPLD fitted to the IO power board.
-- Connects to the thermal board via a 9Mbit/s SPI interface.

-- Handles the following devices:
-- 		5 stepper motors (speed, move length, torque)
-- 		18 pneumatic valve (on/off)
-- 		2 pumps (pwm control)
--		1 door-latch solenoid  (pwm control)
--		1 electromagnet (pwm control)
--
-- Returns the following outputs to the CPU:
-- 		Busy and direction signals from the five motor controllers
--		All PWM values
--		All valves states
--
-- The CPU sends a single 54-byte DMA transfer. The CPLD on the thermal board
-- routes this to several target, including the power board. This CPLD receives
-- a 16-byte transfer with the following assignments:
							
-- Word 0 ls	- Motor command (see below)
-- Word 0 ms	- Unused
-- Word 1		- Step Interval (100us units)
-- Word 2		- Move Length
-- Word 3 ls	- 8-bit PWM  (0-100%) for motor current (common to all motors)
-- Word 3 ms	- 8-bit PWM  (0-100%) for Vacuum pump voltage (speed control)
-- Word 4 ls	- 8-bit PWM  (0-100%) for Pressure pump voltage (speed control)
-- Word 4 ms	- 8-bit PWM  (0-100%) for Eject solenoid
-- Word 5 ls	- 8-bit PWM  (0-100%) for Electromagnet
-- Word 5 ms	- Spare (unused)
-- Word 6 ls	- Valves 1 - 8
-- Word 6 ms	- Valves 9 - 16
-- Word 7 ls	- Valves 17 - 20 (bits 0 - 3), Led (bit 7), unused (bits 6 - 4)
-- Word 7 ms	- Spare (unused)

-- Note the CPU uses a 16-bit transfer, which sends the ms byte of the pair first. We
-- deal with that here, both for transmit and receive.

-- The motor command byte is structured as follows:
-- Bits 2-0		- Motor number (1-5)
-- Bits 7-4		- Command:
--							0 = no action
--							1 = start move with dir = 1
--							2 = start move with dir = 0
--			  Any other value = stop motor
--
-- After the transfer, the receive table will contain:
-- Word 0 ls	- Bits 4-0 are the motor enable (busy) bits for the 5 motors
-- Word 0 ms	- Bits 4-0 are the motor direction bits for the 5 motors
-- Words 1-7	- These are copies of the transmitted words, except word 7 ms byte is CPLD code version
--
-- USED ON:
--        Atlas IO medical diagnostic reader
--
-- MODIFICATIONS:
-- 12/03/13 Added watchdog to force motors and valves off if the SPI ceases for more
-- than 25ms. CPU sends SPI commands every 5ms (+/- 1ms)
-- 24/04/13 Added CPLD code version number as word 7 msb
--
-------------------------------------------------------------------------------

library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.std_logic_unsigned.all;
  
entity IoPowerBoardController is
    port (
        SpiClk          : in std_logic;                             -- 9MHz SPI clock
        SpiMosi	      	: in std_logic;                             -- SPI data from CPU
        SpiMiso	      	: out std_logic;                            -- SPI data to CPU
		nSpiCs			: in std_logic;								-- SPI chip-select
        
		Valves		    : out std_logic_vector(20 downto 1);		-- 20 valve outputs (1 = on)
        PressurePump    : out std_logic;         					-- Pressure pump (PWM)
        VacuumPump      : out std_logic;         					-- Vacuum pump (PWM)
        EjectSolenoid   : out std_logic;         					-- Eject Solenoid (PWM)
        Electromagnet   : out std_logic;         					-- Electromagnet (PWM)
		StepperCurrent	: out std_logic;							-- Reference current for steppers (PWM)
		nStepperReset	: out std_logic;							-- Reset signal to all stepper drivers (active low)

		ClampStep		: out std_logic;							-- Clamp motor step pulse
		ClampDir		: out std_logic;							-- Clamp motor direction
		nClampEnable	: out std_logic;							-- Clamp motor enable (active low)

		Blister1Step	: out std_logic;							-- Blister 1 motor step pulse
		Blister1Dir		: out std_logic;							-- Blister 1 motor direction
		nBlister1Enable	: out std_logic;							-- Blister 1 motor enable (active low)

		Blister2Step	: out std_logic;							-- Blister 2 motor step pulse
		Blister2Dir		: out std_logic;							-- Blister 2 motor direction
		nBlister2Enable	: out std_logic;							-- Blister 2 motor enable (active low)

		Blister3Step	: out std_logic;							-- Blister 3 motor step pulse
		Blister3Dir		: out std_logic;							-- Blister 3 motor direction
		nBlister3Enable	: out std_logic;							-- Blister 3 motor enable (active low)

		MechValveStep	: out std_logic;							-- Mech Valve motor step pulse
		MechValveDir	: out std_logic;							-- Mech Valve motor direction
		nMechValveEnable : out std_logic;							-- Mech Valve motor enable (active low)

		GrnLed			: out std_logic;							-- Green "I'm alive" LED
		LaTest			: out std_logic_vector(7 downto 0);			-- LA test header
		nFault			: out std_logic;							-- Fault output to Thermal board (not used)

		Clk             : in std_logic;                             -- 16MHz System Clock
        nReset          : in std_logic                              -- System reset

    );

end IoPowerBoardController;
    
architecture IoPowerBoardController of IoPowerBoardController is

-- ......................................................................................
-- Constants

	-- CPLD code version - update when code changed after first release
	-- Changed to version 2 for M2 release - no functional changes
	constant VERSION	: std_logic_vector(7 downto 0) := X"02";

-- ......................................................................................
-- Signal declarations

type five7bit is array(4 downto 0) of std_logic_vector(6 downto 0);    
    signal pwmvalue,spwmvalue : five7bit;         				-- 5 7-bit PWM values
	signal pwmsig			: std_logic_vector(4 downto 0); 	-- 5 PWM output signals
	
	signal led				: std_logic;						-- Green LED state
	signal miso				: std_logic;						-- Internal copy of SpiMiso
	signal reset			: std_logic;					 	-- Intenral reset signal
	
	signal valvestates	    : std_logic_vector(20 downto 1);	-- 20 valve states (1 = on)

	signal command			: std_logic_vector( 7 downto 0);	-- Motor command
	signal commandinterval	: std_logic_vector(15 downto 0);	-- step interval in command
	signal commandlength	: std_logic_vector(15 downto 0);	-- move length in command

	signal mdir				: std_logic_vector(4 downto 0);
	signal menable			: std_logic_vector(4 downto 0);
	signal mstart			: std_logic_vector(4 downto 0);
	signal mstop			: std_logic_vector(4 downto 0);
	signal mstep			: std_logic_vector(4 downto 0);
	
	signal subdiv			: std_logic_vector(11 downto 0);	-- Divide by 1600 counter (12-bit)
	signal tick				: std_logic;						-- Pulses every 100us (10kHz)
	signal ptick			: std_logic;						-- Pulses every 250ns (4MHz)
	signal newspi			: std_logic;						-- Set for one clock at trailing edge of SPI nCS
	signal spiok			: std_logic;						-- Cleared if SPI from CPU ceases for 16ms
	signal wdcount			: std_logic_vector(7 downto 0);		-- Watchdog counter - 25ms period

	signal bitcount			: std_logic_vector(2 downto 0);		-- 3-bit bit counter
	signal bytecount		: std_logic_vector(3 downto 0);		-- 4-bit byte counter
	
	signal spirxsr			: std_logic_vector(6 downto 0);		-- SPI rx shift register (7-bit)
	signal spirxval			: std_logic_vector(7 downto 0);		-- SPI rx value (8-bit)
	signal spirxpwm			: std_logic_vector(6 downto 0);		-- SPI rx pwm value (7-bit)
	signal currentbyte		: std_logic_vector(7 downto 0);		-- Current byte value to output

	signal glitchsr			: std_logic_vector(4 downto 0);		-- 5-bit glitch shift-register

begin
-- ......................................................................................
-- Combinational assignments

	SpiMiso 			<= miso;
	reset 				<= not nReset;
	nStepperReset 		<= not reset;
	GrnLed				<= led or not spiok;	-- LED on solid if SPI stops
	nFault 				<= '1';

	StepperCurrent 		<= pwmsig(0);
	VacuumPump 			<= pwmsig(1);
	PressurePump 		<= pwmsig(2);
	EjectSolenoid 		<= pwmsig(3);
	Electromagnet 		<= pwmsig(4);
	
	nClampEnable 		<= not menable(0);
	nBlister1Enable 	<= not menable(1);
	nBlister2Enable 	<= not menable(2);
	nBlister3Enable 	<= not menable(3);
	nMechValveEnable 	<= not menable(4);
	
	ClampDir			<= mdir(0);
	Blister1Dir			<= mdir(1);
	Blister2Dir			<= mdir(2);
	Blister3Dir			<= mdir(3);
	MechValveDir		<= mdir(4);
	
	ClampStep 			<= mstep(0);
	Blister1Step		<= mstep(1);
	Blister2Step		<= mstep(2);
	Blister3Step		<= mstep(3);
	MechValveStep		<= mstep(4);
	
	Valves 				<= valvestates when (spiok = '1') else X"00000";
	
-- Test header assignments
	LaTest(0) <= mstep(0);
	LaTest(1) <= menable(0);
	LaTest(2) <= pwmsig(0);
	LaTest(3) <= SpiClk;
	LaTest(4) <= SpiMosi;
	LaTest(5) <= miso;
	LaTest(6) <= nSpiCs;
	LaTest(7) <= Clk;

-- ......................................................................................
-- Process transfers control signals from the SPI clock domain to the main clock domain
-- Note command, commandlength, commandinterval don't need to be synchronised as they
-- are used only after the end of the SPI packet, on a synchronised sugnal
	process(Clk, reset)
	begin
        if (reset = '1') then
			spwmvalue(0)	<= "0000000";
			spwmvalue(1)	<= "0000000";
			spwmvalue(2)	<= "0000000";
			spwmvalue(3)	<= "0000000";
			spwmvalue(4)	<= "0000000";
        elsif (Clk'event and Clk = '1') then
			spwmvalue(0)	<= pwmvalue(0);
			spwmvalue(1)	<= pwmvalue(1);
			spwmvalue(2)	<= pwmvalue(2);
			spwmvalue(3)	<= pwmvalue(3);
			spwmvalue(4)	<= pwmvalue(4);
		
		end if;
	end process;


-- ......................................................................................
-- Process multiplexes the correct bit onto the SPI Miso signal to the CPU

	process(bytecount, bitcount, currentbyte, menable, mdir, pwmvalue, valvestates)
	begin
		case bytecount is
			when "0001" => currentbyte <= "000" & menable;			-- Which motors are running
			when "0000" => currentbyte <= "000" & mdir;				-- Motor directions		

			when "0111" => currentbyte <= '0' & pwmvalue(0);		-- Motor current pwm
			when "0110" => currentbyte <= '0' & pwmvalue(1);		-- Vacuum pump speed
			when "1001" => currentbyte <= '0' & pwmvalue(2);		-- Pressure pump speed
			when "1000" => currentbyte <= '0' & pwmvalue(3);		-- Eject solenoid PWM
			when "1011" => currentbyte <= '0' & pwmvalue(4);		-- Electromagnet PWM

			when "1101" => currentbyte <= valvestates(8 downto 1);	-- Valve states
			when "1100" => currentbyte <= valvestates(16 downto 9);
			when "1111" => currentbyte <= led & "000" & valvestates(20 downto 17);
			when "1110" => currentbyte <= VERSION;					-- CPLD code version
			when others => currentbyte <= X"00";
		end case;
			
		case bitcount is
			when "000"  => miso <= currentbyte(7);
			when "001"  => miso <= currentbyte(6);
			when "010"  => miso <= currentbyte(5);
			when "011"  => miso <= currentbyte(4);
			when "100"  => miso <= currentbyte(3);
			when "101"  => miso <= currentbyte(2);
			when "110"  => miso <= currentbyte(1);
			when others => miso <= currentbyte(0);
		end case;

	end process;
	
-- ......................................................................................
-- SPI interface. The CPU is the master. The transfer is initiated by nCS going low.
-- Clock is active low and capture is on falling edge (i.e at CPU, CPOL=1, CPHA=0) for
-- compatibility with the ADS8028. Data is transferred msb first for the same reason.
-- Note the byte-swapping - ms byte of each word comes in first

	-- Because we accumulate the rx value on the SPI clock edges, we need to latch
	-- the 8-bit value on the last edge, and include the current Mosi state
	spirxval <= spirxsr & SpiMosi;		-- Form composite (8-bit) spi rx value
	spirxpwm <= spirxval(6 downto 0);	-- 7-bit pwm value
	
	process(SpiClk, reset, nSpiCs)
    begin
		if (reset = '1') then
			valvestates <= X"00000";
			command     <= X"00";
			pwmvalue(0)	<= "0000000";
			pwmvalue(1)	<= "0000000";
			pwmvalue(2)	<= "0000000";
			pwmvalue(3)	<= "0000000";
			pwmvalue(4)	<= "0000000";
			led 		<= '0';
			commandinterval <= X"0000";
			commandlength   <= X"0000";
		elsif (nSpiCs = '1') then
			bitcount    <= "000";
			bytecount   <= "0000";
			spirxsr     <= "0000000";
        elsif (SpiClk'event and SpiClk = '0') then
			bitcount <= bitcount + "001";
			spirxsr <= spirxsr(5 downto 0) & SpiMosi; 
			if (bitcount = "111") then
				bytecount <= bytecount + "0001";
				case bytecount is
					when "0001" => command <= spirxval;
					
					when "0011" => commandinterval(7 downto 0 ) <= spirxval; 
					when "0010" => commandinterval(15 downto 8) <= spirxval; 
					when "0101" => commandlength(7 downto 0 ) <= spirxval;
					when "0100" => commandlength(15 downto 8) <= spirxval; 
					
					when "0111" => pwmvalue(0) <= spirxpwm;
					when "0110" => pwmvalue(1) <= spirxpwm;
					when "1001" => pwmvalue(2) <= spirxpwm;
					when "1000" => pwmvalue(3) <= spirxpwm;
					when "1011" => pwmvalue(4) <= spirxpwm;
					
					when "1101" => valvestates(8 downto 1)   <= spirxval;
					when "1100" => valvestates(16 downto 9)  <= spirxval;
					when "1111" => valvestates(20 downto 17) <= spirxval(3 downto 0);
														 led <= spirxval(7);
					when others => null;
				end case;
			end if;
        end if;
    end process;
	
-- ......................................................................................
-- Process detects the end of the SPI transfer and decodes the command
	process(Clk, reset)
	begin
        if (reset = '1') then
			mstart   <= "00000";
			mstop    <= "00000";
			glitchsr <= "00000";
        elsif (Clk'event and Clk = '1') then
			mstart <= "00000";		-- Default values
			mstop  <= "00000";
			newspi <= '0';
		
			glitchsr <= glitchsr(3 downto 0) & nSpiCs;		-- Shift 5-bit shift reg left
			-- Detect two zeros followed by two ones
			if (glitchsr(4 downto 3) = "00" and glitchsr(2 downto 1) = "11") then
				newspi <= '1';

				if (command(7 downto 4) = "0001" or command(7 downto 4) = "0010") then	-- Start command
					case command(2 downto 0) is				-- Check motor number
						when "001" =>	mstart(0) <= '1';
						when "010" =>	mstart(1) <= '1';
						when "011" =>	mstart(2) <= '1';
						when "100" =>	mstart(3) <= '1';
						when "101" =>	mstart(4) <= '1';
						when others => null;
					end case;
				
				elsif (command(7 downto 4) /= "0000") then	-- Stop command
					case command(2 downto 0) is				-- Check motor number
						when "001" =>	mstop(0) <= '1';
						when "010" =>	mstop(1) <= '1';
						when "011" =>	mstop(2) <= '1';
						when "100" =>	mstop(3) <= '1';
						when "101" =>	mstop(4) <= '1';
						when others => null;
					end case;
				end if;
			end if;
		end if;
	end process;
	
-- ......................................................................................
-- Process creates the subdivider counter (/1600). Pulses tick every 100us (10kHz) and
-- ptick every 250ns (4MHz)

	process(Clk, reset)
	begin
        if (reset = '1') then
			subdiv <= X"000";
			tick   <= '0';
			ptick  <= '0';
        elsif (Clk'event and Clk = '1') then
			if (subdiv(3 downto 0) = "0000") then
				ptick <= '1';
			else
				ptick <= '0';
			end if;
			if (subdiv = X"63F") then
				subdiv <= X"000";
				tick <= '1';
			else
				subdiv <= subdiv + X"001";
				tick <= '0';
			end if;
		end if;
	end process;
-- ......................................................................................
-- Process creates a watchdog counter. Clears flag spiok if the SPI from the CPU stops-- for more than 25ms
	process(Clk, reset)
    begin
        if (reset = '1') then
			wdcount <= X"00";
			spiok <= '1';
        elsif (Clk'event and Clk = '1') then
			if (newspi = '1') then
				wdcount <= X"00";					-- Restart watchdog
			elsif (tick = '1') then					-- Every 100us
				if (wdcount = X"FF") then			-- Watchdog expired
					spiok <= '0';
				else
					wdcount <= wdcount + X"01";
					spiok <= '1';
				end if;
			end if;
        end if;
    end process;
		
-- ......................................................................................
-- Creates five stepper drivers
	
stepper_drivers : for index in 0 to 4 generate

	signal stepcount   	: std_logic_vector(15 downto 0);     -- 16-bit step counter
	signal stepinterval : std_logic_vector(15 downto 0);     -- 16-bit step interval
	signal icount     	: std_logic_vector(15 downto 0);     -- 16-bit interval counter
	
begin
	
	process(Clk, reset, spiok)
	begin
        if (reset = '1' or spiok = '0') then
			menable(index) <= '0';
			mdir(index) <= '0';
			mstep(index) <= '0';
			stepcount <= X"0000";
			stepinterval <= X"0000";
			icount <= X"0000";

        elsif (Clk'event and Clk = '1') then
			if (mstart(index) = '1') then
				menable(index) <= '1';					-- Power the motor up
				mdir(index)  <= command(4);				-- Set the direction
				stepinterval <= commandinterval;		-- Record interval
				stepcount    <= commandlength;			-- Start stop counter
				icount 		 <= commandinterval;		-- First step after interval
			elsif (mstop(index) = '1') then
				menable(index) <= '0';					-- Power the motor down
				stepcount <= X"0000";
				icount <= X"0000";
			elsif (menable(index) = '1') then
				if (tick = '1') then
					if (icount = X"0000") then				-- When interval counter expires ..
						icount <= stepinterval;				-- ... reload it
						if (stepcount = X"0000") then		-- If move is complete
							menable(index) <= '0';			-- Power the motor down
						else
							mstep(index) <= '1';			-- Do a step ...
							stepcount <= stepcount - X"0001"; -- ... and count it out
						end if;
					else
						icount <= icount - X"0001";
						mstep(index) <= '0';
					end if;
				end if;
			end if;
		end if;
	end process;
	
	end generate stepper_drivers;
		
-- ......................................................................................
-- Creates five 7-bit PWM drivers. PWM value is 0 - 100. 0 gives a completely off signal, 100
-- gives a completely on signal (as does any value 100 - 127)
-- PWM0 (motor reference) runs at 160kHz. PWM1-4 run at 10kHz, because the FET switches for
-- these outputs won't go much faster

pwm_drivers : for index in 0 to 4 generate

	signal pwmcount     : std_logic_vector(6 downto 0);     -- 7-bit PWM counter
	
begin
    process(Clk, reset, spiok)
    begin
        if (reset = '1' or spiok = '0') then
			pwmcount <= "0000000";
			pwmsig(index) <= '0';
        elsif (Clk'event and Clk = '1') then
			if (ptick = '1' or index = 0) then				-- PWM0 at high speed - others at low speed
				pwmcount <= pwmcount + "0000001";
				if (pwmcount = "1100011" and pwmvalue(index) /= X"00") then
					pwmcount <= "0000000";							-- 99 -> 0
					pwmsig(index) <= '1';							-- Set PWM state = ON
				elsif (pwmcount = pwmvalue(index)) then
					pwmsig(index) <= '0';							-- Set PWM state = OFF
				end if;
			end if;
        end if;
    end process;

end generate pwm_drivers;

-- ......................................................................................

end IoPowerBoardController;


