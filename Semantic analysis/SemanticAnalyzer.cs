/*
 * A01373670 - Rodrigo Garcia Lopez
 * A01372074 - Jorge Alexis Rubio Sumano
 * A01371084 - Valentin Ochoa Lopez
*/

using System;
using System.Collections.Generic;

namespace Chimera
{

    class SemanticAnalyzer
    {
    
        //-----------------------------------------------------------
        public GlobalSymbolTable GSTable
        {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public GlobalProcedureTable GPTable
        {
            get;
            private set;
        }

        //-----------------------------------------------------------
        private Int32 LoopNestingLevel
        {
            get;
            set;
        }

        //-----------------------------------------------------------
        public SemanticAnalyzer()
        {
            GSTable = new GlobalSymbolTable();
            GPTable = new GlobalProcedureTable();

            GPTable["WrInt"] = new GlobalProcedure(true, Type.VOID);
            GPTable["WrInt"].LocalSymbols["i"] = new LocalSymbol(0, Type.INTEGER);

            GPTable["WrStr"] = new GlobalProcedure(true, Type.VOID);
            GPTable["WrStr"].LocalSymbols["s"] = new LocalSymbol(0, Type.STRING);

            GPTable["WrBool"] = new GlobalProcedure(true, Type.VOID);
            GPTable["WrBool"].LocalSymbols["b"] = new LocalSymbol(0, Type.BOOLEAN);

            GPTable["WrLn"] = new GlobalProcedure(true, Type.VOID);

            GPTable["RdInt"] = new GlobalProcedure(true, Type.INTEGER);

            GPTable["RdStr"] = new GlobalProcedure(true, Type.STRING);

            GPTable["AtStr"] = new GlobalProcedure(true, Type.STRING);
            GPTable["AtStr"].LocalSymbols["s"] = new LocalSymbol(0, Type.STRING);
            GPTable["AtStr"].LocalSymbols["i"] = new LocalSymbol(1, Type.INTEGER);

            GPTable["LenStr"] = new GlobalProcedure(true, Type.INTEGER);
            GPTable["LenStr"].LocalSymbols["s"] = new LocalSymbol(0, Type.STRING);

            GPTable["CmpStr"] = new GlobalProcedure(true, Type.INTEGER);
            GPTable["CmpStr"].LocalSymbols["s1"] = new LocalSymbol(0, Type.STRING);
            GPTable["CmpStr"].LocalSymbols["s2"] = new LocalSymbol(1, Type.STRING);

            GPTable["CatStr"] = new GlobalProcedure(true, Type.STRING);
            GPTable["CatStr"].LocalSymbols["s1"] = new LocalSymbol(0, Type.STRING);
            GPTable["CatStr"].LocalSymbols["s2"] = new LocalSymbol(1, Type.STRING);

            GPTable["LenLstInt"] = new GlobalProcedure(true, Type.INTEGER);
            GPTable["LenLstInt"].LocalSymbols["loi"] = new LocalSymbol(0, Type.LIST_OF_INTEGER);

            GPTable["LenLstStr"] = new GlobalProcedure(true, Type.INTEGER);
            GPTable["LenLstStr"].LocalSymbols["los"] = new LocalSymbol(0, Type.LIST_OF_STRING);

            GPTable["LenLstBool"] = new GlobalProcedure(true, Type.INTEGER);
            GPTable["LenLstBool"].LocalSymbols["lob"] = new LocalSymbol(0, Type.LIST_OF_BOOLEAN);

            GPTable["NewLstInt"] = new GlobalProcedure(true, Type.LIST_OF_INTEGER);
            GPTable["NewLstInt"].LocalSymbols["size"] = new LocalSymbol(0, Type.INTEGER);

            GPTable["NewLstStr"] = new GlobalProcedure(true, Type.LIST_OF_STRING);
            GPTable["NewLstStr"].LocalSymbols["size"] = new LocalSymbol(0, Type.INTEGER);

            GPTable["NewLstBool"] = new GlobalProcedure(true, Type.LIST_OF_BOOLEAN);
            GPTable["NewLstBool"].LocalSymbols["size"] = new LocalSymbol(0, Type.INTEGER);

            GPTable["IntToStr"] = new GlobalProcedure(true, Type.STRING);
            GPTable["IntToStr"].LocalSymbols["i"] = new LocalSymbol(0, Type.INTEGER);

            GPTable["StrToInt"] = new GlobalProcedure(true, Type.INTEGER);
            GPTable["StrToInt"].LocalSymbols["s"] = new LocalSymbol(0, Type.STRING);


            LoopNestingLevel = 0;
        }

        //-----------------------------------------------------------
        public Type Visit(Program node)
        {
            VisitChildren(node, GSTable);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ConstantDeclarationList node, Table table)
        {
            VisitChildren(node, table);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ConstantDeclaration node, Table table)
        {
            GlobalSymbolTable gstable = table as GlobalSymbolTable;
            LocalSymbolTable lstable = table as LocalSymbolTable;

            var symbolName = node.AnchorToken.Lexeme;
            if (table is GlobalSymbolTable)
            {
                if (GSTable.Contains(symbolName))
                    throw new SemanticError("Duplicated symbol: " + symbolName, node[0].AnchorToken);
            }
            else if (table is LocalSymbolTable)
            {
                if (lstable.Contains(symbolName))
                    throw new SemanticError("Duplicated symbol: " + symbolName, node[0].AnchorToken);
            }
            else
            {
                throw new TypeAccessException("Expecting either a GlobalSymbolTable or a LocalSymboltable");
            }

            Type nodeType = Visit((dynamic)node[0], table);
            if (node[0] is Lst)
            {
                if (node[0].Count() == 0)
                    throw new SemanticError("Constant lists cannot be empty: " + symbolName, node.AnchorToken);

                dynamic lst;
                if (nodeType == Type.LIST_OF_BOOLEAN)
                    lst = new Boolean[node[0].Count()];
                else if (nodeType == Type.LIST_OF_INTEGER)
                    lst = new Int32[node[0].Count()];
                else if (nodeType == Type.LIST_OF_STRING)
                    lst = new String[node[0].Count()];
                else
                    throw new TypeAccessException("Expecting one of the following node types: LIST_OF_BOOLEAN, LIST_OF_INTEGER, LIST_OF_STRING");

                int i = 0;
                foreach (var n in node[0])
                    lst[i++] = n.ExtractValue();
                if (table is GlobalSymbolTable)
                    gstable[symbolName] = new GlobalSymbol(nodeType, lst);
                else if (table is LocalSymbolTable)
                    lstable[symbolName] = new LocalSymbol(nodeType, lst);
                else
                    throw new TypeAccessException("Expecting either a GlobalSymbolTable or a LocalSymboltable");
            }
            else
            {
                if (table is GlobalSymbolTable)
                    gstable[symbolName] = new GlobalSymbol(nodeType, node[0].ExtractValue());
                else if (table is LocalSymbolTable)
                    lstable[symbolName] = new LocalSymbol(nodeType, node[0].ExtractValue());
                else
                    throw new TypeAccessException("Expecting either a GlobalSymbolTable or a LocalSymboltable");
            }
            
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(VariableDeclarationList node, Table table)
        {
            VisitChildren(node, table);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(VariableDeclaration node, Table table)
        {
            GlobalSymbolTable gstable = table as GlobalSymbolTable;
            LocalSymbolTable lstable = table as LocalSymbolTable;

            Type declarationType = Visit((dynamic)node[1], table);
            foreach (var n in node[0])
            {
                var symbolName = n.AnchorToken.Lexeme;
                if (table is GlobalSymbolTable)
                {
                    if (GSTable.Contains(symbolName))
                        throw new SemanticError("Duplicated symbol: " + symbolName, n.AnchorToken);
                }
                else if (table is LocalSymbolTable)
                {
                    if (lstable.Contains(symbolName))
                        throw new SemanticError("Duplicated symbol: " + symbolName, n.AnchorToken);
                }
                else
                {
                    throw new TypeAccessException("Expecting either a GlobalSymbolTable or a LocalSymboltable");
                }

                if (table is GlobalSymbolTable)
                    gstable[symbolName] = new GlobalSymbol(declarationType);
                else if (table is LocalSymbolTable)
                    lstable[symbolName] = new LocalSymbol(declarationType);
                else
                    throw new TypeAccessException("Expecting either a GlobalSymbolTable or a LocalSymboltable");
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ProcedureDeclarationList node, Table table)
        {
            VisitChildren(node, table);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ProcedureDeclaration node, Table table)
        {
            GlobalSymbolTable gstable = table as GlobalSymbolTable;
            if (table == null)
                throw new TypeAccessException("Expecting a GlobalSymbolTable");

            var procedureName = node.AnchorToken.Lexeme;
            if (gstable.Contains(procedureName))
                throw new SemanticError("Duplicated procedure: " + procedureName, node.AnchorToken);

            if (node[1] is SimpleType || node[1] is ListType)
            { 
                GPTable[procedureName] = new GlobalProcedure(false, Visit((dynamic)node[1], table));
                var i = 0;
                foreach (var n in node)
                {
                    if (i != 1)
                        Visit((dynamic)n, GPTable[procedureName].LocalSymbols);
                    i++;
                }
            }
            else
            {
                GPTable[procedureName] = new GlobalProcedure(false);
                VisitChildren(node, GPTable[procedureName].LocalSymbols);
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ParameterDeclarationList node, Table table)
        {
            int i = 0;
            foreach (var n in node)
                Visit((dynamic)n, table, ref i);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ParameterDeclaration node, Table table, ref int position)
        {
            LocalSymbolTable lstable = table as LocalSymbolTable;
            if (lstable == null)
                throw new TypeAccessException("Expecting a LocalSymbolTable");

            Type paramType = Visit((dynamic)node[1], table);
            foreach (var n in node[0])
            {
                var symbolName = n.AnchorToken.Lexeme;
                if (lstable.Contains(symbolName))
                    throw new SemanticError("Duplicated symbol: " + symbolName, n.AnchorToken);

                lstable[symbolName] = new LocalSymbol(position++, paramType);
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(StatementList node, Table table)
        {
            VisitChildren(node, table);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(AssignmentStatement node, Table table)
        {
            Type leftExpressionType = Visit((dynamic) node[0], table);
            Type rightExpressionType = Visit((dynamic) node[1], table);
            if (leftExpressionType != rightExpressionType)
                throw new SemanticError("Expecting type " + leftExpressionType + " instead of " + rightExpressionType + " in assignment statement", node.AnchorToken);

            if (node[0] is Identifier)
            {
                GlobalSymbolTable gstable = table as GlobalSymbolTable;
                LocalSymbolTable lstable = table as LocalSymbolTable;

                var symbolName = node[0].AnchorToken.Lexeme;
                if (table is GlobalSymbolTable)
                {
                    if (gstable[symbolName].IsConstant)
                        throw new SemanticError("Cannot perform assignment to constant " + symbolName, node[0].AnchorToken);
                }
                else if (table is LocalSymbolTable)
                {
                    if (lstable.Contains(symbolName))
                    {
                        if (lstable[symbolName].Kind == Clasification.CONST)
                            throw new SemanticError("Cannot perform assignment to constant " + symbolName, node[0].AnchorToken);
                    }
                    else
                    {
                        if (GSTable.Contains(symbolName) && GSTable[symbolName].IsConstant)
                            throw new SemanticError("Cannot perform assignment to constant " + symbolName, node[0].AnchorToken);
                    }
                }
                else
                {
                    throw new TypeAccessException("Expecting either a GlobalSymbolTable or a LocalSymboltable");
                }
            }
            else if (node[0] is ListIndexExpression)  { }
            else { throw new TypeAccessException("Expecting either a Idenetifier or a ListIndexExpression " + node[0]); }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(CallStatement node, Table table)
        {
            var procName = node.AnchorToken.Lexeme;
            if (!GPTable.Contains(procName))
                throw new SemanticError("The procedure " + procName + " is not defined", node.AnchorToken);

            //Check arity and types
            GlobalProcedure gp = GPTable[procName];
            if (gp.ReturnType != Type.VOID)
                throw new SemanticError(procName + " call statement cannot return any value", node.AnchorToken);

            var parameterList = gp.getParameters();

            if (node.Count() != parameterList.Length)
                throw new SemanticError("Incorrect arity. Expecting " + parameterList.Length + " arguments, but found " + node.Count(), node.AnchorToken);

            int i = 0;
            foreach (var parameter in gp.getParameters())
            {
                Type argumType = Visit((dynamic)node[i], table);
                if (argumType != parameter.LocalType)
                    throw new SemanticError("Incorrect argument. Expecting a " + parameter.LocalType + ", but found a " + argumType, node[i].AnchorToken);
                i++;
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(IfStatement node, Table table)
        {
            VisitChildren(node, table);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(IfClause node, Table table)
        {
            Type condType = Visit((dynamic)node[0], table);
            if (condType != Type.BOOLEAN)
                throw new SemanticError("If condition must be evaluated to a boolean value", node[0].AnchorToken);

            for (var i = 1; i < node.Count(); i++)
                Visit((dynamic)node[i], table);

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ElseIfClause node, Table table)
        {
            Type condType = Visit((dynamic)node[0], table);
            if (condType != Type.BOOLEAN)
                throw new SemanticError("If condition must be evaluated to a boolean value", node[0].AnchorToken);

            for (var i = 1; i < node.Count(); i++)
                Visit((dynamic)node[i], table);

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ElseClause node, Table table)
        {
            VisitChildren(node, table);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(LoopStatement node, Table table)
        {
            LoopNestingLevel++;
            VisitChildren(node, table);
            LoopNestingLevel--;
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ForStatement node, Table table)
        {
            LoopNestingLevel++;
            
            Type varType = Visit((dynamic)node[0], table);

            var varName = node[0].AnchorToken.Lexeme;
            GlobalSymbolTable gstable = table as GlobalSymbolTable;
            LocalSymbolTable lstable = table as LocalSymbolTable;
            if (table is GlobalSymbolTable)
            {
                if (gstable[varName].IsConstant)
                    throw new SemanticError("Loop variable " + varName + " cannot be a constant", node[0].AnchorToken);
            }
            else
            {
                if (lstable.Contains(varName))
                {
                    if (lstable[varName].Kind != Clasification.VAR)
                        throw new SemanticError("Loop variable " + varName + " cannot be a constant or parameter", node[0].AnchorToken);
                }
                else
                {
                    if (GSTable[varName].IsConstant)
                        throw new SemanticError("Loop variable " + varName + " cannot be a constant", node[0].AnchorToken);
                }
            }

            Type iterableType = Visit((dynamic)node[1], table);
            if (iterableType == Type.LIST_OF_BOOLEAN)
            {
                if (varType != Type.BOOLEAN)
                    throw new SemanticError("Incorrect loop variable \"" + varName + "\". Expecting " + Type.BOOLEAN + ", but found " + varType, node[0].AnchorToken);
            }
            else if (iterableType == Type.LIST_OF_INTEGER)
            {
                if (varType != Type.INTEGER)
                    throw new SemanticError("Incorrect loop variable \"" + varName + "\". Expecting " + Type.INTEGER + ", but found " + varType, node[0].AnchorToken);
            }
            else if (iterableType == Type.LIST_OF_STRING)
            {
                if (varType != Type.STRING)
                    throw new SemanticError("Incorrect loop variable \"" + varName + "\". Expecting " + Type.STRING + ", but found " + varType, node[0].AnchorToken);
            }
            else
            {
                throw new SemanticError("Loop can only iterate over list types, but found " + iterableType, node[1].AnchorToken);
            }

            for (var i = 2; i < node.Count(); i++)
                Visit((dynamic)node[i], table);

            LoopNestingLevel--;
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ReturnStatement node, Table table)
        {
            if (table is GlobalSymbolTable)
            {
                if (node.Count() != 0)
                    throw new SemanticError("Return statement cannot return values in main section", node.AnchorToken);
                return Type.VOID;
            }
            else
            {
                Type returnType = Visit((dynamic)node[0], table);
                //Search for the procedure in which the return statement was called and compare return types
                foreach (KeyValuePair<string, GlobalProcedure> entry in GPTable)
                {
                    var proc = entry.Value;
                    if (proc.LocalSymbols == table)
                    {
                        if (proc.ReturnType != returnType)
                            throw new SemanticError("Incorrect return statement. Expecting a " + proc.ReturnType + ", but found a " + returnType, node.AnchorToken);
                        break;
                    }
                }

                return returnType;
            }
        }

        //-----------------------------------------------------------
        public Type Visit(ExitStatement node, Table table)
        {
            if (LoopNestingLevel == 0)
                throw new SemanticError("Exit statement must be inside a loop or for statement", node.AnchorToken);
                
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(Call node, Table table)
        {
            var procName = node.AnchorToken.Lexeme;
            if (!GPTable.Contains(procName))
                throw new SemanticError("The procedure " + procName + " is not defined", node.AnchorToken);
                
            GlobalProcedure gp = GPTable[procName];

            var parameterList = gp.getParameters();
            if (node.Count() != parameterList.Length)
                throw new SemanticError("Incorrect arity. Expecting " + parameterList.Length + " arguments, but found " + node.Count(), node.AnchorToken);

            int i = 0;
            foreach (var parameter in gp.getParameters())
            {
                Type argumType = Visit((dynamic)node[i], table);
                if (argumType != parameter.LocalType)
                    throw new SemanticError("Incorrect argument. Expecting a " + parameter.LocalType + ", but found a " + argumType, node[i].AnchorToken);
                i++;
            }

            return gp.ReturnType;
        }

        //-----------------------------------------------------------
        public Type Visit(ListType node, Table table)
        {
            var tokcat = node.AnchorToken.Category;
            if (tokcat == TokenCategory.BOOLEAN)
                return Type.LIST_OF_BOOLEAN;
            if (tokcat == TokenCategory.INTEGER)
                return Type.LIST_OF_INTEGER;
            if (tokcat == TokenCategory.STRING)
                return Type.LIST_OF_STRING;
            throw new TypeAccessException("Expecting one of the following node types: BOOLEAN, INTEGER, STRING");
        }

        //-----------------------------------------------------------
        public Type Visit(SimpleType node, Table table)
        {
            var tokcat = node.AnchorToken.Category;
            if (tokcat == TokenCategory.BOOLEAN)
                return Type.BOOLEAN;
            if (tokcat == TokenCategory.INTEGER)
                return Type.INTEGER;
            if (tokcat == TokenCategory.STRING)
                return Type.STRING;
            throw new TypeAccessException("Expecting one of the following node types: BOOLEAN, INTEGER, STRING");
        }

        //-----------------------------------------------------------
        public Type Visit(Identifier node, Table table)
        {
            GlobalSymbolTable gstable = table as GlobalSymbolTable;
            LocalSymbolTable lstable = table as LocalSymbolTable;
           
            var symbolName = node.AnchorToken.Lexeme;
            if (table is GlobalSymbolTable)
            {
                if (gstable.Contains(symbolName))
                    return gstable[symbolName].TheType;
                throw new SemanticError("Undeclared variable: " + symbolName, node.AnchorToken);
            }
            else if (table is LocalSymbolTable)
            {
                if (lstable.Contains(symbolName))
                    return lstable[symbolName].LocalType;
                if (GSTable.Contains(symbolName))
                    return GSTable[symbolName].TheType;
                throw new SemanticError("Undeclared variable: " + symbolName, node.AnchorToken);
            }
            else
            {
                throw new TypeAccessException("Expecting either a GlobalSymbolTable or a LocalSymboltable");
            }
        }

        //-----------------------------------------------------------
        public Type Visit(ListIndexExpression node, Table table)
        {
            Type index = Visit((dynamic)node[1], table);
            if (index != Type.INTEGER)
                throw new SemanticError("Expecting an integer type instead of " + index, node[1].AnchorToken);

            Type listType = Visit((dynamic)node[0], table);
            if (listType == Type.LIST_OF_BOOLEAN)
                return Type.BOOLEAN;
            if (listType == Type.LIST_OF_INTEGER)
                return Type.INTEGER;
            if (listType == Type.LIST_OF_STRING)
                return Type.STRING;
            throw new SemanticError("Expecting a list type instead of " + listType, node[0].AnchorToken);
        }

        //-----------------------------------------------------------
        public Type Visit(Lst node, Table table)
        {
            if (node.Count() > 0)
            {
                Type lstType = Visit((dynamic)node[0], table);
                foreach (var n in node)
                {
                    Type elmType = Visit((dynamic)n, table);
                    if (elmType != lstType)
                        throw new SemanticError("All list elements should be of the same type: ", n.AnchorToken);
                }

                if (lstType == Type.BOOLEAN)
                    return Type.LIST_OF_BOOLEAN;
                if (lstType == Type.INTEGER)
                    return Type.LIST_OF_INTEGER;
                if (lstType == Type.STRING)
                    return Type.LIST_OF_STRING;
                throw new TypeAccessException("Expecting one of the following node types: BOOLEAN, INTEGER, STRING");
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(True node, Table table)
        {
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(False node, Table table)
        {
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(IntegerLiteral node, Table table)
        {
            var intStr = node.AnchorToken.Lexeme;

            try
            {
                Convert.ToInt32(intStr);
            }
            catch (OverflowException)
            {
                throw new SemanticError("Integer literal too large: " + intStr, node.AnchorToken);
            }

            return Type.INTEGER;
        }

        //-----------------------------------------------------------
        public Type Visit(StringLiteral node, Table table)
        {
            return Type.STRING;
        }

        //-----------------------------------------------------------
        public Type Visit(And node, Table table)
        {
            VisitBinaryOperator("and", node, Type.BOOLEAN, table);
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(Xor node, Table table)
        {
            VisitBinaryOperator("xor", node, Type.BOOLEAN, table);
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(Or node, Table table)
        {
            VisitBinaryOperator("or", node, Type.BOOLEAN, table);
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(Equal node, Table table)
        {
            VisitBinaryOperatorEquality("=", node, Type.BOOLEAN, Type.INTEGER, table);
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(NotEqual node, Table table)
        {
            VisitBinaryOperatorEquality("<>", node, Type.BOOLEAN, Type.INTEGER, table);
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(Smaller node, Table table)
        {
            VisitBinaryOperator("<", node, Type.INTEGER, table);
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(Greater node, Table table)
        {
            VisitBinaryOperator(">", node, Type.INTEGER, table);
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(SmallerEq node, Table table)
        {
            VisitBinaryOperator("<=", node, Type.INTEGER, table);
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(GreaterEq node, Table table)
        {
            VisitBinaryOperator(">=", node, Type.INTEGER, table);
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(Plus node, Table table)
        {
            VisitBinaryOperator("+", node, Type.INTEGER, table);
            return Type.INTEGER;
        }

        //-----------------------------------------------------------
        public Type Visit(Minus node, Table table)
        {
            VisitBinaryOperator("-", node, Type.INTEGER, table);
            return Type.INTEGER;
        }

        //-----------------------------------------------------------
        public Type Visit(Mul node, Table table)
        {
            VisitBinaryOperator("*", node, Type.INTEGER, table);
            return Type.INTEGER;
        }

        //-----------------------------------------------------------
        public Type Visit(Div node, Table table)
        {
            VisitBinaryOperator("div", node, Type.INTEGER, table);
            return Type.INTEGER;
        }

        //-----------------------------------------------------------
        public Type Visit(Rem node, Table table)
        {
            VisitBinaryOperator("rem", node, Type.INTEGER, table);
            return Type.INTEGER;
        }

        //-----------------------------------------------------------
        public Type Visit(Not node, Table table)
        {
            VisitBinaryOperatorNegation("not", node, Type.BOOLEAN, table);
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(Negative node, Table table)
        {
            VisitBinaryOperatorNegation("-", node, Type.INTEGER, table);
            return Type.INTEGER;
        }

        //-----------------------------------------------------------
        private void VisitBinaryOperator(String op, Node node, Type type, Table table)
        {
            if (Visit((dynamic)node[0], table) != type || Visit((dynamic)node[1], table) != type)
            {
                throw new SemanticError(
                    String.Format("Operator {0} requires two operands of type {1}", op, type),
                    node.AnchorToken);
            }
        }

        //-----------------------------------------------------------
        private void VisitBinaryOperatorEquality(String op, Node node, Type typeA, Type typeB, Table table)
        {
            if (Visit((dynamic)node[0], table) == typeA)
            {
                VisitBinaryOperator(op, node, typeA, table);
            }
            else if (Visit((dynamic)node[0], table) == typeB)
            {
                VisitBinaryOperator(op, node, typeB, table);
            }
            else
            {
                throw new SemanticError(
                    String.Format("Operator {0} requires two operands of type {1} or {2}", op, typeA, typeB),
                    node.AnchorToken);
            }
        }

        //-----------------------------------------------------------
        private void VisitBinaryOperatorNegation(String op, Node node, Type type, Table table)
        {
            if (Visit((dynamic)node[0], table) != type)
            {
                throw new SemanticError(
                    String.Format( "Operator {0} requires an operand of type {1}", op, type),
                    node.AnchorToken);
            }
        }

        //-----------------------------------------------------------
        private Type Visit(Node node, Table table)
        {
            Console.WriteLine(node + " visit not implemented yet");
            return Type.VOID;
        }

        //-----------------------------------------------------------
        private void VisitChildren(Node node, Table table)
        {
            foreach (var n in node)
            {
                Visit((dynamic)n, table);
            }
        }

        //-----------------------------------------------------------
    }
}
