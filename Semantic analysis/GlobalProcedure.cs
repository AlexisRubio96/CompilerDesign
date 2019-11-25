/*
 * A01373670 - Rodrigo Garcia Lopez
 * A01372074 - Jorge Alexis Rubio Sumano
 * A01371084 - Valentin Ochoa Lopez
*/

using System;
using System.Text;
using System.Collections.Generic;

namespace Chimera
{
    //
    public class GlobalProcedure
    {

        Boolean IsPredefined;
        Type ReturnType;
        public LocalSymbolTable LocalSymbols
        {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public GlobalProcedure(Boolean IsPredefined, Type ReturnType)
        {
            this.IsPredefined = IsPredefined;
            this.ReturnType = ReturnType;
            this.LocalSymbols = new LocalSymbolTable();
        }

        //-----------------------------------------------------------
        public GlobalProcedure(Boolean IsPredefined)
        {
            this.IsPredefined = IsPredefined;
            this.ReturnType = Type.VOID;
            this.LocalSymbols = new LocalSymbolTable();
        }

        //-----------------------------------------------------------
        public LocalSymbol[] getParameters()
        {
            List<LocalSymbol> parameters = new List<LocalSymbol>();
            foreach (var ls in LocalSymbols)
            {
                if (ls.Value.Kind == Clasification.PARAM)
                    parameters.Add(ls.Value);
            }

            parameters.Sort((a, b) => a.Position.CompareTo(b.Position));

            return parameters.ToArray();
        }

        //-----------------------------------------------------------
        public override string ToString()
        {
            return "IsPredifined=" + IsPredefined + " ReturnType=" + ReturnType + LocalSymbols ;
        }
    }
}

