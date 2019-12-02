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
        Stack<string> loopLabels = new Stack<string>();

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
                + "['mscorlib']'System'.'Object' {\n\n"
                + DeclareGlobals() + "\n"
                + "\t.method public static void '.init' () {\n"
                + Visit((dynamic) node[0], GSTable) //Constant declaration list
                + Visit((dynamic) node[1], GSTable) //Variable declaration list
                + "\t\tret\n"
                + "\t}\n\n"
                + Visit((dynamic) node[2], GSTable) //Procedure declaration list
                + "\t.method public static void 'main'() {\n"
                + "\t\t.entrypoint\n"
                + "\t\tcall void class 'ChimeraProgram'::'.init'()\n"
                + Visit((dynamic) node[3], GSTable) //Statement list
                +"\t\tret\n"
                + "\t}\n"
                + "}\n";
        }

        private string DeclareGlobals()
        {
            var sb = new StringBuilder();
            foreach (var gs in GSTable)
            {
                sb.Append("\t.field public static " + CILTypes[gs.Value.TheType] + " '" + gs.Key + "'\n");
            }
            return sb.ToString();
        }

        //-----------------------------------------------------------
        private string Visit(ConstantDeclarationList node, Table table)
        {
            return VisitChildren(node, table);
        }

        //-----------------------------------------------------------
        private string Visit(ConstantDeclaration node, Table table)
        {
            string retString = "";
            if (table is GlobalSymbolTable)
            {
                retString += Visit((dynamic)node[0], table);
                retString += "\t\tstsfld " + CILTypes[GSTable[node.AnchorToken.Lexeme].TheType] + " 'ChimeraProgram'::" + "'" + node.AnchorToken.Lexeme + "'\n";
            }
            else
            {
                LocalSymbolTable lstable = table as LocalSymbolTable;
                var consName = node.AnchorToken.Lexeme;
                retString += "\t\t.locals init (" + CILTypes[lstable[consName].LocalType] + " '" + consName + "')\n";
                retString += Visit((dynamic)node[0], table);
                retString += "\t\tstloc '" + consName + "'\n";
            }

            return retString;
        }

        //-----------------------------------------------------------
        private string Visit(VariableDeclarationList node, Table table)
        {
            return VisitChildren(node, table);
        }

        //-----------------------------------------------------------
        private string Visit(VariableDeclaration node, Table table)
        {
            string retString = "";
            if (table is GlobalSymbolTable)
            {
                foreach (var n in node[0])
                {
                    retString += Visit((dynamic)node[1], table);
                    retString += "\t\tstsfld " + CILTypes[GSTable[n.AnchorToken.Lexeme].TheType] + " 'ChimeraProgram'::" + "'" + n.AnchorToken.Lexeme + "'\n";
                }
            }
            else
            {
                LocalSymbolTable lstable = table as LocalSymbolTable;
                retString += "\t\t.locals init (";
                var strType = CILTypes[lstable[node[0][0].AnchorToken.Lexeme].LocalType];
                retString += strType + " '" + node[0][0].AnchorToken.Lexeme + "'";
                for (int i = 1;  i < node[0].Count(); i++)
                {
                    var n = node[0][i];
                    retString += ", " + strType +" '" + n.AnchorToken.Lexeme + "'";
                }

                return retString + ")\n";
            }
            return retString;
        }

        //-----------------------------------------------------------
        private string Visit(SimpleType node, Table table)
        {
            var tokcat = node.AnchorToken.Category;
            if (tokcat == TokenCategory.BOOLEAN)
                return "\t\tldc.i4.0\n";
            if (tokcat == TokenCategory.INTEGER)
                return "\t\tldc.i4.0\n";
            if (tokcat == TokenCategory.STRING)
                return "\t\tldstr \"" + "\"\n";
            return "!!ERROR!!";
        }

        //-----------------------------------------------------------
        private string Visit(ProcedureDeclarationList node, Table table)
        {
            return VisitChildren(node, table);
        }

        //-----------------------------------------------------------
        private string Visit(ProcedureDeclaration node, Table table)
        {
            string retString = "";
            var procName = node.AnchorToken.Lexeme;
            var proc = GPTable[procName];
            var procParamsNames = proc.getParametersNames();
            var procStrParams = "";

            if (procParamsNames.Length > 0)
            {
                procStrParams += CILTypes[proc.LocalSymbols[procParamsNames[0]].LocalType] + " '" + procParamsNames[0] + "'";
                for (var i = 1; i < procParamsNames.Length; i++)
                {
                    procStrParams += ", " + CILTypes[proc.LocalSymbols[procParamsNames[i]].LocalType] + " '" + procParamsNames[i] + "'";
                }
            }

            var body = "";
            foreach (var n in node)
            {
                // 
                if (n is ParameterDeclarationList || n is SimpleType || n is ListType) 
                    continue;
                body += Visit((dynamic)n, proc.LocalSymbols);
            }

            retString += "\t.method public static " + CILTypes[GPTable[procName].ReturnType] + " '" + procName + "' (" + procStrParams + ") {\n"
                + body
                + "\t\tret\n"
                + "\t}\n\n";
            return retString;
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
        private string Visit(AssignmentStatement node, Table table)
        {
            string retString = "";
            if (table is GlobalSymbolTable)
            {
                retString += Visit((dynamic)node[1], table);
                if (node[0] is Identifier)
                {
                    retString += "\t\tstsfld " + CILTypes[GSTable[node[0].AnchorToken.Lexeme].TheType] + " 'ChimeraProgram'::" + "'" + node[0].AnchorToken.Lexeme + "'\n";
                }

            }
            else
            {
                LocalSymbolTable lstable = table as LocalSymbolTable;
                retString += Visit((dynamic)node[1], table);
                if (node[0] is Identifier)
                {
                    var name = node[0].AnchorToken.Lexeme;
                    if (lstable.Contains(name))
                    {
                        if (lstable[name].Kind == Clasification.PARAM)
                        {
                            retString += "\t\tstarg '" + name + "'\n";
                        }
                        else
                        {
                            retString += "\t\tstloc '" + name + "'\n";
                        }   
                    }
                    else
                    {
                        retString += "\t\tstsfld " + CILTypes[GSTable[name].TheType] + " 'ChimeraProgram'::" + "'" + name + "'\n";
                    }
                }
            }
            return retString;
        }

        //-----------------------------------------------------------
        private string Visit(IfStatement node, Table table)
        {
            string retString = "";
            var lastLabel = GenerateLabel();
            foreach (var n in node)
            {
                var nextClauseLabel = GenerateLabel();
                retString += Visit((dynamic)n, table, nextClauseLabel);
                retString += "\t\tbr " + lastLabel + "\n";
                retString += "\t\t" + nextClauseLabel + ":\n";
            }

            retString += "\t\t" + lastLabel + ":\n";
            return retString;
        }

        //-----------------------------------------------------------
        private string Visit(IfClause node, Table table, string nextClauseLabel)
        {
            string retString = Visit((dynamic)node[0], table);
            retString += "\t\tbrfalse " + nextClauseLabel + "\n";
            for (int i = 1; i < node.Count(); i++)
            {
                retString += Visit((dynamic)node[i], table);
            }

            return retString;
        }

        //-----------------------------------------------------------
        private string Visit(ElseIfClause node, Table table, string nextClauseLabel)
        {
            string retString = Visit((dynamic)node[0], table);
            retString += "\t\tbrfalse " + nextClauseLabel + "\n";
            for (int i = 1; i < node.Count(); i++)
            {
                retString += Visit((dynamic)node[i], table);
            }

            return retString;
        }

        //-----------------------------------------------------------
        private string Visit(ElseClause node, Table table, string nextClauseLabel)
        {
            return VisitChildren(node, table);
        }

        //-----------------------------------------------------------
        private string Visit(LoopStatement node, Table table)
        {
            string retString = "";
            string loopStartLabel = GenerateLabel();
            string loopEndLabel = GenerateLabel();
            loopLabels.Push(loopEndLabel);
            retString += "\t\t" + loopStartLabel + ":\n";
            retString += VisitChildren(node, table);
            retString += "\t\tbr " + loopStartLabel + "\n";
            loopLabels.Pop();
            retString += "\t\t" + loopEndLabel + ":\n";
            return retString;
        }

        //-----------------------------------------------------------
        private string Visit(ExitStatement node, Table table)
        {
            return "\t\tbr " + loopLabels.Peek() + "\n";
        }

        //-----------------------------------------------------------
        private string Visit(ReturnStatement node, Table table)
        {
            return Visit((dynamic) node[0], table)
                + "\t\tret";
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
                retString += "\t\tcall void class 'ChimeraProgram'::'" + procName + "'(" + procParamsTypes + ")\n";
            }

            return retString;
        }

        //-----------------------------------------------------------
        private string Visit(Call node, Table table)
        {
            var retString = VisitChildren(node, table);

            var procName = node.AnchorToken.Lexeme;
            var proc = GPTable[procName];
            var procParamsTypes = "";
            var procParams = proc.getParameters();

            if (procParams.Length > 0)
            {
                procParamsTypes = CILTypes[procParams[0].LocalType];
                for (var i = 1; i < procParams.Length; i++)
                {
                    procParamsTypes += "," + CILTypes[procParams[i].LocalType];
                }
            }

            if (proc.IsPredefined)
            {
                retString += "\t\tcall " + CILTypes[proc.ReturnType] + " class ['chimeralib']'Chimera'.'Utils'::'" + procName + "'(" + procParamsTypes + ")\n";
            }
            else
            {
                retString += "\t\tcall " + CILTypes[proc.ReturnType]  + " class 'ChimeraProgram'::'" + procName + "'(" + procParamsTypes + ")\n";
            }

            return retString;
        }

        //-----------------------------------------------------------
        private string Visit(Identifier node, Table table)
        {
            var identifierName = node.AnchorToken.Lexeme;
            if (table is GlobalSymbolTable)
            {
                return "\t\tldsfld " + CILTypes[GSTable[identifierName].TheType] + " 'ChimeraProgram'::'" + identifierName + "'\n";
            }
            else
            {

                LocalSymbolTable lstable = table as LocalSymbolTable;
                if (lstable.Contains(identifierName))
                {
                    if (lstable[identifierName].Kind == Clasification.PARAM)
                    {
                        return "\t\tldarg '" + identifierName + "'\n";
                    }
                    else
                    {
                        return "\t\tldloc '" + identifierName + "'\n";
                    }
                }
                else
                {
                    return "\t\tldsfld " + CILTypes[GSTable[identifierName].TheType] + " 'ChimeraProgram'::'" + identifierName + "'\n";
                }
            }
            return "";
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

        //-----------------------------------------------------------
        public string Visit(Negative node, Table table)
        {
            return Visit((dynamic)node[0],table) + "\n\t\tneg\n";
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

           "\t\tldc.i4.1\n"
            + "{1}{2}"
            + "\t\tbge {0}\n"
            + "\t\tpop\n"
            + "\t\tldc.i4.0\n"
            + "\t\t{0}:\n",
                label,
                Visit((dynamic)node[0], table),
                Visit((dynamic)node[1], table));
        }

        //-----------------------------------------------------------
        public string Visit(SmallerEq node, Table table)
        {
            string label = GenerateLabel();
            return String.Format(

           "\t\tldc.i4.1\n"
            + "{1}{2}"
            + "\t\tble {0}\n"
            + "\t\tpop\n"
            + "\t\tldc.i4.0\n"
            + "\t\t{0}:\n",
                label,
                Visit((dynamic)node[0], table),
                Visit((dynamic)node[1], table));
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
