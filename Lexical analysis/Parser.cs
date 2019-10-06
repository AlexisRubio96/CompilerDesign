/*
 * A01373670 - Rodrigo Garcia Lopez
 * A01372074 - Jorge Alexis Rubio Sumano
 * A01371084 - Valentin Ochoa Lopez
*/


using System;
using System.Collections.Generic;

namespace Chimera
{

    public class Parser
    {
        private IEnumerator<Token> tokenStream;

        public Parser(IEnumerator<Token> tokenStream)
        {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }

        private TokenCategory CurrentToken
        {
            get { return tokenStream.Current.Category; }
        }

        private Token Expect(TokenCategory category)
        {
            if (CurrentToken == category)
            {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            }
            else
            {
                throw new SyntaxError(category, tokenStream.Current);
            }
        }

        /* 
         * LL(1) Grammar:
         *      PROGRAM ::= ("const" CONST_DECL+)? ("var" VAR_DECL+)? PROC_DECL* "program" STATEMENT* "end" ";"
         *         
         * 
         * 
         * 
         */

        public void Program()
        {
            Console.WriteLine("Test");
        }
    }
}
