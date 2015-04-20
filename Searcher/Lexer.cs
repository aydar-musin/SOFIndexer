using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Searcher
{
    class Lexer
    {
        public static string[] getTokens(string s)
        {
                s = WebUtility.HtmlDecode(s);

                s = s.Replace("%", " % ").Replace("#", " # ").Replace("@", " @ ").Replace("$", " $ ");

                char[] delimiterChars = { '\n', '\r', ' ', '\t', '\v', '-', '.', ';', ':', '`', '?', '!', '|', '\"', '\\', '(', ')', '*', '=', '+', '\'', '~', '>', '<', '&', ']', '[', '^', ',' };
                string[] list = s.Split(delimiterChars);
                List<string> tokens = new List<string>();

                foreach (string token in list)
                {
                    if (token.Trim().Length > 0)
                    {
                        tokens.Add(line.Trim());
                    }
                }

                return filledLines.ToArray();
        }
    }
}
