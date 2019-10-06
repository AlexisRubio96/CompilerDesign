/*
 * A01373670 - Rodrigo Garcia Lopez
 * A01372074 - Jorge Alexis Rubio Sumano
 * A01371084 - Valentin Ochoa Lopez
*/

using System;

namespace Chimera
{

    public class Token
    {

        readonly string lexeme;

        readonly TokenCategory category;

        readonly int row;

        readonly int column;

        public string Lexeme
        {
            get { return lexeme; }
        }

        public TokenCategory Category
        {
            get { return category; }
        }

        public int Row
        {
            get { return row; }
        }

        public int Column
        {
            get { return column; }
        }

        public Token(string lexeme,
                     TokenCategory category,
                     int row,
                     int column)
        {
            this.lexeme = lexeme;
            this.category = category;
            this.row = row;
            this.column = column;
        }

        public override string ToString()
        {
            return string.Format("{{{0}, \"{1}\", @({2}, {3})}}",
                                 category, lexeme, row, column);
        }
    }
}