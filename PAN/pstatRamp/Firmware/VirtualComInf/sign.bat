@ECHO OFF

SET SIGNTOOL=%1..\WinDDK\bin\x86\SignTool.exe
SET INF2CAT=%1..\WinDDK\bin\selfsign\Inf2Cat.exe
SET SIGNOPTS=sign /v /ac %1MSCV-GlobalSign.cer /s my /n "The Technology Partnership plc" /t http://timestamp.verisign.com/scripts/timestamp.dll

ECHO Signing drivers...


ECHO Creating catalogue file...

%INF2CAT% /driver:%1. /os:XP_X86,XP_X64,Vista_X86,Vista_x64,7_X86,7_X64 /verbose

ECHO Signing catalogue file...

REM *** MSCV-GlobalSign.cer is the FULL path to the Global Sign Cross Certificate ***
REM *** /n means look for a "personal" certificate (which is how we installed it) ***
REM *** "The Technology Partnership plc" is the name of the TTP Certificate (shouldn't change) ***

%SIGNTOOL% %SIGNOPTS% %1IoVirComport.cat
