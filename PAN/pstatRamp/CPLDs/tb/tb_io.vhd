-- Top-level test bench file for IO thermal and power-board cplds
--
-- EPD 28/02/13
--
library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.std_logic_unsigned.all;
  
entity tb_toplevel is
end tb_toplevel;
    
architecture tb_toplevel of tb_toplevel is

	constant WZ		: std_logic_vector(15 downto 0) := X"8003";	
	constant B20	: std_logic_vector(7 downto 0)  := X"16";	
	constant B40	: std_logic_vector(7 downto 0)  := X"28";	
	constant B60	: std_logic_vector(7 downto 0)  := X"3C";	
	constant BC		: std_logic_vector(7 downto 0)  := X"3C";	
	constant WC		: std_logic_vector(15 downto 0) := X"0000";	

-- ......................................................................................
-- Signal declarations

signal SpiClk			: std_logic;			-- 9MHz SPI clock
signal SpiMosi			: std_logic;			-- SPI data from CPU
signal nSpiCs			: std_logic;			-- SPI chip-select
signal Peltier1Fault	: std_logic;			-- Peltier 1 Over-current
signal Peltier2Fault	: std_logic;			-- Peltier 2 Over-current
signal Peltier3Fault	: std_logic;			-- Peltier 3 Over-current
signal SensorMiso		: std_logic;
signal PeltierMiso		: std_logic;
signal PowerMiso		: std_logic;
signal nPowerFault		: std_logic;
signal Clk				: std_logic;			-- 16MHz System Clock
signal nReset			: std_logic;			-- System reset

-- Thermal board outputs
signal  SpiMiso	      	: std_logic;                            -- SPI data to CPU

signal	Peltier1PwmH    : std_logic;         					-- Peltier 1 PWM high-side
signal	Peltier1PwmL    : std_logic;         					-- Peltier 1 PWM low-side
signal  Peltier1Heat    : std_logic;         					-- Peltier 1 Heating
signal  Peltier1Cool    : std_logic;         					-- Peltier 1 Cooling
signal	Peltier1Fan		: std_logic;							-- Peltier 1 Fan
		
signal  Peltier2PwmH    : std_logic;         					-- Peltier 2 PWM high-side
signal  Peltier2PwmL    : std_logic;         					-- Peltier 2 PWM low-side
signal  Peltier2Heat    : std_logic;         					-- Peltier 2 Heating
signal  Peltier2Cool    : std_logic;         					-- Peltier 2 Cooling
signal	Peltier2Fan		: std_logic;							-- Peltier 2 Fan
		
signal  Peltier3PwmH    : std_logic;         					-- Peltier 3 PWM high-side
signal  Peltier3PwmL    : std_logic;         					-- Peltier 3 PWM low-side
signal  Peltier3Heat    : std_logic;         					-- Peltier 3 Heating
signal  Peltier3Cool    : std_logic;         					-- Peltier 3 Cooling
signal	Peltier3Fan		: std_logic;							-- Peltier 3 Fan

signal	SensorSclk		: std_logic;							-- Temp sensor sense ADC
signal	SensorMosi		: std_logic;
signal	nSensorCs		: std_logic;
signal	nSensorRst		: std_logic;
		
signal	PeltierSclk		: std_logic;							-- Peltier feedback ADC
signal	PeltierMosi		: std_logic;
signal	nPeltierCs		: std_logic;
signal	nPeltierRst		: std_logic;

signal	PowerSclk		: std_logic;							-- Power board
signal	PowerMosi		: std_logic;
signal	nPowerCs		: std_logic;
signal	nPowerRst		: std_logic;

signal  AdcOsMode  		: std_logic_vector(2 downto 0);			-- Potentiostat ADC oversampling mode
signal  PcbFan    		: std_logic;         					-- PCB Fan
signal	ThGrnLed		: std_logic;
signal	ThLaTest		: std_logic_vector(7 downto 0);			-- LA test header

