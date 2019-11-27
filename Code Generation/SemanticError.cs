/*
 * A01373670 - Rodrigo Garcia Lopez
 * A01372074 - Jorge Alexis Rubio Sumano
 * A01371084 - Valentin Ochoa Lopez
*/


using System;

namespace Chimera
{

    public class SemanticError : Exception
    {

        public SemanticError(string message, Token token) :
            base(String.Format(
                "Semantic Error: {0} \n" + "at row {1}, column {2}.",
                message, token.Row, token.Column))
        {
        }
    }
}
