/*
 * A01373670 - Rodrigo Garcia Lopez
 * A01372074 - Jorge Alexis Rubio Sumano
 * A01371084 - Valentin Ochoa Lopez
*/

using System;
using System.IO;
using System.Text;

namespace Chimera
{

    public class Driver
    {

        const string VERSION = "0.4";

        //-----------------------------------------------------------
        static readonly string[] ReleaseIncludes = {
            "Lexical analysis",
            "Syntactic analysis",
            "AST construction",
            "Semantic analysis",
            "Semantic analysis",
            "CIL code generation"
        };

        //-----------------------------------------------------------
        void PrintAppHeader()
        {
            Console.WriteLine("Chimera compiler, version " + VERSION);
            Console.WriteLine(
                "Copyright \u00A9 2019 by V. Ochoa, R. García & J.A. Rubio.");
            Console.WriteLine("This program is free software; you may "
                + "redistribute it under the terms of");
            Console.WriteLine("the GNU General Public License version 3 or "
                + "later.");
            Console.WriteLine("This program has absolutely no warranty.");
        }

        //-----------------------------------------------------------
        void PrintReleaseIncludes()
        {
            Console.WriteLine("Included in this release:");
            foreach (var phase in ReleaseIncludes)
            {
                Console.WriteLine("   * " + phase);
            }
        }

        //-----------------------------------------------------------
        void Run(string[] args)
        {
            PrintAppHeader();
            Console.WriteLine();
            PrintReleaseIncludes();
            Console.WriteLine();

            if (args.Length != 2)
            {
                Console.Error.WriteLine("Please specify the name of the input and output files.");
                Environment.Exit(1);
            }

            try
            {
                var inputPath = args[0];
                var outputPath = args[1];
                var input = File.ReadAllText(inputPath);

                /* Lexical analysis * /
                Console.WriteLine(String.Format(
                    "===== Tokens from: \"{0}\" =====", inputPath)
                );
                var count = 1;
                foreach (var tok in new Scanner(input).Start()) {
                    Console.WriteLine(String.Format("[{0}] {1}", 
                                                    count++, tok)
                    );
                }
                    
                /* Lexical + syntactic analysis* /
                var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                parser.Program();
                Console.WriteLine("Syntax OK.");

                /* Lexical + syntactic analysis + AST construction* /
                var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                var program = parser.Program();
                Console.Write(program.ToStringTree());

                /* Lexical + syntactic analysis + AST construction + Semantic Analyzer*/
                var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                var program = parser.Program();
                Console.WriteLine("---START SYNTAX TREE---");
                Console.Write(program.ToStringTree());//
                Console.WriteLine("---END SYNTAX TREE---");

                var semantic = new SemanticAnalyzer();
                semantic.Visit((dynamic)program);
                Console.WriteLine(semantic.GSTable);
                Console.WriteLine(semantic.GPTable);
                Console.WriteLine("Semantics OK.");


                var codeGenerator = new CILGenerator(semantic.GSTable, semantic.GPTable);
                var code = codeGenerator.Visit((dynamic)program);
                File.WriteAllText(outputPath, code);
                Console.WriteLine(code);
                Console.WriteLine("Generated CIL code to '" + outputPath + "'.");
                Console.WriteLine();
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }
            catch (SyntaxError e)
            {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(2);
            }
            catch (SemanticError e)
            {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(3);
            }
        }

        //-----------------------------------------------------------
        void TestSyntacticAnalysis()
        {
            PrintAppHeader();
            Console.WriteLine();
            PrintReleaseIncludes();
            Console.WriteLine();

            foreach (string filename in Directory.EnumerateFiles("Tests", "*.chimera"))
            {
                try
                {
                    var input = File.ReadAllText(filename);
                    var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                    parser.Program();
                    Console.WriteLine("> " + filename + " PASSED");
                }
                catch (SyntaxError e)
                {
                    Console.WriteLine("> " + filename + " FAILED");
                    Console.Error.WriteLine(e.Message);
                }
            }

        }

        //-----------------------------------------------------------
        public static void Main(string[] args)
        {
            new Driver().Run(args);
            //new Driver().TestSyntacticAnalysis();
        }
    }
}
