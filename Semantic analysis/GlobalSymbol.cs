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

    public class GlobalSymbol
    {

        Boolean IsConstant;
        Type TheType;
        dynamic Value;

        //-----------------------------------------------------------
        public GlobalSymbol(Boolean IsConstant, Type TheType, dynamic Value)
        {
            this.IsConstant = IsConstant;
            this.TheType = TheType;
            this.Value = Value;

        }

        //-----------------------------------------------------------
        public GlobalSymbol(Boolean IsConstant, Type TheType)
        {
            this.IsConstant = IsConstant;
            this.TheType = TheType;

            if (TheType == Type.BOOLEAN)
            {
                this.Value = false;
            }
            else if (TheType == Type.INTEGER)
            {
                this.Value = 0;
            }
            else if (TheType == Type.STRING)
            {
                this.Value = "";
            }
            else if (TheType == Type.LIST_OF_BOOLEAN)
            {
                this.Value = new Boolean[0];
            }
            else if (TheType == Type.LIST_OF_INTEGER)
            {
                this.Value = new Int32[0];
            }
            else if (TheType == Type.LIST_OF_STRING)
            {
                this.Value = new String[0];
            }
            else
            {
                throw new ArgumentException("Not a valid type");
            }
        }


        //-----------------------------------------------------------
        public override string ToString()
        {
            if (Value.GetType().IsArray)
            {
                return "IsConstant=" + IsConstant + " Type=" + TheType + " Values=[" + string.Join(", ", Value) + "]";
            }
            return "IsConstant=" + IsConstant + " Type=" + TheType + " Value=" + Value;

        }
    }
}
