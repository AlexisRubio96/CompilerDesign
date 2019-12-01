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
        int labelCounter = 0;

        //-----------------------------------------------------------
        string GenerateLabel() {
            return String.Format("${0:000000}", labelCounter++);
        }

        //-----------------------------------------------------------    
        static readonly IDictionary<Type, string> CILTypes =
            new Dictionary<Type, string>() {
                { Type.VOID, "void" },
                { Type.BOOLEAN, "bool"},
                { Type.INTEGER, "int32"},          
                { Type.STRING, "string"}
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
                + ".assembly extern 'chimeralib' {}\n\n"
                + ".class public 'ChimeraProgram' extends "
                + "['mscorlib']'System'.'Object' {\n"
                + "\t.method public static void 'main'() {\n"
                + "\t\t.entrypoint\n"
                + VisitChildren((dynamic)node, GSTable)
                +"\t\tret\n"
                + "\t}\n"
                + "}\n";
        }


        //-----------------------------------------------------------
        private string Visit(Node node, Table table)
        {
            Console.WriteLine(node + " visit not implemented yet for code generation");
            return "";
        }
       
        //-----------------------------------------------------------
        private string Visit(StatementList node, Table table)
        {
            return VisitChildren(node, table);
        }

        //-----------------------------------------------------------
        private string Visit(CallStatement node, Table table)
        {
            var retString = VisitChildren(node, table);

            var procName = node.AnchorToken.Lexeme;
            var proc = GPTable[procName];
            var procParamsTypes = "";
            var procParams = proc.getParameters();

            if (procParams.Length > 0) {
                procParamsTypes = CILTypes[procParams[0].LocalType];
                for (var i = 1; i < procParams.Length; i++)
                {
                    procParamsTypes += "," + CILTypes[procParams[i].LocalType];
                }
            }

            if (proc.IsPredefined)
            {
                retString += "\t\tcall void class ['chimeralib']'Chimera'.'Utils'::'" + procName + "'(" + procParamsTypes + ")\n";
            }
            else
            {
                Console.WriteLine("Not implemented yet");
            }

            return retString;
        }

        //-----------------------------------------------------------
        public string Visit(True node, Table table)
        {
            return "\t\tldc.i4.1\n";
        }

        //-----------------------------------------------------------
        public string Visit(False node, Table table)
        {
            return "\t\tldc.i4.0\n";
        }

        //-----------------------------------------------------------
        public string Visit(IntegerLiteral node, Table table)
        {
            var intValue = Convert.ToInt32(node.AnchorToken.Lexeme);
            return "\t\tldc.i4 " + intValue + "\n";
        }

        //-----------------------------------------------------------
        private string Visit(StringLiteral node, Table table)
        {
            return "\t\tldstr \""+ node.AnchorToken.Lexeme.Replace("\"", "\\\"") + "\"\n";
        }

        //-----------------------------------------------------------
        public string Visit(Plus node, Table table)
        {
            return VisitBinaryOperator("add.ovf", node, table);
        }

        //-----------------------------------------------------------
        public string Visit(Mul node, Table table)
        {
            return VisitBinaryOperator("mul.ovf", node, table);
        }

        //-----------------------------------------------------------
        public string Visit(Minus node, Table table)
        {
            return VisitBinaryOperator("sub.ovf", node, table);
        }

        //-----------------------------------------------------------
        public string Visit(Div node, Table table)
        {
            return VisitBinaryOperator("div", node, table);
        }
           //-----------------------------------------------------------
        public string Visit(Rem node, Table table)
        {
            return VisitBinaryOperator("rem", node, table);
        }
          public string Visit(Negative node, Table table)
        {
            return Visit((dynamic)node[0],table)+"\n\t\tneg\n";
        }

        //-----------------------------------------------------------
        public string Visit(And node, Table table)
        {
            string label = GenerateLabel();
            return String.Format("{0}\t\tdup\n\t\tbrfalse {1}\n{2}\t\tand\n\t\t{1}:\n",
                Visit((dynamic)node[0], table),
                label,
                Visit((dynamic)node[1], table));
        }

        //-----------------------------------------------------------
        public string Visit(Or node, Table table)
        {
            string label = GenerateLabel();
            return String.Format("{0}\t\tdup\n\t\tbrtrue {1}\n{2}\t\tor\n\t\t{1}:\n",
                Visit((dynamic)node[0], table),
                label,
                Visit((dynamic)node[1], table));
        }

        //-----------------------------------------------------------
        public string Visit(Xor node, Table table)
        {
            return VisitBinaryOperator("xor", node, table);
        }
          //-----------------------------------------------------------
        public string Visit(Not node, Table table)
        {
            return String.Format(
            "{0}"
            +"\t\tldc.i4.0\n"
            +"\t\tceq\n",
                Visit((dynamic)node[0], table));
        }


        //-----------------------------------------------------------
        public string Visit(Equal node, Table table)
        {
            return VisitBinaryOperator("ceq", node, table);
        }
         //-----------------------------------------------------------
         public string Visit(NotEqual node, Table table)
        {
            return String.Format(
            "{0}{1}"
            +"\t\tceq\n"
            +"\t\tldc.i4.0\n"
            +"\t\tceq\n",
                Visit((dynamic)node[0], table),
                Visit((dynamic)node[1], table));
        }
        //-----------------------------------------------------------
        public string Visit(Smaller node, Table table)
        {
            return VisitBinaryOperator("clt", node, table);
        }

        //-----------------------------------------------------------
          public string Visit(Greater node, Table table)
        {
            return VisitBinaryOperator("cgt", node, table);
        }
          //-----------------------------------------------------------
        public string Visit(GreaterEq node, Table table)
        {
            string label = GenerateLabel();
            return String.Format(
           
           "ldc.i4.1\n"
            +"{1}{2}"
            +"\t\tbge {0}\n"
            +"\t\tpop\n"
            +"\t\tldc.i4.0\n"
            +"\t\t{0}:\n",
                label,
                Visit((dynamic)node[0], table),
                Visit((dynamic)node[1], table));
        }
           //-----------------------------------------------------------
          public string Visit(SmallerEq node, Table table)
        {
            string label = GenerateLabel();
            return String.Format(
           
           "ldc.i4.1\n"
            +"{1}{2}"
            +"\t\tble {0}\n"
            +"\t\tpop\n"
            +"\t\tldc.i4.0\n"
            +"\t\t{0}:\n",
                label,
                Visit((dynamic)node[0], table),
                Visit((dynamic)node[1], table));
        }

        //-----------------------------------------------------------
        public string Visit(IfStatement node, Table table)
        {

            var label = GenerateLabel();

            return String.Format(
                "{1}\t\tbrfalse '{0}'\n{2}\t'{0}':\n",
                label,
                Visit((dynamic)node[0], table),
                Visit((dynamic)node[1], table)
            );
        }
        

        private string VisitChildren(Node node, Table table)
        {
            var sb = new StringBuilder();
            foreach (var n in node)
            {
                sb.Append(Visit((dynamic)n, table));
            }
            return sb.ToString();
        }


        private string VisitBinaryOperator(string op, Node node, Table table)
        {
            return Visit((dynamic)node[0], table)
                + Visit((dynamic)node[1], table)
                + "\t\t"
                + op
                + "\n";
        }
    }
}
