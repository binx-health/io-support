-------------------------------------------------------------------------------
-- FILE: IoThermalBoardController.vhd
-- 
-- Copyright 2013, The Technology Partnership Ltd
-- 
-- DATE: 10/02/13
--
-- AUTHOR: Phil Duffy
--
-- DESCRIPTION:
-- Single-file definition for the CPLD fitted to the IO thermal board.
-- Connects to the main control board via a 9Mbit/s SPI interface.

-- Handles the following devices directly:
-- 		Three Peltier heater/coolers (each an 8-bit PWM value)
-- 		Three heat/cool control bits for the peltiers
-- 		Three fans
-- 		Two 8-channel, 12-bit ADCs (ADS8028) via SPI interfaces. One of these
-- reports current and voltage measurements for the three Peltier drivers. The
-- other reports three top and three bottom temperatures for the three Peltier
-- stacks
-- In addition, passes a subset of the supplied SPI transfer to the power board
--
-- The CPU accesses all values using a 27-word DMA transfer
--
-- The transmit table should contain 27 x 16-bit words assigned as follows:
-- Words 0-7 	8 unused words (time slot used for Peltier ADC results)
-- Words 8-15	8 unused words (time slot used for Sensor ADC results)
-- Words 16-23	8 words targeted at the Power board (see Power board CPLD for assignment)
-- Word 24 ls	1 8-bit (0 - 100%) PWM values for Peltier 1
-- Word 24 ms	1 8-bit (0 - 100%) PWM values for Peltier 2
-- Word 25 ls	1 8-bit (0 - 100%) PWM values for Peltier 2
-- Word 25 ms	1 8-bit control byte:
--			bit 7 controls an "I'm Alive" LED (toggled by the CPU at ~1Hz)
--			bits 6-4 are the Peltier heat(1) or cool(0) controls
--			bits 3-0 are the Fan control bits (1 = on)
-- Word 26		1 control word:
--			bits 15-7 unused (spare)
--			bits 6-4 are the Potentistat ADC Oversampling mode (OS2-0)
--			bits 3-0 unused(spare)
--
-- After the transfer, the receive table will contain 27 words:
-- Words 0-7	8 16-bit values returned from the Peltier ADC
-- Words 8-15	8 16-bit values returned from the temperature ADC
-- Words 16-23	8 16-bit words returned from power board (See power board CPLD for details)
-- Words 24-25	Copies of the input words 24-25
-- Word 26		1 16-bit status word:
--			bits 15-8 CPLD code version number (8-bit)
--			bit 7 unused
--			bits 6-4 are the Potentistat ADC Oversampling mode (OS2-0)
--			bit  3 is the Fault signal from the Power Board
-- 			bits 2-0 are the three motor overcurrent fault bits
--
-- Data is sent as 16-bit with the msb sent first
--
-- The total transfer time is (27 x 16)/9 = 48us or 21kHz
--
-- USED ON:
--        Atlas IO medical diagnostic reader
--
-- MODIFICATIONS:
-- 12/03/13 Added watchdog to turn Peltiers off if the SPI ceases for more than 16ms
-- 24/04/13 Added CPLD version number to word 26 ms byte
--
-------------------------------------------------------------------------------

library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.std_logic_unsigned.all;
  
