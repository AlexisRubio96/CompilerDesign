#
# Chimera compiler - Project make file: 
# Copyright (C) 2019 by Valentín Ochoa López, Rodrigo García López & 
#	Jorge Alexis Rubio 
#  
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#  
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#  
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.
#



chimera.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs Parser.cs SyntaxError.cs Node.cs SpecificNodes.cs SemanticError.cs SemanticAnalyzer.cs Type.cs GlobalSymbolTable.cs Table.cs LocalSymbolTable.cs GlobalProcedureTable.cs Clasification.cs GlobalSymbol.cs GlobalProcedure.cs LocalSymbol.cs CILGenerator.cs    
	mcs -out:chimera.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs Parser.cs SyntaxError.cs Node.cs SpecificNodes.cs SemanticError.cs SemanticAnalyzer.cs Type.cs GlobalSymbolTable.cs Table.cs LocalSymbolTable.cs GlobalProcedureTable.cs Clasification.cs GlobalSymbol.cs GlobalProcedure.cs LocalSymbol.cs CILGenerator.cs

			
clean:
	rm *.exe
	rm *.il
