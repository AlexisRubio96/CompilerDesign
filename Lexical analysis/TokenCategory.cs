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
        EOF,
        IDENTIFIER,
        INT_LITERAL,
        RIGHT_PAR,
        LEFT_PAR,
        GREATER_EQ,
        SMALLER_EQ,
        ASSIGN_CONST,
        NOT_EQUAL,
        SEMICOLON,
        GREATER,
        SMALLER,
        EQUAL,
        COMA,
        COLON,
        PLUS,
        MINUS,
        MUL,
        ILLEGAL_CHAR,
        VAR,
        PROGRAM,
        END,
        INTEGER,
        STRING,
        BOOLEAN,
        LIST,
        OF,
        PROCEDURE,
        CONST,
        BEGIN,
        IF,
        ELSEIF,
        THEN,
        ELSE,
        LOOP,
        FOR,
        IN,
        DO,
        RETURN,
        AND,
        XOR,
        OR,
        DIV,
        REM,
        NOT
    }
}