entity IoThermalBoardController is
    port (
        SpiClk          : in std_logic;                             -- 9MHz SPI clock
        SpiMosi	      	: in std_logic;                             -- SPI data from CPU
        SpiMiso	      	: out std_logic;                            -- SPI data to CPU
		nSpiCs			: in std_logic;								-- SPI chip-select

		Peltier1PwmH    : out std_logic;         					-- Peltier 1 PWM high-side
        Peltier1PwmL    : out std_logic;         					-- Peltier 1 PWM low-side
        Peltier1Heat    : out std_logic;         					-- Peltier 1 Heating
        Peltier1Cool    : out std_logic;         					-- Peltier 1 Cooling
        nPeltier1Oc   	: in std_logic;         					-- Peltier 1 Over-current
		Peltier1Fan		: out std_logic;							-- Peltier 1 Fan
		
        Peltier2PwmH    : out std_logic;         					-- Peltier 2 PWM high-side
        Peltier2PwmL    : out std_logic;         					-- Peltier 2 PWM low-side
        Peltier2Heat    : out std_logic;         					-- Peltier 2 Heating
        Peltier2Cool    : out std_logic;         					-- Peltier 2 Cooling
        nPeltier2Oc   	: in std_logic;         					-- Peltier 2 Over-current
		Peltier2Fan		: out std_logic;							-- Peltier 2 Fan
		
        Peltier3PwmH    : out std_logic;         					-- Peltier 3 PWM high-side
        Peltier3PwmL    : out std_logic;         					-- Peltier 3 PWM low-side
        Peltier3Heat    : out std_logic;         					-- Peltier 3 Heating
        Peltier3Cool    : out std_logic;         					-- Peltier 3 Cooling
        nPeltier3Oc   	: in std_logic;         					-- Peltier 3 Over-current
		Peltier3Fan		: out std_logic;							-- Peltier 3 Fan

		SensorSclk		: out std_logic;							-- Temp sensor sense ADC
		SensorMosi		: out std_logic;
		SensorMiso		: in std_logic;
		nSensorCs		: out std_logic;
		nSensorRst		: out std_logic;
		
		PeltierSclk		: out std_logic;							-- Peltier feedback ADC
		PeltierMosi		: out std_logic;
		PeltierMiso		: in std_logic;
		nPeltierCs		: out std_logic;
		nPeltierRst		: out std_logic;

		PowerSclk		: out std_logic;							-- Power board
		PowerMosi		: out std_logic;
		PowerMiso		: in std_logic;
		nPowerCs		: out std_logic;
		nPowerRst		: out std_logic;
		nPowerFault		: in std_logic;

        AdcOsMode  		: out std_logic_vector(2 downto 0);			-- Potentiostat ADC oversampling mode
        PcbFan    		: out std_logic;         					-- PCB Fan
		GrnLed			: out std_logic;
		LaTest			: out std_logic_vector(7 downto 0);			-- LA test header
		
		Clk             : in std_logic;                             -- 16MHz System Clock
        nReset          : in std_logic                              -- System reset
    );

end IoThermalBoardController;
    
architecture IoThermalBoardController of IoThermalBoardController is

-- ......................................................................................
-- Constants

	-- CPLD code version - update when code changed after first release
	-- Changed to version 2 for M2 release - no functional changes
	constant VERSION	: std_logic_vector(7 downto 0) := X"02";
	-- Maximum Peltier PWM is around 18V, to give enough overhead to turn top FETs on full
	constant PWM_MAX	: std_logic_vector(6 downto 0) := "1001011";	-- 0x4b = 75% max
	-- ADC command. Convert on all channels using internal reference
	constant ADC_CMD	: std_logic_vector(15 downto 0) := "1011111111000000";

