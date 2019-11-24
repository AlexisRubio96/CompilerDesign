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
                /*
                { TokenCategory.BOOL, Type.BOOL },
                { TokenCategory.INT, Type.INT }
                */               
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
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ConstantDeclarationList node)
        {
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ConstantDeclaration node)
        {
            var globalSymbolName = node.AnchorToken.Lexeme;
            if (GSTable.Contains(globalSymbolName))
                throw new SemanticError("Duplicated symbol: " + globalSymbolName, node[0].AnchorToken);

            Type nodeType = Visit((dynamic)node[0]);
            if (node[0] is Lst)
            {
                if (node[0].Count() == 0)
                    throw new SemanticError("Constant lists cannot be empty: " + globalSymbolName, node.AnchorToken);

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
                GSTable[globalSymbolName] = new GlobalSymbol(nodeType, lst);
            }
            else
            {
                GSTable[globalSymbolName] = new GlobalSymbol(nodeType, node[0].ExtractValue());
            }
            
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(VariableDeclarationList node)
        {
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(VariableDeclaration node)
        {
            Type declarationType = Visit((dynamic)node[1]);
            foreach (var n in node[0])
            {
                var globalSymbolName = n.AnchorToken.Lexeme;
                if (GSTable.Contains(globalSymbolName))
                {
                    throw new SemanticError("Duplicated symbol: " + globalSymbolName, n.AnchorToken);
                }
                GSTable[globalSymbolName] = new GlobalSymbol(declarationType);
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ProcedureDeclarationList node)
        {
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(StatementList node)
        {
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(AssignmentStatement node)
        {
            Type leftExpressionType = Visit((dynamic) node[0]);
            Type rightExpressionType = Visit((dynamic) node[1]);
            if (leftExpressionType != rightExpressionType)
                throw new SemanticError("Expecting type " + leftExpressionType + " instead of " + rightExpressionType + " in assignment statement" +
                	"", node.AnchorToken);

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ListType node)
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
        public Type Visit(SimpleType node)
        {
            var tokcat = node.AnchorToken.Category;
            if (tokcat == TokenCategory.BOOLEAN)
                return Type.BOOLEAN;
            else if (tokcat == TokenCategory.INTEGER)
                return Type.INTEGER;
            if (tokcat == TokenCategory.STRING)
                return Type.STRING;
            throw new TypeAccessException("Expecting one of the following node types: BOOLEAN, INTEGER, STRING");
        }

        //-----------------------------------------------------------
        public Type Visit(Lst node)
        {
            if (node.Count() > 0)
            {
                Type lstType = Visit((dynamic)node[0]);
                foreach (var n in node)
                {
                    Type elmType = Visit((dynamic)n);
                    if (elmType != lstType)
                        throw new SemanticError("All list elements should be of the same type: ", n.AnchorToken);
                }

                if (lstType == Type.BOOLEAN)
                    return Type.LIST_OF_BOOLEAN;
                else if (lstType == Type.INTEGER)
                    return Type.LIST_OF_INTEGER;
                else if (lstType == Type.STRING)
                    return Type.LIST_OF_STRING;
                else
                    throw new TypeAccessException("Expecting one of the following node types: BOOLEAN, INTEGER, STRING");
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(True node)
        {
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(False node)
        {
            return Type.BOOLEAN;
        }

        //-----------------------------------------------------------
        public Type Visit(IntegerLiteral node)
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
        public Type Visit(StringLiteral node)
        {
            return Type.STRING;
        }

        //-----------------------------------------------------------
        private Type Visit(Node node)
        {
            Console.WriteLine(node + " visit not implemented yet");
            return Type.VOID;
        }

        //-----------------------------------------------------------
        private void VisitChildren(Node node)
        {
            foreach (var n in node)
            {
                Visit((dynamic)n);
            }
        }

        //-----------------------------------------------------------
    }
}
