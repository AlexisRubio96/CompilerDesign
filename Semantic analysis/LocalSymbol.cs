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

    public class LocalSymbol
    {
        public Type LocalType
        {
            get;
            private set;
        }
        int Position;
        dynamic Value;
        public Clasification Kind
        {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public LocalSymbol(Type LocalType, dynamic Value)
        {
            this.LocalType = LocalType;
            this.Kind = Clasification.CONST;
            this.Value = Value;
        }

        //-----------------------------------------------------------
        public LocalSymbol(Type LocalType)
        {
            this.LocalType = LocalType;
            this.Kind = Clasification.VAR;
            setDefaultValues(LocalType);
        }

        //-----------------------------------------------------------
        public LocalSymbol(Type LocalType, int Position)
        {
            this.Kind = Clasification.PARAM;
            this.LocalType = LocalType;
            this.Position = Position;
            setDefaultValues(LocalType);
        }

        //-----------------------------------------------------------
        public void setDefaultValues(Type LocalType)
        {
            if (LocalType == Type.BOOLEAN)
                this.Value = false;
            else if (LocalType == Type.INTEGER)
                this.Value = 0;
            else if (LocalType == Type.STRING)
                this.Value = "";
            else if (LocalType == Type.LIST_OF_BOOLEAN)
                this.Value = new Boolean[0];
            else if (LocalType == Type.LIST_OF_INTEGER)
                this.Value = new Int32[0];
            else if (LocalType == Type.LIST_OF_STRING)
                this.Value = new String[0];
            else
                throw new ArgumentException("Not a valid type");
        }

        //-----------------------------------------------------------
        public override string ToString()
        {
            if (LocalType == Type.LIST_OF_BOOLEAN || LocalType == Type.LIST_OF_INTEGER || LocalType == Type.LIST_OF_STRING)
                return "Type=" + LocalType + " Position=" + Position + " Kind=" + Kind + " Value=[" + string.Join(", ", Value) + "]";
            return "Type=" + LocalType + " Position=" + Position + " Kind=" + Kind + " Value=" + Value;

        }
    }

}