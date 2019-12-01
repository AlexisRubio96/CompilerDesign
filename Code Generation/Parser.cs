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

        public Node Program()
        {
            var result = new Program();

            var constantDeclarationList = new ConstantDeclarationList();
            if (CurrentToken == TokenCategory.CONST)
            {
                Expect(TokenCategory.CONST);
                do
                {
                    constantDeclarationList.Add(ConstantDeclaration());
                } while (CurrentToken == TokenCategory.IDENTIFIER);
            }
            result.Add(constantDeclarationList);

            var variableDeclarationList = new VariableDeclarationList();
            if (CurrentToken == TokenCategory.VAR)
            {
                Expect(TokenCategory.VAR);
                do
                {
                    variableDeclarationList.Add(VariableDeclaration());
                } while (CurrentToken == TokenCategory.IDENTIFIER);
            }
            result.Add(variableDeclarationList);

            var procedureDeclarationList = new ProcedureDeclarationList();
            if (CurrentToken == TokenCategory.PROCEDURE)
            {
                do
                {
                    procedureDeclarationList.Add(ProcedureDeclaration());
                } while (CurrentToken == TokenCategory.PROCEDURE);
            }
            result.Add(procedureDeclarationList);

            Expect(TokenCategory.PROGRAM);

            var statementList = new StatementList();
            while (firstOfStatement.Contains(CurrentToken))
            {
                statementList.Add(Statement());
            }
            result.Add(statementList);

            Expect(TokenCategory.END);
            Expect(TokenCategory.SEMICOLON);
            Expect(TokenCategory.EOF);

            return result; 
        }

        public Node ConstantDeclaration()
        {
            var result = new ConstantDeclaration()
                { AnchorToken = Expect(TokenCategory.IDENTIFIER) };
            Expect(TokenCategory.ASSIGN_CONST);
            result.Add(Literal());
            Expect(TokenCategory.SEMICOLON);

            return result;
        }

        public Node VariableDeclaration()
        {
            var result = new VariableDeclaration();

            var identifierList = new IdentifierList() 
                { new Identifier() { AnchorToken = Expect(TokenCategory.IDENTIFIER) } };
            result.Add(identifierList);

            while (CurrentToken == TokenCategory.COMA)
            {
                Expect(TokenCategory.COMA);
                identifierList.Add(new Identifier() { AnchorToken = Expect(TokenCategory.IDENTIFIER) });
            }

            Expect(TokenCategory.COLON);
            result.Add(Type());
            Expect(TokenCategory.SEMICOLON);

            return result;
        }

        public Node Literal()
        {
            if (firstOfLiteral.Contains(CurrentToken))
            {
                if (CurrentToken == TokenCategory.LEFT_BRACES)
                {
                    return Lst();
                }
                else
                {
                    return SimpleLiteral();
                }
            }
            else
            {
                throw new SyntaxError(firstOfLiteral, tokenStream.Current);
            }
        }

        public Node SimpleLiteral()
        {
            switch (CurrentToken)
            { 
                case TokenCategory.INT_LITERAL:
                    return new IntegerLiteral() { AnchorToken = Expect(TokenCategory.INT_LITERAL) };

                case TokenCategory.STR_LITERAL:
                    return new StringLiteral() { AnchorToken = Expect(TokenCategory.STR_LITERAL) };

                case TokenCategory.TRUE:
                    return new True() { AnchorToken = Expect(TokenCategory.TRUE) };

                case TokenCategory.FALSE:
                    return new False() { AnchorToken = Expect(TokenCategory.FALSE) };

                default:
                    throw new SyntaxError(simpleLiterals, tokenStream.Current);
            }
        }

        public Node Type()
        {
            if (firstOfTypes.Contains(CurrentToken))
            {
                if (CurrentToken == TokenCategory.LIST)
                {
                    return ListType();
                }
                else
                {
                    return SimpleType();
                }
            }
            else
            {
                throw new SyntaxError(firstOfTypes, tokenStream.Current);
            }
        }

        public Node SimpleType()
        {
            switch (CurrentToken)
            {
                case TokenCategory.INTEGER:
                    return new SimpleType() { AnchorToken = Expect(TokenCategory.INTEGER) };

                case TokenCategory.STRING:
                    return new SimpleType() { AnchorToken = Expect(TokenCategory.STRING) };

                case TokenCategory.BOOLEAN:
                    return new SimpleType() { AnchorToken = Expect(TokenCategory.BOOLEAN) };

                default:
                    throw new SyntaxError(simpleTypes, tokenStream.Current);
            }
        }

        public Node ListType()
        {
            Expect(TokenCategory.LIST);
            Expect(TokenCategory.OF);
            return new ListType() { AnchorToken = SimpleType().AnchorToken};
        }

        public Node Lst()
        {
            var result = new Lst();

            Expect(TokenCategory.LEFT_BRACES);

            if (simpleLiterals.Contains(CurrentToken))
            { 
                result.Add(SimpleLiteral());

                while (CurrentToken == TokenCategory.COMA)
                {
                    Expect(TokenCategory.COMA);
                    result.Add(SimpleLiteral());
                }
            }

            Expect(TokenCategory.RIGHT_BRACES);

            return result;
        }

        public Node ProcedureDeclaration()
        {
            var result = new ProcedureDeclaration();

            Expect(TokenCategory.PROCEDURE);
            result.AnchorToken = Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.LEFT_PAR);

            var parameterDeclarationList = new ParameterDeclarationList();
            while (CurrentToken == TokenCategory.IDENTIFIER)
            {
                parameterDeclarationList.Add(ParameterDeclaration());
            }
            result.Add(parameterDeclarationList);

            Expect(TokenCategory.RIGHT_PAR);

            if (CurrentToken == TokenCategory.COLON)
            {
                Expect(TokenCategory.COLON);
                result.Add(Type());
            }

            Expect(TokenCategory.SEMICOLON);

            if (CurrentToken == TokenCategory.CONST)
            {
                var constantDeclarationList = new ConstantDeclarationList()
                    { AnchorToken = Expect(TokenCategory.CONST) };
                do
                {
                    constantDeclarationList.Add(ConstantDeclaration());
                } while (CurrentToken == TokenCategory.IDENTIFIER);

                result.Add(constantDeclarationList);
            }

            if (CurrentToken == TokenCategory.VAR)
            {
                var variableDeclarationList = new VariableDeclarationList()
                    { AnchorToken = Expect(TokenCategory.VAR) };
                do
                {
                    variableDeclarationList.Add(VariableDeclaration());
                } while (CurrentToken == TokenCategory.IDENTIFIER);

                result.Add(variableDeclarationList);
            }

            Expect(TokenCategory.BEGIN);

            var statementList = new StatementList();
            while (firstOfStatement.Contains(CurrentToken))
            {
                statementList.Add(Statement());
            }
            result.Add(statementList);

            Expect(TokenCategory.END);
            Expect(TokenCategory.SEMICOLON);

            return result;
        }

        public Node ParameterDeclaration()
        {
            var result = new ParameterDeclaration();

            var identifierList = new IdentifierList()
                { new Identifier() { AnchorToken = Expect(TokenCategory.IDENTIFIER) } };
            result.Add(identifierList);

           while (CurrentToken == TokenCategory.COMA)
            {
                Expect(TokenCategory.COMA);
                identifierList.Add(new Identifier() { AnchorToken = Expect(TokenCategory.IDENTIFIER) });
            }

            Expect(TokenCategory.COLON);
            result.Add(Type());
            Expect(TokenCategory.SEMICOLON);

            return result;
        }

        public Node Statement()
        {
            switch (CurrentToken)
            {
                case TokenCategory.IDENTIFIER:
                    Token anchorToken = Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.LEFT_SQR_BRACK || CurrentToken == TokenCategory.ASSIGN_CONST)
                    {
                        return AssignmentStatement(anchorToken);
                    }
                    else if (CurrentToken == TokenCategory.LEFT_PAR)
                    {
                        var result = CallStatement();
                        result.AnchorToken = anchorToken;
                        return result;
                    }
                    else
                    {
                        throw new SyntaxError(firstOfStatement, tokenStream.Current);
                    }

                case TokenCategory.IF:
                    return IfStatement();

                case TokenCategory.LOOP:
                    return LoopStatement();

                case TokenCategory.FOR:
                    return ForStatement();

                case TokenCategory.RETURN:
                    return ReturnStatement();

                case TokenCategory.EXIT:
                    return ExitStatement();

                default:
                    throw new SyntaxError(firstOfStatement, tokenStream.Current);
            }
        }

        public Node AssignmentStatement(Token anchorToken)
        {
            var result = new AssignmentStatement();

            if (CurrentToken == TokenCategory.LEFT_SQR_BRACK)
            {
                var listIndexExpression = new ListIndexExpression();
                listIndexExpression.Add(new Identifier(){ AnchorToken = anchorToken });
                Expect(TokenCategory.LEFT_SQR_BRACK);
                listIndexExpression.Add(Expression());
                Expect(TokenCategory.RIGHT_SQR_BRACK);
                result.Add(listIndexExpression);
            }
            else
            {
                result.Add(new Identifier(){ AnchorToken = anchorToken });
            }

            result.AnchorToken = Expect(TokenCategory.ASSIGN_CONST);
            result.Add(Expression());
            Expect(TokenCategory.SEMICOLON);

            return result;
        }

        public Node CallStatement()
        {
            var result = new CallStatement();

            Expect(TokenCategory.LEFT_PAR);
           
            if (firstOfExpression.Contains(CurrentToken))
            {
                result.Add(Expression());

                while (CurrentToken == TokenCategory.COMA)
                {
                    Expect(TokenCategory.COMA);
                    result.Add(Expression());
                }
            }

            Expect(TokenCategory.RIGHT_PAR);
            Expect(TokenCategory.SEMICOLON);

            return result;
        }

        public Node IfStatement()
        {
            var result = new IfStatement();

            var ifClause = new IfClause() { AnchorToken = Expect(TokenCategory.IF) };
            ifClause.Add(Expression());
            Expect(TokenCategory.THEN);

            while (firstOfStatement.Contains(CurrentToken))
            {
                ifClause.Add(Statement());
            }

            result.Add(ifClause);

            while (CurrentToken == TokenCategory.ELSEIF)
            {
                var elseIfClause = new ElseIfClause() { AnchorToken = Expect(TokenCategory.ELSEIF) };
                elseIfClause.Add(Expression());
                Expect(TokenCategory.THEN);

                while (firstOfStatement.Contains(CurrentToken))
                {
                    elseIfClause.Add(Statement());
                }

                result.Add(elseIfClause);
            }

            if (CurrentToken == TokenCategory.ELSE)
            {
                var elseClause = new ElseClause() { AnchorToken = Expect(TokenCategory.ELSE) };

                while (firstOfStatement.Contains(CurrentToken))
                {
                    elseClause.Add(Statement());
                }

                result.Add(elseClause);
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.SEMICOLON);

            return result;
        }

        public Node LoopStatement()
        {
            var result = new LoopStatement() { AnchorToken = Expect(TokenCategory.LOOP) };

            while (firstOfStatement.Contains(CurrentToken))
            {
                result.Add(Statement());
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.SEMICOLON);

            return result;
        }

        public Node ForStatement()
        {
            var result = new ForStatement() { AnchorToken = Expect(TokenCategory.FOR) };

            result.Add(new Identifier() { AnchorToken = Expect(TokenCategory.IDENTIFIER) });
            Expect(TokenCategory.IN);
            result.Add(Expression());
            Expect(TokenCategory.DO);

            while (firstOfStatement.Contains(CurrentToken))
            {
                result.Add(Statement());
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.SEMICOLON);

            return result;
        }

        public Node ReturnStatement()
        {
            var result = new ReturnStatement() { AnchorToken = Expect(TokenCategory.RETURN) };

            if (firstOfExpression.Contains(CurrentToken))
            {
                result.Add(Expression());
            }
            Expect(TokenCategory.SEMICOLON);

            return result; 
        }

        public Node ExitStatement()
        {
            var result = new ExitStatement() { AnchorToken = Expect(TokenCategory.EXIT) };
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node Expression()
        {
            return LogicExpression();
        }

        public Node LogicExpression()
        {
            var expr1 = RelationalExpression();

            while (logicOperators.Contains(CurrentToken))
            {
                var expr2 = LogicOperator();
                expr2.Add(expr1);
                expr2.Add(RelationalExpression());
                expr1 = expr2;
            }

            return expr1;
        }

        public Node LogicOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.AND:
                    return new And() { AnchorToken = Expect(TokenCategory.AND) };

                case TokenCategory.XOR:
                    return new Xor() { AnchorToken = Expect(TokenCategory.XOR) };

                case TokenCategory.OR:
                    return new Or() { AnchorToken = Expect(TokenCategory.OR) };

                default:
                    throw new SyntaxError(logicOperators, tokenStream.Current);
            }
        }

        public Node RelationalExpression()
        {
            var expr1 = SumExpression();

            while (relationalOperators.Contains(CurrentToken))
            {
                var expr2 = RelationalOperator();
                expr2.Add(expr1);
                expr2.Add(SumExpression());
                expr1 = expr2;
            }

            return expr1;
        }

        public Node RelationalOperator() 
        { 
            switch (CurrentToken)
            {
                case TokenCategory.EQUAL:
                    return new Equal() { AnchorToken = Expect(TokenCategory.EQUAL) };

                case TokenCategory.NOT_EQUAL:
                    return new NotEqual() { AnchorToken = Expect(TokenCategory.NOT_EQUAL) };

                case TokenCategory.SMALLER:
                    return new Smaller() { AnchorToken = Expect(TokenCategory.SMALLER) };

                case TokenCategory.GREATER:
                    return new Greater() { AnchorToken = Expect(TokenCategory.GREATER) };

                case TokenCategory.SMALLER_EQ:
                    return new SmallerEq() { AnchorToken = Expect(TokenCategory.SMALLER_EQ) };

                case TokenCategory.GREATER_EQ:
                    return new GreaterEq() { AnchorToken = Expect(TokenCategory.GREATER_EQ) };

                default:
                    throw new SyntaxError(relationalOperators, tokenStream.Current);
            }
        }

        public Node SumExpression()
        {
            var expr1 = MulExpression();

            while (sumOperators.Contains(CurrentToken))
            {
                var expr2 = SumOperator();
                expr2.Add(expr1);
                expr2.Add(MulExpression());
                expr1 = expr2;
            }
            return expr1;
        }

        public Node SumOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.PLUS:
                    return new Plus() { AnchorToken = Expect(TokenCategory.PLUS) };

                case TokenCategory.MINUS:
                    return new Minus() { AnchorToken = Expect(TokenCategory.MINUS) };

            default:
                    throw new SyntaxError(sumOperators, tokenStream.Current);
            }
        }

        public Node MulExpression()
        {
            var expr1 = UnaryExpression();

            while (mulOperators.Contains(CurrentToken))
            {
                var expr2 = MulOperator();
                expr2.Add(expr1);
                expr2.Add(UnaryExpression());
                expr1 = expr2;
            }

            return expr1;
        }

        public Node MulOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.MUL:
                    return new Mul() { AnchorToken = Expect(TokenCategory.MUL) };

                case TokenCategory.DIV:
                    return new Div() { AnchorToken = Expect(TokenCategory.DIV) };

                case TokenCategory.REM:
                    return new Rem() { AnchorToken = Expect(TokenCategory.REM) };

                default:
                    throw new SyntaxError(mulOperators, tokenStream.Current);
            }
        }

        public Node UnaryExpression()
        {
            if (CurrentToken == TokenCategory.NOT)
            {
                var result = new Not() { AnchorToken = Expect(TokenCategory.NOT) };
                result.Add(UnaryExpression());
                return result;
            } 
            else if (CurrentToken == TokenCategory.MINUS)
            {
                var result = new Negative() { AnchorToken = Expect(TokenCategory.MINUS) };
                result.Add(UnaryExpression());
                return result;
            }
            else if (firstOfSimpleExpression.Contains(CurrentToken))
            {
                return SimpleExpression();
            }
            else
            {
                throw new SyntaxError(firstOfUnaryExpression, tokenStream.Current);
            }
        }

        public Node SimpleExpression()
        {
            Node result;

            if (CurrentToken == TokenCategory.LEFT_PAR)
            {
                Expect(TokenCategory.LEFT_PAR);
                result = Expression();
                Expect(TokenCategory.RIGHT_PAR);
            }
            else if (CurrentToken == TokenCategory.IDENTIFIER)
            {
                Token anchorToken = Expect(TokenCategory.IDENTIFIER); 

                if (CurrentToken == TokenCategory.LEFT_PAR)
                {
                    var call = Call();
                    call.AnchorToken = anchorToken;
                    result = call;
                }
                else
                {
                    result = new Identifier() { AnchorToken = anchorToken }; 
                }
            }
            else if (firstOfLiteral.Contains(CurrentToken))
            {
                result = Literal();
            }
            else
            {
                throw new SyntaxError(firstOfSimpleExpression, tokenStream.Current);
            }

            if (CurrentToken == TokenCategory.LEFT_SQR_BRACK)
            {
                var tempresult = new ListIndexExpression() { result };
                Expect(TokenCategory.LEFT_SQR_BRACK);
                tempresult.Add(Expression());
                Expect(TokenCategory.RIGHT_SQR_BRACK);
                result = tempresult;
            }

            return result;
        }

        public Node Call()
        {
            var result = new Call();

            Expect(TokenCategory.LEFT_PAR);
            if (firstOfExpression.Contains(CurrentToken))
            {
                result.Add(Expression());

                while (CurrentToken == TokenCategory.COMA)
                {
                    Expect(TokenCategory.COMA);
                    result.Add(Expression());
                }
            }
            Expect(TokenCategory.RIGHT_PAR);

            return result;
        }

    }
}
