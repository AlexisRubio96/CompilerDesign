VAR="test6_lists"
./chimera.exe Tests/$VAR.chimera $VAR.il
ilasm $VAR.il
chmod +x $VAR.exe
./$VAR.exe