-- ......................................................................................
-- Signal declarations

	type ADC_STATE is (ADC_IDLE, ADC_START, ADC_RUN, ADC_GAP);
	signal adcstate : ADC_STATE;

	type eightword is array(7 downto 0) of std_logic_vector(15 downto 0);
	signal sensorval		: eightword;						-- 8 sensor ADC results
	signal peltierval		: eightword;						-- 8 peltier ADC results

    type three7bit is array(2 downto 0) of std_logic_vector(6 downto 0);    
    signal pwmval,spwmval   : three7bit;						-- 3 7-bit PWM values

	signal ncsa, ncsb		: std_logic;						-- Pipelined versions of nSpiCs
	signal adcbitcount		: std_logic_vector(3 downto 0);		-- 4-bit ADC bit counter
	signal channel			: std_logic_vector(2 downto 0);		-- 3-bit ADC channel number

	signal sensor_rxsr		: std_logic_vector(15 downto 0);	-- 16-bit sensor ADC rx shift-register
	signal peltier_rxsr		: std_logic_vector(15 downto 0);	-- 16-bit peltier ADC rx shift-register
	signal sensor_txsr		: std_logic_vector(15 downto 0);	-- 16-bit sensor ADC tx shift-register
	signal peltier_txsr		: std_logic_vector(15 downto 0);	-- 16-bit peltier ADC tx shift-register

	signal tickdiv			: std_logic_vector(3 downto 0);		-- 4-bit tick-divider
	signal tick	 			: std_logic;						-- Tick every 1us
	signal adccs 			: std_logic;						-- ADC chip-select (internal)
	signal spiok			: std_logic;						-- Cleared if SPI from CPU ceases for 16ms
	signal wdcount			: std_logic_vector(15 downto 0);	-- Watchdog counter - 16ms period

	signal bitcount			: std_logic_vector(3 downto 0);		-- 4-bit bit-counter
	signal wordcount		: std_logic_vector(4 downto 0);		-- 5-bit byte-counter
	signal currentword		: std_logic_vector(15 downto 0);	-- Current word value to output

	signal pwmh 			: std_logic_vector(2 downto 0);
	signal pwml				: std_logic_vector(2 downto 0);
	signal hreq,shreq		: std_logic_vector(2 downto 0);
	signal heat 			: std_logic_vector(2 downto 0);
	signal cool 			: std_logic_vector(2 downto 0);
	signal pwmzero 			: std_logic_vector(2 downto 0);		-- Set when PWM value is zero
	signal fan				: std_logic_vector(3 downto 0);		-- 3 Peltier fans + PCB fan
	signal fault 			: std_logic_vector(2 downto 0);
	signal osmode			: std_logic_vector(2 downto 0);		-- 3-bit ADC oversampling mode
	signal led	 			: std_logic;
	
	signal spirxsr			: std_logic_vector(14 downto 0);	-- SPI rx shift register (15-bit)
	signal spirxval			: std_logic_vector(15 downto 0);	-- SPI rx value (16-bit)
	signal spirxpwml		: std_logic_vector(6 downto 0);		-- SPI rx pwm value (7-bit)
	signal spirxpwmh		: std_logic_vector(6 downto 0);		-- SPI rx pwm value (7-bit)

	signal sensorclk		: std_logic;						-- Internal version of SensorSclk
	signal peltierclk		: std_logic;						-- Internal version of PeltierSclk
	signal miso				: std_logic;						-- Internal version of SpiMiso
	signal powercs			: std_logic;						-- Internal version of nPowerCs

begin
-- ......................................................................................
-- Combinational assignments

	pwmzero(0) <= '1' when (spwmval(0) = "0000000") else '0';
	pwmzero(1) <= '1' when (spwmval(1) = "0000000") else '0';
	pwmzero(2) <= '1' when (spwmval(2) = "0000000") else '0';

	Peltier1PwmH <= pwmh(0) and nPeltier1Oc;
	Peltier1PwmL <= pwml(0) and nPeltier1Oc;
	Peltier1Heat <= heat(0) and not pwmzero(0);
	Peltier1Cool <= cool(0) and not pwmzero(0);
	Peltier1Fan  <= fan(0);
	fault(0) 	 <= not nPeltier1Oc;

	Peltier2PwmH <= pwmh(1) and nPeltier2Oc;
	Peltier2PwmL <= pwml(1) and nPeltier2Oc;
	Peltier2Heat <= heat(1) and not pwmzero(1);
	Peltier2Cool <= cool(1) and not pwmzero(1);
	Peltier2Fan  <= fan(1);
	fault(1) 	 <= not nPeltier2Oc;

	Peltier3PwmH <= pwmh(2) and nPeltier3Oc;
	Peltier3PwmL <= pwml(2) and nPeltier3Oc;
	Peltier3Heat <= heat(2) and not pwmzero(2);
	Peltier3Cool <= cool(2) and not pwmzero(2);
	Peltier3Fan  <= fan(2);
	fault(2) 	 <= not nPeltier3Oc;

	PcbFan      <= fan(3);
	GrnLed 	    <= led or not spiok;	-- LED on solid if SPI stops
	AdcOsMode   <= osmode;
	
	SensorMosi  <= sensor_txsr(15);		-- msb of txsr is mosi
	PeltierMosi <= peltier_txsr(15);	-- msb of txsr is mosi
	PowerMosi   <= SpiMosi;				-- Route power mosi straight through
	
	PowerSclk   <= SpiClk;				-- Route power clock straight through
	
	nSensorRst  <= nReset;				-- Distribute reset
	nPeltierRst <= nReset;
	nPowerRst   <= nReset;

	SensorSclk 	<= sensorclk;
	PeltierSclk	<= peltierclk;
	SpiMiso		<= miso;
	nPowerCs 	<= not powercs;

