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
        public SymbolTable Table
        {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public SemanticAnalyzer()
        {
            Table = new SymbolTable();
        }

        //-----------------------------------------------------------
        public Type Visit(Program node)
        {
            foreach (var child in node)
            {
                Console.WriteLine("children");
            }
            return Type.VOID;
        }

        //-----------------------------------------------------------
    }
}
