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

        private static readonly ISet<TokenCategory> firstOfSimpleExpression =
            new HashSet<TokenCategory>() {
                TokenCategory.LEFT_PAR,
                TokenCategory.IDENTIFIER,
                TokenCategory.INT_LITERAL,
                TokenCategory.STR_LITERAL,
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.LEFT_BRACES
            };

        private static readonly ISet<TokenCategory> firstOfExpression =
            new HashSet<TokenCategory>() {
                TokenCategory.NOT,
                TokenCategory.MINUS,
                TokenCategory.LEFT_PAR,
                TokenCategory.IDENTIFIER,
                TokenCategory.INT_LITERAL,
                TokenCategory.STR_LITERAL,
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.LEFT_BRACES
            };

        private static readonly ISet<TokenCategory> firstOfUnaryExpression = firstOfExpression;

        private static readonly ISet<TokenCategory> logicOperators =
            new HashSet<TokenCategory>() {
                TokenCategory.AND,
                TokenCategory.OR,
                TokenCategory.XOR
            };

        private static readonly ISet<TokenCategory> relationalOperators =
            new HashSet<TokenCategory>() {
                TokenCategory.EQUAL,
                TokenCategory.NOT_EQUAL,
                TokenCategory.SMALLER,
                TokenCategory.GREATER,
                TokenCategory.SMALLER_EQ,
                TokenCategory.GREATER_EQ
            };

        private static readonly ISet<TokenCategory> sumOperators =
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS,
                TokenCategory.MINUS
            };

        private static readonly ISet<TokenCategory> mulOperators =
            new HashSet<TokenCategory>() {
                TokenCategory.MUL,
                TokenCategory.DIV,
                TokenCategory.REM
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
         *      PROC_DECL ::= "procedure" IDENTIFIER "(" PARAM_DECL* ")" (":" TYPE)? ";" ("const" CONST_DECL+)? ("var" VAR_DECL+)? "begin" STATEMENT* "end" ";"
         *      PARAM_DECL ::= IDENTIFIER ("," IDENTIFIER)* ":" TYPE ";"
         *      STATEMENT ::= (IDENTIFIER (ASS_STAT | CALL_STAT) ) | IF_STAT | LOOP_STAT | FOR_STAT | RET_STAT | EXIT_STAT
         *      ASS_STAT ::= ("[" EXPR "]")? ":=" EXPR ";"
         *      CALL_STAT ::= "(" (EXPR ("," EXPR)*)? ")" ";"
         *      IF_STAT ::= "if" EXPR "then" STATEMENT* ("elseif" EXPR "THEN" STATEMENT*)* ("else" STATEMENT*)? "end" ";"        
         *      LOOP_STAT ::= "loop" STATEMENT* "end" ";"     
         *      FOR_STAT ::= "for" IDENTIFIER "in" EXPR "do" STATEMENT* "end" ";"    
         *      RET_STAT ::= "return" EXPR? ";"
         *      EXIT_STAT ::= "exit" ";"   
         *      EXPR ::= LOGIC_EXPR
         *      LOGIC_EXPR ::= REL_EXPR (LOGIC_OP REL_EXPR)*
         *      LOGIC_OP ::= "and" | "or" | "xor"
         *      REL_EXPR ::= SUM_EXPR (REL_OP SUM_EXPR)*
         *      REL_OP ::= "=" | "<>" | "<" | ">" | "<=" | ">="
         *      SUM_EXPR ::= MUL_EXPR (SUM_OP MUL_EXPR)*     
         *      SUM_OP ::= "+" | "-"
         *      MUL_EXPR ::= UN_EXPR (MUL_OP UN_EXPR)*
         *      MUL_OP ::=  "*" | "div" | "rem"   
         *      UN_EXPR ::= ("not" UN_EXPR) | ("-" UN_EXPR) | SIMP_EXPR
         *      SIMP_EXPR ::= ("(" Expression ")" | (IDENTIFIER CALL?) | LITERAL ) ("[" EXPR "]")?            
         *      CALL ::= "(" (EXPR ("," EXPR)*)? ")"
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
            Expect(TokenCategory.PROCEDURE);
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.LEFT_PAR);

            while (CurrentToken == TokenCategory.IDENTIFIER)
            {
                ParameterDeclaration();
            }

            Expect(TokenCategory.RIGHT_PAR);

            if (CurrentToken == TokenCategory.COLON)
            {
                Expect(TokenCategory.COLON);
                Type();
            }

            Expect(TokenCategory.SEMICOLON);

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

            Expect(TokenCategory.BEGIN);

            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.SEMICOLON);
        }

        public void ParameterDeclaration()
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

        public void Statement()
        {
            switch (CurrentToken)
            {

                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);

                    if (CurrentToken == TokenCategory.LEFT_SQR_BRACK || CurrentToken == TokenCategory.ASSIGN_CONST)
                    {
                        AssignmentStatement();
                        break;
                    }
                    else if (CurrentToken == TokenCategory.LEFT_PAR)
                    {
                        CallStatement();
                        break;
                    }
                    else
                    {
                        throw new SyntaxError(firstOfStatement, tokenStream.Current);
                    }

                case TokenCategory.IF:
                    IfStatement();
                    break;

                case TokenCategory.LOOP:
                    LoopStatement();
                    break;

                case TokenCategory.FOR:
                    ForStatement();
                    break;
                case TokenCategory.RETURN:
                    ReturnStatement();
                    break;
                case TokenCategory.EXIT:
                    ExitStatement();
                    break;
                default:
                    throw new SyntaxError(firstOfStatement, tokenStream.Current);
            }
        }

        public void AssignmentStatement()
        {
            if (CurrentToken == TokenCategory.LEFT_SQR_BRACK)
            {
                Expect(TokenCategory.LEFT_SQR_BRACK);
                Expression();
                Expect(TokenCategory.RIGHT_SQR_BRACK);
            }

            Expect(TokenCategory.ASSIGN_CONST);
            Expression();
            Expect(TokenCategory.SEMICOLON);
        }

        public void CallStatement()
        {
            Expect(TokenCategory.LEFT_PAR);

            if (firstOfExpression.Contains(CurrentToken))
            {
                Expression();

                while (CurrentToken == TokenCategory.COMA)
                {
                    Expect(TokenCategory.COMA);
                    Expression();
                }
            }

            Expect(TokenCategory.RIGHT_PAR);
            Expect(TokenCategory.SEMICOLON);
        }

        public void IfStatement()
        {
            Expect(TokenCategory.IF);
            Expression();
            Expect(TokenCategory.THEN);

            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }

            while (CurrentToken == TokenCategory.ELSEIF)
            {
                Expect(TokenCategory.ELSEIF);
                Expression();
                Expect(TokenCategory.THEN);

                while (firstOfStatement.Contains(CurrentToken))
                {
                    Statement();
                }
            }

            if (CurrentToken == TokenCategory.ELSE)
            {
                Expect(TokenCategory.ELSE);

                while (firstOfStatement.Contains(CurrentToken))
                {
                    Statement();
                }
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.SEMICOLON);
        }

        public void LoopStatement()
        {
            Expect(TokenCategory.LOOP);

            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.SEMICOLON);
        }

        public void ForStatement()
        {
            Expect(TokenCategory.FOR);
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.IN);
            Expression();
            Expect(TokenCategory.DO);

            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.SEMICOLON);
        }

        public void ReturnStatement()
        {
            Expect(TokenCategory.RETURN);

            if (firstOfExpression.Contains(CurrentToken))
            {
                Expression();
            }

            Expect(TokenCategory.SEMICOLON);
        }

        public void ExitStatement()
        {
            Expect(TokenCategory.EXIT);
            Expect(TokenCategory.SEMICOLON);
        }

        public void Expression()
        {
            LogicExpression();
        }

        public void LogicExpression()
        {
            RelationalExpression();

            while (logicOperators.Contains(CurrentToken))
            {
                LogicOperator();
                RelationalExpression();
            }
        }

        public void LogicOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.AND:
                    Expect(TokenCategory.AND);
                    break;

                case TokenCategory.XOR:
                    Expect(TokenCategory.XOR);
                    break;

                case TokenCategory.OR:
                    Expect(TokenCategory.OR);
                    break;

                default:
                    throw new SyntaxError(logicOperators, tokenStream.Current);
            }
        }

        public void RelationalExpression()
        {
            SumExpression();

            while (relationalOperators.Contains(CurrentToken))
            {
                RelationalOperator();
                SumExpression();
            }
        }

        public void RelationalOperator() 
        { 
            switch (CurrentToken)
            {
                case TokenCategory.EQUAL:
                    Expect(TokenCategory.EQUAL);
                    break;

                case TokenCategory.NOT_EQUAL:
                    Expect(TokenCategory.NOT_EQUAL);
                    break;

                case TokenCategory.SMALLER:
                    Expect(TokenCategory.SMALLER);
                    break;

                case TokenCategory.GREATER:
                    Expect(TokenCategory.GREATER);
                    break;

                case TokenCategory.SMALLER_EQ:
                    Expect(TokenCategory.SMALLER_EQ);
                    break;

                case TokenCategory.GREATER_EQ:
                    Expect(TokenCategory.GREATER_EQ);
                    break;

                default:
                    throw new SyntaxError(relationalOperators, tokenStream.Current);
            }
        }

        public void SumExpression()
        {
            MulExpression();

            while (sumOperators.Contains(CurrentToken))
            {
                SumOperator();
                MulExpression();
            }
        }

        public void SumOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.PLUS:
                    Expect(TokenCategory.PLUS);
                    break;

                case TokenCategory.MINUS:
                    Expect(TokenCategory.MINUS);
                    break;

                default:
                    throw new SyntaxError(sumOperators, tokenStream.Current);
            }
        }

        public void MulExpression()
        {
            UnaryExpression();

            while (mulOperators.Contains(CurrentToken))
            {
                MulOperator();
                UnaryExpression();
            }
        }

        public void MulOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.MUL:
                    Expect(TokenCategory.MUL);
                    break;

                case TokenCategory.DIV:
                    Expect(TokenCategory.DIV);
                    break;

                case TokenCategory.REM:
                    Expect(TokenCategory.REM);
                    break;

                default:
                    throw new SyntaxError(mulOperators, tokenStream.Current);
            }
        }

        public void UnaryExpression()
        {
            if (CurrentToken == TokenCategory.NOT)
            {
                Expect(TokenCategory.NOT);
                UnaryExpression();
            } 
            else if (CurrentToken == TokenCategory.MINUS)
            {
                Expect(TokenCategory.MINUS);
                UnaryExpression();
            }
            else if (firstOfSimpleExpression.Contains(CurrentToken))
            {
                SimpleExpression();
            }
            else
            {
                throw new SyntaxError(firstOfUnaryExpression, tokenStream.Current);
            }
        }

        public void SimpleExpression()
        {
            if (CurrentToken == TokenCategory.LEFT_PAR)
            {
                Expect(TokenCategory.LEFT_PAR);
                Expression();
                Expect(TokenCategory.RIGHT_PAR);
            }
            else if (CurrentToken == TokenCategory.IDENTIFIER)
            {
                Expect(TokenCategory.IDENTIFIER); 

                if (CurrentToken == TokenCategory.LEFT_PAR)
                {
                    Call();
                }
            }
            else if (firstOfLiteral.Contains(CurrentToken))
            {
                Literal();
            }
            else
            {
                throw new SyntaxError(firstOfSimpleExpression, tokenStream.Current);
            }

            if (CurrentToken == TokenCategory.LEFT_SQR_BRACK)
            {
                Expect(TokenCategory.LEFT_SQR_BRACK);
                Expression();
                Expect(TokenCategory.RIGHT_SQR_BRACK);
            }
        }

        public void Call()
        {
            Expect(TokenCategory.LEFT_PAR);

            if (firstOfExpression.Contains(CurrentToken))
            {
                Expression();

                while (CurrentToken == TokenCategory.COMA)
                {
                    Expect(TokenCategory.COMA);
                    Expression();
                }
            }

            Expect(TokenCategory.RIGHT_PAR);
        }

    }
}
