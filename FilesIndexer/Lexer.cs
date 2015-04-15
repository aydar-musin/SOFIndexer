using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FilesIndexer
{
    class Lexer
    {

        string fileName;
        public Lexer(string fName) { fileName = fName; }

        public string[] getTokens()
        {
            // Open the file and read it back.
            using (System.IO.StreamReader sr = System.IO.File.OpenText(fileName))
            {
                string s = sr.ReadToEnd();

                s = s.Replace("%", " % ").Replace("#", " # ").Replace("@", " @ ").Replace("$", " $ ");
              

                char[] delimiterChars = { '\n', '\r', ' ', '\t', '\v','-','.',';',':','`','?','!','|','\"','\\','(',')','*','=','+','\'','~','>','<','&',']','[','^'};
                string[] lines = s.Split(delimiterChars);
                List<string> filledLines = new List<string>();

                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        filledLines.Add(line);
                    }
                }

                return filledLines.ToArray();

            }
        }


    }
}