-- Test header assignments
	LaTest(0) <= spiok;
	LaTest(1) <= heat(0);
	LaTest(2) <= cool(0);
	LaTest(3) <= heat(1);
	LaTest(4) <= cool(1);
	LaTest(5) <= heat(2);
	LaTest(6) <= cool(2);
	LaTest(7) <= nReset;
	
-- ......................................................................................
-- Process transfers control signals from the SPI clock domain to the main clock domain

	process(Clk, nReset)
    begin
        if (nReset = '0') then
			spwmval(0) <= "0000000";
			spwmval(1) <= "0000000";
			spwmval(2) <= "0000000";
			shreq	  <= "000";
		
        elsif (Clk'event and Clk = '1') then
			spwmval(0) <= pwmval(0);
			spwmval(1) <= pwmval(1);
			spwmval(2) <= pwmval(2);
			shreq	   <= hreq;
        end if;
    end process;

-- ......................................................................................
-- Process creates the tick counter - divides the clock by 16 to give a 1MHz tick.
-- Also generates the ADC clock signal and pipelines the ADC chip-selects

	process(Clk, nReset)
    begin
        if (nReset = '0') then
			tickdiv    <= "0000";
			sensorclk  <= '1';
			peltierclk <= '1';
			nSensorCs  <=  '1';
			nPeltierCs <= '1';
        elsif (Clk'event and Clk = '1') then
			nSensorCs <=  not adccs;
			nPeltierCs <= not adccs;
			if (adccs = '1' and tickdiv(3) = '1') then
				sensorclk <= '0';
				peltierclk <= '0';
			else
				sensorclk <= '1';
				peltierclk <= '1';
			end if;
			tickdiv <= tickdiv + "0001";
			if (tickdiv = "1111") then
				tick <= '1';
			else
				tick <= '0';
			end if;
        end if;
    end process;
	
-- ......................................................................................
-- Process creates a watchdog counter. Clears flag spiok if the SPI from the CPU stops-- for more than 16ms

	process(Clk, nReset)
    begin
        if (nReset = '0') then
			wdcount <= X"0000";
			spiok <= '1';
        elsif (Clk'event and Clk = '1') then
			if (tick = '1') then						-- Every 1us
				if (ncsb = '0' and ncsa = '1') then		-- Detect trailing edge of nCS
					wdcount <= X"0000";					-- Restart watchdog
				elsif (wdcount = X"FFFF") then			-- Watchdog expired
					spiok <= '0';
				else
					wdcount <= wdcount + X"0001";
					spiok <= '1';
				end if;
			end if;
        end if;
    end process;
	
-- ......................................................................................
-- Process creates two separate synchronous 16-bit SPI interfaces to the Sensor ADC and
-- the Peltier ADC. The state machine is triggered by the end of an SPI transfer from the
-- CPU, and sends one command to each ADC to convert all 8 channels, then recovers 8
-- result values, which are stored for passing back to the CPU on the next transfer.

	process(Clk, nReset)
    begin
        if (nReset = '0') then
			adcstate  <= ADC_IDLE;
			adccs 	  <= '0';
			adcbitcount <= "0000";
			channel   <= "000";
			sensor_rxsr  <= X"0000";
			peltier_rxsr <= X"0000";
			sensor_txsr  <= X"0000";
			peltier_txsr <= X"0000";
			ncsa 	  <= '0';
			ncsb      <= '0';
			peltierval(0) <= X"0000";
			peltierval(1) <= X"0000";
			peltierval(2) <= X"0000";
			peltierval(3) <= X"0000";
			peltierval(4) <= X"0000";
			peltierval(5) <= X"0000";
			peltierval(6) <= X"0000";
			peltierval(7) <= X"0000";
			sensorval(0)  <= X"0000";
			sensorval(1)  <= X"0000";
			sensorval(2)  <= X"0000";
			sensorval(3)  <= X"0000";
			sensorval(4)  <= X"0000";
			sensorval(5)  <= X"0000";
			sensorval(6)  <= X"0000";
			sensorval(7)  <= X"0000";

        elsif (Clk'event and Clk = '1') then
		
			if (tick = '1') then				-- At 1MHz
				case adcstate is
					when ADC_IDLE =>
						ncsa <= nSpiCs;				-- Detect trailing (rising) edge of nSpiCs
						ncsb <= ncsa;
						if (ncsb = '0' and ncsa = '1') then
							channel <= "000";
							sensor_txsr <= ADC_CMD;	-- Send command to convert all channels
							peltier_txsr <= ADC_CMD;
							adcstate <= ADC_START;
						end if;
						
					when ADC_START =>
						adcbitcount <= "0000";
						adccs <= '1';				-- Set ADC chip-selects
						adcstate <= ADC_RUN;
						
					when ADC_RUN =>
						sensor_txsr  <= sensor_txsr(14 downto 0)  & '1';
						peltier_txsr <= peltier_txsr(14 downto 0) & '1';
						if (adcbitcount = "1111") then
							adccs <= '0';			-- Chip-selects off
							adcstate <= ADC_GAP;
						else
							adcbitcount <= adcbitcount + "0001";
						end if;
						
					when ADC_GAP =>
						-- Store result based on channel. Note result is delayed by 2 cycles
						-- so store the data accordingly
						case channel is
							when "000" =>
								sensorval(6)  <= sensor_rxsr;
								peltierval(6) <= peltier_rxsr;
							when "001" =>
								sensorval(7)  <= sensor_rxsr;
								peltierval(7) <= peltier_rxsr;
							when "010" =>
								sensorval(0)  <= sensor_rxsr;
								peltierval(0) <= peltier_rxsr;
							when "011" =>
								sensorval(1)  <= sensor_rxsr;
								peltierval(1) <= peltier_rxsr;
							when "100" =>
								sensorval(2)  <= sensor_rxsr;
								peltierval(2) <= peltier_rxsr;
							when "101" =>
								sensorval(3)  <= sensor_rxsr;
								peltierval(3) <= peltier_rxsr;
							when "110" =>
								sensorval(4)  <= sensor_rxsr;
								peltierval(4) <= peltier_rxsr;
							when others =>
								sensorval(5)  <= sensor_rxsr;
								peltierval(5) <= peltier_rxsr;
						end case;		
					
						if (channel = "111") then
							adcstate <= ADC_IDLE;
						else
							sensor_txsr(15) <= '0';					-- Clear write bit, so command ignored
							peltier_txsr(15) <= '0';
							channel <= channel + "001";
							adcstate <= ADC_START;
						end if;
						
					when others =>
						adcstate <= ADC_IDLE;
				end case;
			elsif (adcstate = ADC_RUN and tickdiv = "0111") then	-- On falling edge of Sclk, latch MISO
				sensor_rxsr  <= sensor_rxsr(14 downto 0)  & SensorMiso;
				peltier_rxsr <= peltier_rxsr(14 downto 0) & PeltierMiso;
			end if;
        end if;
    end process;
	
-- ......................................................................................
-- Process multiplexes the correct bit onto the SPI Miso signal to the CPU

	process(wordcount, bitcount, currentword, nSpiCs, PowerMiso, pwmval, heat, fan, fault)
	begin

		if (wordcount(4 downto 3) = "10") then						-- Words 16-23
			miso <= PowerMiso;
			powercs <= not nSpiCs;
			currentword <= X"0000";
		else
			powercs   <= '0';

			case wordcount is
				when "00000" => currentword <= peltierval(0);		-- Word 0
				when "00001" => currentword <= peltierval(1);
				when "00010" => currentword <= peltierval(2);
				when "00011" => currentword <= peltierval(3);
				when "00100" => currentword <= peltierval(4);
				when "00101" => currentword <= peltierval(5);
				when "00110" => currentword <= peltierval(6);
				when "00111" => currentword <= peltierval(7);
				
				when "01000" => currentword <= sensorval(0);		-- Word 8
				when "01001" => currentword <= sensorval(1);
				when "01010" => currentword <= sensorval(2);
				when "01011" => currentword <= sensorval(3);
				when "01100" => currentword <= sensorval(4);
				when "01101" => currentword <= sensorval(5);
				when "01110" => currentword <= sensorval(6);
				when "01111" => currentword <= sensorval(7);
				
				when "11000" => currentword <= '0' & pwmval(1) & '0' & pwmval(0); 					-- Word 24
				when "11001" => currentword <= led & hreq & fan & '0' & pwmval(2);					-- Word 25
				when "11010" => currentword <= VERSION & '0' & osmode & fault & not nPowerFault;  	-- Word 26
				
				when others =>	currentword <= X"0000";		-- This never happens
		
			end case;

			case bitcount is
				when "0000"  => miso <= currentword(15);
				when "0001"  => miso <= currentword(14);
				when "0010"  => miso <= currentword(13);
				when "0011"  => miso <= currentword(12);
				when "0100"  => miso <= currentword(11);
				when "0101"  => miso <= currentword(10);
				when "0110"  => miso <= currentword(9);
				when "0111"  => miso <= currentword(8);
				when "1000"  => miso <= currentword(7);
				when "1001"  => miso <= currentword(6);
				when "1010"  => miso <= currentword(5);
				when "1011"  => miso <= currentword(4);
				when "1100"  => miso <= currentword(3);
				when "1101"  => miso <= currentword(2);
				when "1110"  => miso <= currentword(1);
				when others  => miso <= currentword(0);
			end case;
		end if;
	end process;
--
-- ......................................................................................
-- SPI interface. The CPU is the master. The transfer is initiated by nCS going low.
-- Clock is active low and capture is on falling edge (i.e at CPU, CPOL=1, CPHA=0) for
-- compatibility with the ADS8028. Data is transferred msb first for the same reason.
-- Note the ADC results are pipelined, so the result table in the CPU software will
-- shifted accordingly

	-- Because we're clocked by the SPI clock, we can't pipeline the Mosi signal, so
	-- we use a 7-bit shift register, and add the last bit in live here
	spirxval <= spirxsr & SpiMosi;		-- Form composite (16-bit) spi rx value
	-- Form 7-bit PWM values, and limit to PWM_MAX for safety (75%)
	spirxpwmh <= spirxval(14 downto 8) when (spirxval(14 downto 8) < PWM_MAX) else PWM_MAX;
	spirxpwml <= spirxval(6 downto 0) when  (spirxval(6 downto 0)  < PWM_MAX) else PWM_MAX;

	process(SpiClk, nReset, nSpiCs)
    begin
        if (nReset = '0') then
			pwmval(0) <= "0000000";
			pwmval(1) <= "0000000";
			pwmval(2) <= "0000000";
			fan       <= "0000";
			hreq	  <= "000";
			led 	  <= '0';
			osmode 	  <= "000";
		elsif (nSpiCs = '1') then
			bitcount  <= "0000";
			wordcount <= "00000";
			spirxsr   <= "000000000000000";
        elsif (SpiClk'event and SpiClk = '0') then
			bitcount <= bitcount + "0001";
			spirxsr <= spirxsr(13 downto 0) & SpiMosi; 
			if (bitcount = "1111") then
				wordcount <= wordcount + "00001";
				-- Store PWM values and fan signal states sent by CPU
				if    (wordcount = "11000") then	-- Word 24
					pwmval(1) <= spirxpwmh;
					pwmval(0) <= spirxpwml;
				elsif (wordcount = "11001") then	-- Word 25
					led  <= spirxval(15);
					hreq <= spirxval(14 downto 12);
					fan  <= spirxval(11 downto 8);
					pwmval(2) <= spirxpwml;
				elsif (wordcount = "11010") then	-- Word 26
					osmode <= spirxval(6 downto 4);
				end if;
			end if;
        end if;
    end process;
	
-- ......................................................................................
-- Create three PWM drivers

pwm_drivers : for index in 0 to 2 generate

	signal pwmdeadtimer : std_logic_vector(3 downto 0);		-- deadtime counter (312ns)
	signal pwmcount     : std_logic_vector(6 downto 0);     -- 7-bit PWM counter
	signal pwmsig 		: std_logic;						-- pwm output signal
	
	begin

    process(Clk, nReset, spiok)
    begin
        if (nReset = '0' or spiok = '0') then
			pwmcount <= "0000000";
			pwmdeadtimer <= X"0";
			pwmsig <= '0';
			pwml(index) <= '0';
			pwmh(index) <= '0';
        elsif (Clk'event and Clk = '1') then
			pwmcount <= pwmcount + "0000001";
			if (pwmcount = "1100011" and spwmval(index) /= "0000000") then
				pwmcount <= "0000000";							-- Restart count
				pwmsig <= '1';									-- Set PWM state = ON
--				pwmdeadtimer <= X"5";							-- Start deadtime counter
				pwmdeadtimer <= X"A";							-- Start deadtime counter (extended for test)
				pwml(index) <= '0';								-- Both outputs off now
				pwmh(index) <= '0';
			elsif (pwmcount = spwmval(index)) then
				pwmsig <= '0';									-- Set PWM state = OFF
--				pwmdeadtimer <= X"5";							-- Start deadtime counter
				pwmdeadtimer <= X"A";							-- Start deadtime counter (extended for test)
				pwml(index) <= '0';								-- Both outputs off now
				pwmh(index) <= '0';
			elsif (pwmdeadtimer /= X"0") then
				pwmdeadtimer <= pwmdeadtimer - X"1";
				if (pwmdeadtimer = X"1") then					-- At the end of the deadtime ...
					pwmh(index) <= pwmsig and not pwmzero(index);	-- ... switch on one output
					pwml(index) <= not pwmsig and not pwmzero(index); -- ... unless PWM value is zero
				end if;
			end if;
        end if;
    end process;

    end generate pwm_drivers;

-- ......................................................................................
-- Create three heat/cool controls with deadtime

heatcool : for index in 0 to 2 generate

	signal hcdeadtimer	: std_logic_vector(15 downto 0);		-- deadtime counter (4ms)
	signal dhreq 		: std_logic;							-- delayed heat/cool request state

	begin

	process(Clk, nReset, spiok)
    begin
        if (nReset = '0' or spiok = '0') then
			hcdeadtimer <= X"FFFF";								-- Run dead timer at reset
			dhreq <= '0';
			heat(index) <= '0';
			cool(index) <= '0';
        elsif (Clk'event and Clk = '1') then
			dhreq <= shreq(index);								-- pipeline heat/cool request bit
			if (dhreq /= shreq(index)) then						-- detect change of state
				hcdeadtimer <= X"FFFF";							-- start deadtime counter
				heat(index) <= '0';								-- Both outputs off now
				cool(index) <= '0';
			elsif (hcdeadtimer /= X"0000") then
				hcdeadtimer <= hcdeadtimer - X"0001";
			else												-- Dead timer not running ...
				heat(index) <= dhreq;							-- ... so drive one or other output
				cool(index) <= not dhreq;
			end if;
        end if;
    end process;

    end generate heatcool;
    
-- ......................................................................................

end IoThermalBoardController;


