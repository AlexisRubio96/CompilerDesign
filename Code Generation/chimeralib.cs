/*
 * A01373670 - Rodrigo Garcia Lopez
 * A01372074 - Jorge Alexis Rubio Sumano
 * A01371084 - Valentin Ochoa Lopez
*/

using System.Collections;
using System.Collections.Generic;

namespace Chimera {
    
    using System;

    public class Utils {

        public static void WrInt(int i)
        {
            Console.WriteLine(i);
        }

        public static void WrBool(bool b)
        {
            Console.WriteLine(b);
        }
         public static void WrLn( )
        {
            Console.WriteLine();
        }
       public static int RdInt(){
            String input = Console.ReadLine();
			int output;
			bool good = Int32.TryParse(input,out output);
			if(!good){
				throw new ArgumentException("Invalid input, must be an int value.");
			}
            return output;
            
        }
        public static String RdStr() {
            String input = Console.ReadLine();
            return input;
        }
        public static char AtStr(String s, int i){
			return s[i];
        }
        public static int LenStr(String s){
			return s.Length;
        }
        public static int CmpStr(String s1, String s2){
            return string.Compare(s1,s2);
        }
        public static String CatStr(String s1,String s2){
            return s1+s2;
        }
       public static int LenLstInt(List<int> lst){
            return lst.Count;
        }
         public static int LenLstStr(List<String> lst){
            return lst.Count;
        }
        public static int LenLstBool(List<Boolean> lst){
            return lst.Count;
        }
       	public static List<int> NewLstInt(int size){
            List<int> L = new List<int> ( new int[size] );
            return L;
        }
        public static List<string> NewLstStr(int size){
            List<string> L = new List<string> ( new string[size] );
            return L;
        }
        public static List<bool> NewLsBool(int size){
            List<bool> L = new List<bool> ( new bool[size] );
            return L;
        }
        public static String IntToStr(int i){
		    return i.ToString();
        }
        public static int StrToInt(String s){
            int i;
		    Boolean good = Int32.TryParse(s, out i);
		  	if(!good){
				throw new ArgumentException("Invalid input, must be an int value.");
			}
            return i;
        }


    }
}