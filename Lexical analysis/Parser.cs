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
        private static readonly ISet<TokenCategory> firstOfStatement =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.IF,
                TokenCategory.LOOP,
                TokenCategory.FOR,
                TokenCategory.RETURN,
                TokenCategory.EXIT
            };

        private static readonly ISet<TokenCategory> firstOfLiteral =
            new HashSet<TokenCategory>() {
                TokenCategory.INT_LITERAL,
                TokenCategory.STR_LITERAL,
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.LEFT_BRACES
            };

        private static readonly ISet<TokenCategory> simpleLiterals =
            new HashSet<TokenCategory>() {
                TokenCategory.INT_LITERAL,
                TokenCategory.STR_LITERAL,
                TokenCategory.TRUE,
                TokenCategory.FALSE
            };

        private static readonly ISet<TokenCategory> firstOfTypes =
            new HashSet<TokenCategory>() {
                TokenCategory.INTEGER,
                TokenCategory.STRING,
                TokenCategory.BOOLEAN,
                TokenCategory.LIST
            };

        private static readonly ISet<TokenCategory> simpleTypes =
            new HashSet<TokenCategory>() {
                TokenCategory.INTEGER,
                TokenCategory.STRING,
                TokenCategory.BOOLEAN
            };

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
         *      CONST_DECL ::= IDENTIFIER ":=" LITERAL ";"    
         *      VAR_DECL ::= IDENTIFIER ("," IDENTIFIER)* ":" TYPE ";"
         *      LITERAL ::= SIMPLE_LITERAL | LIST
         *      SIMPLE_LITERAL ::= INTEGER_LITERAL | STRING_LITERAL | TRUE | FALSE
         *      TYPE ::= SIMPLE_TYPE | LIST_TYPE
         *      SIMPLE_TYPE ::= "integer" | "string" | "boolean"
         *      LIST_TYPE ::= "list" "of" SIMPLE_TYPE
         *      LIST ::= "{" (SIMPLE_LITERAL ("," SIMPLE_LITERAL )*)? "}"        
         */

        public void Program()
        {
            if (CurrentToken == TokenCategory.CONST)
            {
                Expect(TokenCategory.CONST);
                do
                {
                    ConstantDeclaration();
                } while (CurrentToken == TokenCategory.IDENTIFIER);        
            }

            if (CurrentToken == TokenCategory.VAR)
            {
                Expect(TokenCategory.VAR);
                do
                {
                    VariableDeclaration();
                } while (CurrentToken == TokenCategory.IDENTIFIER);
            }

            while (CurrentToken == TokenCategory.PROCEDURE)
            {
                ProcedureDeclaration();
            }

            Expect(TokenCategory.PROGRAM);

            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.SEMICOLON);
            Expect(TokenCategory.EOF);
        }

        public void ConstantDeclaration()
        {
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.ASSIGN_CONST);
            Literal();
            Expect(TokenCategory.SEMICOLON);
        }

        public void VariableDeclaration()
        {
            Expect(TokenCategory.IDENTIFIER);

            while (CurrentToken == TokenCategory.COMA)
            {
                Expect(TokenCategory.COMA);
                Expect(TokenCategory.IDENTIFIER);
            }

            Expect(TokenCategory.COLON);
            Type();
            Expect(TokenCategory.SEMICOLON);
        }

        public void Literal()
        {
            if (firstOfLiteral.Contains(CurrentToken))
            {
                if (CurrentToken == TokenCategory.LEFT_BRACES)
                {
                    Lst();
                }
                else
                {
                    SimpleLiteral();
                }
            }
            else
            {
                throw new SyntaxError(firstOfLiteral, tokenStream.Current);
            }
        }

        public void SimpleLiteral()
        {
            switch (CurrentToken)
            { 
                case TokenCategory.INT_LITERAL:
                    Expect(TokenCategory.INT_LITERAL);
                    break;

                case TokenCategory.STR_LITERAL:
                    Expect(TokenCategory.STR_LITERAL);
                    break;

                case TokenCategory.TRUE:
                    Expect(TokenCategory.TRUE);
                    break;

                case TokenCategory.FALSE:
                    Expect(TokenCategory.FALSE);
                    break;

                default:
                    throw new SyntaxError(simpleLiterals, tokenStream.Current);
            }
        }

        public void Type()
        {
            if (firstOfTypes.Contains(CurrentToken))
            {
                if (CurrentToken == TokenCategory.LIST)
                {
                    ListType();
                }
                else
                {
                    SimpleType();
                }
            }
            else
            {
                throw new SyntaxError(firstOfTypes, tokenStream.Current);
            }
        }

        public void SimpleType()
        {
            switch (CurrentToken)
            {
                case TokenCategory.INTEGER:
                    Expect(TokenCategory.INTEGER);
                    break;

                case TokenCategory.STRING:
                    Expect(TokenCategory.STRING);
                    break;

                case TokenCategory.BOOLEAN:
                    Expect(TokenCategory.BOOLEAN);
                    break;

                default:
                    throw new SyntaxError(simpleTypes, tokenStream.Current);
            }
        }

        public void ListType()
        {
            Expect(TokenCategory.LIST);
            Expect(TokenCategory.OF);
            SimpleType();
        }

        public void Lst()
        {
            Expect(TokenCategory.LEFT_BRACES);

            if (simpleLiterals.Contains(CurrentToken))
            { 
                SimpleLiteral();

                while (CurrentToken == TokenCategory.COMA)
                {
                    Expect(TokenCategory.COMA);
                    SimpleLiteral();
                }
            }

            Expect(TokenCategory.RIGHT_BRACES);
        }

        public void ProcedureDeclaration()
        {
            return;
        }

        public void Statement()
        {
            return;
        }

    }
}
