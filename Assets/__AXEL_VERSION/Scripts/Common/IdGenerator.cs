using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdGenerator
{
    public class Id
    {
        private static System.Random rnd = new System.Random();

        public static string Generate()
        {
            string newId = GenerateLetter() + GenerateLetter() + GenerateLetter() + GenerateLetter();

            newId += "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString();

            return newId;
        }
        

        private static string GenerateLetter()
        {
            string rtnStr = "";

            int rndInt = rnd.Next(0, 26);

            switch (rndInt)
            {
                case 0:
                    rtnStr += "a";
                    break;

                case 1:
                    rtnStr += "b";
                    break;

                case 2:
                    rtnStr += "c";
                    break;

                case 3:
                    rtnStr += "d";
                    break;
                case 4:
                    rtnStr += "e";
                    break;
                case 5:
                    rtnStr += "f";
                    break;
                case 6:
                    rtnStr += "g";
                    break;
                case 7:
                    rtnStr += "h";
                    break;
                case 8:
                    rtnStr += "i";
                    break;
                case 9:
                    rtnStr += "j";
                    break;
                case 10:
                    rtnStr += "k";
                    break;
                case 11:
                    rtnStr += "l";
                    break;
                case 12:
                    rtnStr += "m";
                    break;
                case 13:
                    rtnStr += "n";
                    break;
                case 14:
                    rtnStr += "o";
                    break;
                case 15:
                    rtnStr += "p";
                    break;
                case 16:
                    rtnStr += "q";
                    break;
                case 17:
                    rtnStr += "r";
                    break;
                case 18:
                    rtnStr += "s";
                    break;
                case 19:
                    rtnStr += "t";
                    break;
                case 20:
                    rtnStr += "u";
                    break;
                case 21:
                    rtnStr += "v";
                    break;
                case 22:
                    rtnStr += "w";
                    break;
                case 23:
                    rtnStr += "x";
                    break;
                case 24:
                    rtnStr += "y";
                    break;
                case 25:
                    rtnStr += "z";
                    break;
            }

            return rtnStr;
        }

    }
}
