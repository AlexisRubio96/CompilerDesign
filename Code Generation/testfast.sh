VAR="test14_booleanOps"
./chimera.exe Tests/$VAR.chimera $VAR.il
ilasm $VAR.il
chmod +x $VAR.exe
./$VAR.exe


