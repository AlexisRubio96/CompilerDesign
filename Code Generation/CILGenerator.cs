/*
 * A01373670 - Rodrigo Garcia Lopez
 * A01372074 - Jorge Alexis Rubio Sumano
 * A01371084 - Valentin Ochoa Lopez
*/

using System;
using System.Text;
using System.Collections.Generic;

namespace Chimera {

    class CILGenerator {

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
        public CILGenerator(GlobalSymbolTable gsTable, GlobalProcedureTable gpTable)
        {
            GSTable = gsTable;
            GPTable = gpTable;
        }

        //-----------------------------------------------------------
        public string Visit(Program node)
        {
            return "// Code generated by the chimera compiler.\n\n"
                + ".assembly 'chimera' {}\n\n"
                + ".assembly extern 'chimera' {}\n\n"
                + ".class public 'ChimeraProgram' extends "
                + "['mscorlib']'System'.'Object' {\n"
                + "\t.method public static void 'start'() {\n"
                + "\t\t.entrypoint\n"
                + "\t\tret\n"
                + "\t}\n"
                + "}\n";
        }

        //-----------------------------------------------------------
        private string Visit(Node node, Table table)
        {
            Console.WriteLine(node + " visit not implemented yet");
            return "";
        }

        //-----------------------------------------------------------
        private string VisitChildren(Node node, Table table)
        {
            var sb = new StringBuilder();
            foreach (var n in node)
            {
                sb.Append(Visit((dynamic)n, table));
            }
            return sb.ToString();
        }
    }
}
