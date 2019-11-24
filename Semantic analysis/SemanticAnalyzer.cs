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
        static readonly IDictionary<TokenCategory, Type> typeMapper =
            new Dictionary<TokenCategory, Type>() {
                { TokenCategory.BOOLEAN, Type.BOOLEAN },
                { TokenCategory.INTEGER, Type.INTEGER },
                { TokenCategory.STRING, Type.STRING },
            };

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
                Visit((dynamic)n, table, i++);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ParameterDeclaration node, Table table, int position)
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

                lstable[symbolName] = new LocalSymbol(paramType, position);
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
                throw new SemanticError("Expecting type " + leftExpressionType + " instead of " + rightExpressionType + " in assignment statement" +
                	"", node.AnchorToken);


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
                    if (lstable.Contains(symbolName) && lstable[symbolName].Kind == Clasification.CONST || GSTable.Contains(symbolName) && GSTable[symbolName].IsConstant)
                        throw new SemanticError("Cannot perform assignment to constant " + symbolName, node[0].AnchorToken);
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
            GlobalSymbolTable gstable = table as GlobalSymbolTable;
            LocalSymbolTable lstable = table as LocalSymbolTable;

            Type index = Visit((dynamic)node[1], table);
            if (index != Type.INTEGER)
                throw new SemanticError("Expecting an integer type instead of " + index, node[1].AnchorToken);

            var listName = node[0].AnchorToken.Lexeme;
            if (table is GlobalSymbolTable)
            {
                if (gstable.Contains(listName))
                {
                    if (gstable[listName].TheType == Type.LIST_OF_BOOLEAN)
                        return Type.BOOLEAN;
                    if (gstable[listName].TheType == Type.LIST_OF_INTEGER)
                        return Type.INTEGER;
                    if (gstable[listName].TheType == Type.LIST_OF_STRING)
                        return Type.STRING;
                    throw new SemanticError("Expecting a list type instead of " + gstable[listName].TheType, node[0].AnchorToken);
                }
                throw new SemanticError("Undeclared variable: " + listName, node[0].AnchorToken);
            }
            else if (table is LocalSymbolTable)
            {
                if (lstable.Contains(listName))
                {
                    if (lstable[listName].LocalType == Type.LIST_OF_BOOLEAN)
                        return Type.BOOLEAN;
                    if (lstable[listName].LocalType == Type.LIST_OF_INTEGER)
                        return Type.INTEGER;
                    if (lstable[listName].LocalType == Type.LIST_OF_STRING)
                        return Type.STRING;
                    throw new SemanticError("Expecting a list type instead of " + lstable[listName].LocalType, node[0].AnchorToken);
                }
                if (GSTable.Contains(listName))
                {
                    if (GSTable[listName].TheType == Type.LIST_OF_BOOLEAN)
                        return Type.BOOLEAN;
                    if (GSTable[listName].TheType == Type.LIST_OF_INTEGER)
                        return Type.INTEGER;
                    if (GSTable[listName].TheType == Type.LIST_OF_STRING)
                        return Type.STRING;
                    throw new SemanticError("Expecting a list type instead of " + GSTable[listName].TheType, node[0].AnchorToken);
                }
                throw new SemanticError("Undeclared variable: " + listName, node[0].AnchorToken);
            }
            else
            {
                throw new TypeAccessException("Expecting either a GlobalSymbolTable or a LocalSymboltable");
            }
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
