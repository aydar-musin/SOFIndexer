using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace FilesIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Files directory: ");
            string directory = Console.ReadLine();

            int count = 0;

            foreach (var dir in Directory.GetDirectories(directory))
            {
                var files = Directory.GetFiles(dir);

                files.AsParallel().WithDegreeOfParallelism(10).ForAll((f) =>
                    {
                            Indexer indexer = new Indexer();
                            indexer.ProcessFile(f);
                            Console.WriteLine(++count+" "+Thread.CurrentThread.ManagedThreadId);
                        
                    });
            }

            Console.WriteLine("Complete!");
            Console.ReadKey();
            
        }
        static string[] GetTokens(string fileName)
        {
            var text=File.ReadAllText(fileName);
            return text.Split(' ','.','!','?',',','\n','&','#','\t','\r','\v','=').Where(s=>!string.IsNullOrEmpty(s)).ToArray();
        }
    }
    
}
