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
        LocalSymbolTable localSymbols;

        //-----------------------------------------------------------
        public GlobalProcedure(Boolean IsPredefined, Type ReturnType)
        {
            this.IsPredefined = IsPredefined;
            this.ReturnType = ReturnType;
        }

        //-----------------------------------------------------------
        public GlobalProcedure(Boolean IsPredefined)
        {
            this.IsPredefined = IsPredefined;
            this.ReturnType = Type.VOID;
        }

        //-----------------------------------------------------------
        public override string ToString()
        {
            return "haz global procedure.ToString!!!!";
        }
    }
}

