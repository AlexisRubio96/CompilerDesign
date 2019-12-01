VAR="test5_variable_declaration"
./chimera.exe Tests/$VAR.chimera $VAR.il
ilasm $VAR.il
chmod +x $VAR.exe
./$VAR.exe


