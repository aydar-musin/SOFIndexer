using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FilesIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            var tokens = GetTokens("test.txt");
            Console.ReadKey();

        }
        static string[] GetTokens(string fileName)
        {
            var text=File.ReadAllText(fileName);
            return text.Split(' ','.','!','?',',','\n','&','#','\t','\r','\v').Where(s=>!string.IsNullOrEmpty(s)).ToArray();
        }
    }
    
}