-- Power board outputs
signal	Valves		    : std_logic_vector(20 downto 1);		-- 20 valve outputs (1 = on)
signal  PressurePump    : std_logic;         					-- Pressure pump (PWM)
signal	VacuumPump      : std_logic;         					-- Vacuum pump (PWM)
signal  EjectSolenoid   : std_logic;         					-- Eject Solenoid (PWM)
signal  Electromagnet   : std_logic;         					-- Electromagnet (PWM)
signal	StepperCurrent	: std_logic;							-- Reference current for steppers (PWM)
signal	nStepperReset	: std_logic;							-- Reset signal to all stepper drivers (active low)

signal	ClampStep		: std_logic;							-- Clamp motor step pulse
signal	ClampDir		: std_logic;							-- Clamp motor direction
signal	nClampEnable	: std_logic;							-- Clamp motor enable (active low)

signal	Blister1Step	: std_logic;							-- Blister 1 motor step pulse
signal	Blister1Dir		: std_logic;							-- Blister 1 motor direction
signal	nBlister1Enable	: std_logic;							-- Blister 1 motor enable (active low)

signal	Blister2Step	: std_logic;							-- Blister 2 motor step pulse
signal	Blister2Dir		: std_logic;							-- Blister 2 motor direction
signal	nBlister2Enable	: std_logic;							-- Blister 2 motor enable (active low)

signal	Blister3Step	: std_logic;							-- Blister 3 motor step pulse
signal	Blister3Dir		: std_logic;							-- Blister 3 motor direction
signal	nBlister3Enable	: std_logic;							-- Blister 3 motor enable (active low)

signal	MechValveStep	: std_logic;							-- Mech Valve motor step pulse
signal	MechValveDir	: std_logic;							-- Mech Valve motor direction
signal	nMechValveEnable : std_logic;							-- Mech Valve motor enable (active low)

signal	PwrGrnLed		: std_logic;							-- Green "I'm alive" LED
signal	PwrLaTest		: std_logic_vector(7 downto 0);			-- LA test header

-- Local variables

signal clk2				: std_logic;							-- 9MHz clock
signal txsr				: std_logic_vector(431 downto 0);		-- 27 16-bit words
signal pcount  			: std_logic_vector(11 downto 0);
signal tog     			: std_logic;

begin

tbc : entity work.IoThermalBoardController(IoThermalBoardController) port map(
        SpiClk           => SpiClk,
        SpiMosi          => SpiMosi,
        SpiMiso          => SpiMiso,
		nSpiCs           => nSpiCs,
		Peltier1PwmH     => Peltier1PwmH,
        Peltier1PwmL     => Peltier1PwmL,
        Peltier1Heat     => Peltier1Heat,
        Peltier1Cool     => Peltier1Cool,
        Peltier1Fault  	 => Peltier1Fault,
		Peltier1Fan		 => Peltier1Fan,
        Peltier2PwmH     => Peltier2PwmH,
        Peltier2PwmL     => Peltier2PwmL,
        Peltier2Heat     => Peltier2Heat,
        Peltier2Cool     => Peltier2Cool,
        Peltier2Fault    => Peltier2Fault,
		Peltier2Fan		 => Peltier2Fan,
        Peltier3PwmH     => Peltier3PwmH,
        Peltier3PwmL     => Peltier3PwmL,
        Peltier3Heat     => Peltier3Heat,
        Peltier3Cool     => Peltier3Cool,
        Peltier3Fault    => Peltier3Fault,
		Peltier3Fan		 => Peltier3Fan,
		SensorSclk		 => SensorSclk,
		SensorMosi		 => SensorMosi,
		SensorMiso		 => SensorMiso,
		nSensorCs		 => nSensorCs,
		nSensorRst		 => nSensorRst,
		PeltierSclk		 => PeltierSclk,
		PeltierMosi		 => PeltierMosi,
		PeltierMiso		 => PeltierMiso,
		nPeltierCs		 => nPeltierCs,
		nPeltierRst		 => nPeltierRst,
		PowerSclk		 => PowerSclk,
		PowerMosi		 => PowerMosi,
		PowerMiso		 => PowerMiso,
		nPowerCs		 => nPowerCs,
		nPowerRst		 => nPowerRst,
		nPowerFault		 => nPowerFault,
        AdcOsMode  		 => AdcOsMode,
        PcbFan    		 => PcbFan,
		GrnLed			 => ThGrnLed,
		LaTest			 => ThLaTest,
		Clk              => Clk,
        nReset           => nReset
);

