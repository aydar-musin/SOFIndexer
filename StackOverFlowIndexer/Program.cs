using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Threading;
using System.Configuration;

namespace StackOverFlowIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            int number = Convert.ToInt32(ConfigurationSettings.AppSettings["LastNumber"]);

            if (!Directory.Exists("files"))
                Directory.CreateDirectory("files");

            Action d_action = new Action(() =>
                {
                    WebClient wclient = new WebClient();
                    while (true)
                    {

                        try
                        {
                            string text = wclient.DownloadString("http://stackoverflow.com/questions/" + number);

                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(text);

                            var el=doc.GetElementbyId("question").Element("table").Element("tr").Elements("td").ToArray()[1];
                            string result = el.Element("div").Elements("div").ToArray()[0].InnerText + "\n" + el.Element("div").Elements("div").ToArray()[1].InnerText;

                            WriteFile(result,number);

                            number++;

                            CountMessage(number);
                            Message("I'm working");
                        }
                        catch(Exception ex)
                        {
                            if (ex.Message.Contains("503"))
                            {
                                Message("I'm waiting...");
                                Thread.Sleep(60000);
                            }
                            else
                            {
                                number++;
                                Message("error: " + number);
                            }
                        }
                    }
                });

            Task.Factory.StartNew(d_action);
            Task.Factory.StartNew(d_action);
            Task.Factory.StartNew(d_action);

            Console.SetCursorPosition(0,2);
            Console.WriteLine("Press S to stop");
            while (Console.ReadKey().Key != ConsoleKey.S) ;
            Configuration conf = ConfigurationManager.OpenExeConfiguration(Environment.CurrentDirectory + "/StackOverFlowIndexer.exe");
            conf.AppSettings.Settings["LastNumber"].Value = number.ToString();
            conf.Save();
            
        }

        static object marker = new object();
        static void CountMessage(int number)
        {
            lock (marker)
            {
                Console.SetCursorPosition(0, 1);
                Console.WriteLine(number);
            }
        }
        static void Message(string message)
        {
            lock (marker)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(message);
            }
        }
        static int GetLastNumber()
        {
            try
            {
                var files = Directory.GetFiles("files");
                return files.Length;
            }
            catch
            {
                return 1;
            }
        }
        static void WriteFile(string text,int number)
        {
            string guid = Guid.NewGuid().ToString();
            string directory = "files/"+guid.Substring(0, 2);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(directory+"/"+guid+"#"+number.ToString()+".txt",text.Trim());
        }
    }
}
