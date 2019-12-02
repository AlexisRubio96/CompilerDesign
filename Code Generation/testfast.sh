VAR="test10_if_statement"
./chimera.exe Tests/$VAR.chimera $VAR.il
ilasm $VAR.il
chmod +x $VAR.exe
./$VAR.exe


