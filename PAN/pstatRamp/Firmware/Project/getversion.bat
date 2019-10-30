chdir /d %1
"C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" .. BuildNumberTemplate.h version.h
rem Just echo something to get rid of the return value of SubWCRev.exe
echo " "