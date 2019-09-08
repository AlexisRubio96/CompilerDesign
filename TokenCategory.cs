/*
  Chimera compiler - Token categories for the scanner.
  Copyright @2019 by Valentín Ochoa López, Rodrigo García López & 
  Jorge Alexis Rubio 

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.
  
  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  
  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace Chimera {

    enum TokenCategory {
        AND,
        ASSIGN_CONST,
        COLON,
        COMA,
        END,
        EOF,
        EQUAL,
        GREATER,
        GREATER_EQ,
        IDENTIFIER,
        IF,
        ILLEGAL_CHAR,
        INT_LITERAL,
        LEFT_BRACES,
        LEFT_PAR,
        LEFT_SQR_BRACK,
        MINUS,
        MUL,
        NOT_EQUAL,
        PLUS,
        PROGRAM,
        RIGHT_BRACES,
        RIGHT_PAR,
        RIGHT_SQR_BRACK,
        SEMICOLON,
        SMALLER,
        SMALLER_EQ,
        STRING,
    }
}

