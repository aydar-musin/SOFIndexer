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
            Indexer d = new Indexer();
            var files = Directory.GetFiles("files");

            foreach (var file in files)
            {
                d.ProcessFile(file);
                Console.WriteLine(file+" processed");
            }
            Console.ReadKey();
            "1test1".Replace("1", " 1 ");
        }
        static string[] GetTokens(string fileName)
        {
            var text=File.ReadAllText(fileName);
            return text.Split(' ','.','!','?',',','\n','&','#','\t','\r','\v','=').Where(s=>!string.IsNullOrEmpty(s)).ToArray();
        }
    }
    
}
