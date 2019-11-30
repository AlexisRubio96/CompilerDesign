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
      	 public static int LenLstInt(int[] lst){
            return lst.Length;
        }
         public static int LenLstStr(String[] lst){
            return lst.Length;
        }
        public static int LenLstBool(Boolean[] lst){
            return lst.Length;
        }
       	public static int[] NewLstInt(int size){
            int[] L = new int[size];
            return L;
        }
        public static String[] NewLstInt(int size){
            String[] L = new String[size];
            for(int i = 0;i < L.Length; i++ ){
                L[i] = "";
            }
            return L;
        }
       public static bool[] NewLstInt(int size){
            bool[] L = new bool[size];
            return L;
        }
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