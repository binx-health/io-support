onerror {resume}
quietly WaveActivateNextPane {} 0
add wave -noupdate /tb_toplevel/SpiClk
add wave -noupdate /tb_toplevel/SpiMosi
add wave -noupdate /tb_toplevel/SpiMiso
add wave -noupdate /tb_toplevel/nSpiCs
add wave -noupdate /tb_toplevel/Peltier1Fault
add wave -noupdate /tb_toplevel/Peltier2Fault
add wave -noupdate /tb_toplevel/Peltier3Fault
add wave -noupdate /tb_toplevel/nPowerFault
add wave -noupdate /tb_toplevel/Clk
add wave -noupdate /tb_toplevel/nReset
add wave -noupdate /tb_toplevel/Peltier1PwmH
add wave -noupdate /tb_toplevel/Peltier1PwmL
add wave -noupdate /tb_toplevel/Peltier1Heat
add wave -noupdate /tb_toplevel/Peltier1Cool
add wave -noupdate /tb_toplevel/Peltier1Fan
add wave -noupdate /tb_toplevel/Peltier2PwmH
add wave -noupdate /tb_toplevel/Peltier2PwmL
add wave -noupdate /tb_toplevel/Peltier2Heat
add wave -noupdate /tb_toplevel/Peltier2Cool
add wave -noupdate /tb_toplevel/Peltier2Fan
add wave -noupdate /tb_toplevel/Peltier3PwmH
add wave -noupdate /tb_toplevel/Peltier3PwmL
add wave -noupdate /tb_toplevel/Peltier3Heat
add wave -noupdate /tb_toplevel/Peltier3Cool
add wave -noupdate /tb_toplevel/Peltier3Fan
add wave -noupdate /tb_toplevel/SensorSclk
add wave -noupdate /tb_toplevel/SensorMosi
add wave -noupdate /tb_toplevel/SensorMiso
add wave -noupdate /tb_toplevel/nSensorCs
add wave -noupdate /tb_toplevel/nSensorRst
add wave -noupdate /tb_toplevel/PeltierSclk
add wave -noupdate /tb_toplevel/PeltierMosi
add wave -noupdate /tb_toplevel/PeltierMiso
add wave -noupdate /tb_toplevel/nPeltierCs
add wave -noupdate /tb_toplevel/nPeltierRst
add wave -noupdate /tb_toplevel/PowerSclk
add wave -noupdate /tb_toplevel/PowerMosi
add wave -noupdate /tb_toplevel/PowerMiso
add wave -noupdate /tb_toplevel/nPowerCs
add wave -noupdate /tb_toplevel/nPowerRst
add wave -noupdate /tb_toplevel/AdcOsMode
add wave -noupdate /tb_toplevel/PcbFan
add wave -noupdate /tb_toplevel/ThGrnLed
add wave -noupdate /tb_toplevel/ThLaTest
add wave -noupdate /tb_toplevel/Valves
add wave -noupdate /tb_toplevel/PressurePump
add wave -noupdate /tb_toplevel/VacuumPump
add wave -noupdate /tb_toplevel/EjectSolenoid
add wave -noupdate /tb_toplevel/Electromagnet
add wave -noupdate /tb_toplevel/StepperCurrent
add wave -noupdate /tb_toplevel/nStepperReset
add wave -noupdate /tb_toplevel/ClampStep
add wave -noupdate /tb_toplevel/ClampDir
add wave -noupdate /tb_toplevel/nClampEnable
add wave -noupdate /tb_toplevel/Blister1Step
add wave -noupdate /tb_toplevel/Blister1Dir
add wave -noupdate /tb_toplevel/nBlister1Enable
add wave -noupdate /tb_toplevel/Blister2Step
add wave -noupdate /tb_toplevel/Blister2Dir
add wave -noupdate /tb_toplevel/nBlister2Enable
add wave -noupdate /tb_toplevel/Blister3Step
add wave -noupdate /tb_toplevel/Blister3Dir
add wave -noupdate /tb_toplevel/nBlister3Enable
add wave -noupdate /tb_toplevel/MechValveStep
add wave -noupdate /tb_toplevel/MechValveDir
add wave -noupdate /tb_toplevel/nMechValveEnable
add wave -noupdate /tb_toplevel/PwrGrnLed
add wave -noupdate /tb_toplevel/PwrLaTest
add wave -noupdate /tb_toplevel/clk2
add wave -noupdate -radix unsigned /tb_toplevel/pcount
add wave -noupdate /tb_toplevel/tog
add wave -noupdate -radix unsigned /tb_toplevel/tbc/bitcount
add wave -noupdate -radix unsigned /tb_toplevel/tbc/wordcount
TreeUpdate [SetDefaultTree]
WaveRestoreCursors {{Cursor 1} {363692 ps} 0}
quietly wave cursor active 1
configure wave -namecolwidth 150
configure wave -valuecolwidth 38
configure wave -justifyvalue left
configure wave -signalnamewidth 1
configure wave -snapdistance 10
configure wave -datasetprefix 0
configure wave -rowmargin 4
configure wave -childrowmargin 2
configure wave -gridoffset 0
configure wave -gridperiod 1
configure wave -griddelta 40
configure wave -timeline 0
configure wave -timelineunits ns
update
WaveRestoreZoom {0 ps} {2155736 ps}
