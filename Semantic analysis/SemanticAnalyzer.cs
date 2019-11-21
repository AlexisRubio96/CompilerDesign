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
        public SemanticAnalyzer()
        {
            GSTable = new GlobalSymbolTable();
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
            {
                throw new SemanticError("Duplicated symbol: " + globalSymbolName, node[0].AnchorToken);
            }

            if (node[0] is Lst)
            {
                if (node[0].Count() == 0)
                {
                    throw new SemanticError("Constant lists cannot be empty: " + globalSymbolName, node.AnchorToken);
                }

                Type nodeType = Visit((dynamic)node[0]);
                if (nodeType == Type.LIST_OF_BOOLEAN)
                {
                    Boolean[] lst = new Boolean[node[0].Count()];
                    int i = 0;
                    foreach (var n in node[0])
                    {
                        lst[i++] = Convert.ToBoolean(n.AnchorToken.Lexeme);
                    }
                    GSTable[globalSymbolName] = new GlobalSymbol(true, nodeType, lst);
                }
                else if (nodeType == Type.LIST_OF_INTEGER)
                {
                    Int32[] lst = new Int32[node[0].Count()];
                    int i = 0;
                    foreach (var n in node[0])
                    {
                        lst[i++] = Convert.ToInt32(n.AnchorToken.Lexeme);
                    }
                    GSTable[globalSymbolName] = new GlobalSymbol(true, nodeType, lst);
                }
                else if (nodeType == Type.LIST_OF_STRING)
                {
                    String[] lst = new String[node[0].Count()];
                    int i = 0;
                    foreach (var n in node[0])
                    {
                        lst[i++] = n.AnchorToken.Lexeme;
                    }
                    GSTable[globalSymbolName] = new GlobalSymbol(true, nodeType, lst);
                }

            }
            else
            {
                Type nodeType = Visit((dynamic)node[0]);
                if (nodeType == Type.BOOLEAN)
                {
                    GSTable[globalSymbolName] = new GlobalSymbol(true, nodeType, Convert.ToBoolean(node[0].AnchorToken.Lexeme));
                }
                else if (nodeType == Type.INTEGER)
                {
                    GSTable[globalSymbolName] = new GlobalSymbol(true, nodeType, Convert.ToInt32(node[0].AnchorToken.Lexeme));
                }
                else if (nodeType == Type.STRING)
                {
                    GSTable[globalSymbolName] = new GlobalSymbol(true, nodeType, node[0].AnchorToken.Lexeme);
                }
         
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
                GSTable[globalSymbolName] = new GlobalSymbol(false, declarationType);
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(ListType node)
        {
            var tokcat = node.AnchorToken.Category;
            if (tokcat == TokenCategory.BOOLEAN)
            {
                return Type.LIST_OF_BOOLEAN;
            }
            else if (tokcat == TokenCategory.INTEGER)
            {
                return Type.LIST_OF_INTEGER;
            }
            else
            {
                return Type.LIST_OF_STRING;
            }
        }

        //-----------------------------------------------------------
        public Type Visit(SimpleType node)
        {
            var tokcat = node.AnchorToken.Category;
            if (tokcat == TokenCategory.BOOLEAN)
            {
                return Type.BOOLEAN;
            }
            else if (tokcat == TokenCategory.INTEGER)
            {
                return Type.INTEGER;
            }
            else
            {
                return Type.STRING;
            }
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
                    {
                        throw new SemanticError("All list elements should be of the same type: ", n.AnchorToken);
                    }
                }

                if (lstType == Type.BOOLEAN)
                {
                    return Type.LIST_OF_BOOLEAN;
                }
                else if (lstType == Type.INTEGER)
                {
                    return Type.LIST_OF_INTEGER;
                }
                else if (lstType == Type.STRING)
                {
                    return Type.LIST_OF_STRING;
                }
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