pbc : entity work.IoPowerBoardController(IoPowerBoardController) port map(
        SpiClk			=> PowerSclk,
        SpiMosi			=> PowerMosi,
        SpiMiso			=> PowerMiso,
		nSpiCs			=> nPowerCs,
		Valves			=> Valves,
        PressurePump	=> PressurePump,
        VacuumPump      => VacuumPump,
        EjectSolenoid   => EjectSolenoid,
        Electromagnet   => Electromagnet,
		StepperCurrent	=> StepperCurrent,
		nStepperReset	=> nStepperReset,
		ClampStep		=> ClampStep,
		ClampDir		=> ClampDir,
		nClampEnable	=> nClampEnable,
		Blister1Step	=> Blister1Step,
		Blister1Dir		=> Blister1Dir,
		nBlister1Enable	=> nBlister1Enable,
		Blister2Step	=> Blister2Step,
		Blister2Dir		=> Blister2Dir,
		nBlister2Enable	=> nBlister2Enable,
		Blister3Step	=> Blister3Step,
		Blister3Dir		=> Blister3Dir,
		nBlister3Enable	=> nBlister3Enable,
		MechValveStep	=> MechValveStep,
		MechValveDir	=> MechValveDir,
		nMechValveEnable => nMechValveEnable,
		GrnLed			=> PwrGrnLed,
		LaTest			=> PwrLaTest,
		nFault			=> nPowerFault,
		Clk             => Clk,
        nReset          => nReset
);

-- ......................................................................................
-- Combinational assignments

Peltier1Fault <= '0';
Peltier2Fault <= '0';
Peltier3Fault <= '0';

SpiMosi <= txsr(431);
	
-- ......................................................................................

rst: process
begin
  nReset <= '0';
  wait for 10000 PS;
  nReset <= '1';
  wait;
end process rst; 

clk16: process
begin
  Clk <= '0';
  wait for 31250 PS;
  Clk <= '1';
  wait for 31250 PS;
end process clk16; 

clk18: process
begin
  clk2 <= '0';
  wait for 27777 PS;
  clk2 <= '1';
  wait for 27777 PS;
end process clk18; 

padc_spi: process(nPeltierRst, nPeltierCs, PeltierSclk)	-- Simulate Peltier ADC
begin
	if (nPeltierRst = '0' or nPeltierCs = '1') then
		PeltierMiso <= '1';
	elsif (PeltierSclk'event and PeltierSclk = '0') then
		PeltierMiso <= not PeltierMiso;					-- Just toggle Miso
	end if;
end process;

sadc_spi: process(nSensorRst, nSensorCs, SensorSclk)	-- Simulate Sensor ADC
begin
	if (nSensorRst = '0' or nSensorCs = '1') then
		SensorMiso <= '1';
	elsif (SensorSclk'event and SensorSclk = '0') then 
		SensorMiso <= not SensorMiso;					-- Just toggle Miso
	end if;
end process;
	
main_spi: process(nReset, clk2)							-- Simulate CPU SPI 
begin
	if (nReset = '0') then
		SpiClk  <= '1';
		nSpiCs  <= '1';
		pcount  <= X"000";
		txsr	<= (others => '1');
		tog 	<= '0';
	elsif (clk2'event and clk2 = '1') then 
		tog <= not tog;
		if (tog = '0' and pcount /= X"1B3") then		-- On falling edge of clock
			pcount <= pcount + X"001";
			SpiClk <= '0';
			if (pcount = X"03") then
				nSpiCs <= '0';
				txsr <= WZ&WZ&WZ&WZ&WZ&WZ&WZ&WZ & 		-- Peltier ADC
						WZ&WZ&WZ&WZ&WZ&WZ&WZ&WZ & 		-- Sensor ADC
						WZ&WZ&WZ&WZ&WZ&WZ&WZ&WZ & 		-- Power board (to be changed)
						B40 & B20 & BC & B60 & WC;		-- PWM values, control byte, control word
			else
				txsr <= txsr(430 downto 0) & '1';
				if (pcount = X"1B2") then
					nSpiCs <= '1';
				end if;
			end if;
		else
			SpiClk <= '1';
		end if;
	end if;
end process;

-- ......................................................................................

end tb_toplevel;

