using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Net;

namespace FilesIndexer
{
    class Indexer
    {
        public List<string> Terms;
        private List<string> StopWords;
        private int DocId;

        private Dictionary<string,int> BagOfWords;
        public Indexer()
        {
            StopWords = File.ReadAllLines("StopWords.txt").Select(s => s.Trim()).ToList();
        }
        public void ProcessFile(string fileName)
        {
            BagOfWords = new Dictionary<string, int>();
            DocId = GetDocIdFromFileName(fileName);
            
            Lexer lexer = new Lexer(fileName);
            var tokens = lexer.getTokens().Select(t=>t.Trim().ToLower()).ToArray();
            
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                if (token == "\"")
                {
                    string term = "";
                    i++;
                    while (tokens[i] != "\""&&i<tokens.Length-1)//waiting for the end of quotes
                    {
                        term += tokens[i];
                        i++;
                        ProcessToken(tokens[i]);
                    }

                    ProcessToken(term);//add this expression to terms list
                }
                else if (token == "\'")
                {
                    string term = "";
                    i++;
                    while (tokens[i] != "\'" && i < tokens.Length - 1)//waiting for the end of quotes
                    {
                        term += tokens[i];
                        i++;
                        ProcessToken(tokens[i]);
                    }

                    ProcessToken(term);//add this expression to terms list
                }
                else
                {
                    //do normalization

                    ProcessToken(token);
                }
            }
            InsertBagOfWords();
        }
        public void ProcessToken(string term)//
        {
            if(!StopWords.Contains(term))
            {
                if (BagOfWords.ContainsKey(term))
                    BagOfWords[term] = BagOfWords[term] + 1;
                else
                    BagOfWords.Add(term,1);
            }
        }
        public void InsertBagOfWords()
        {
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=SOFIndex;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
            sqlConnection.Open();

            foreach (var term in BagOfWords)
            {
                SqlCommand cmd = new SqlCommand() {
                    Connection=sqlConnection,
                    CommandText = "exec InsertPosting @Term=@termp,@DocId=@DocIdp,@tf=@tfp"
                };
                cmd.Parameters.AddWithValue("termp", term.Key);
                cmd.Parameters.AddWithValue("DocIdp",DocId);
                cmd.Parameters.AddWithValue("tfp",term.Value);

                cmd.ExecuteNonQuery();
            }
            sqlConnection.Close();
            BagOfWords = null;
        }

        private int GetDocIdFromFileName(string filename)
        {
            int index1 = filename.IndexOf('#');
            int index2 = filename.LastIndexOf(".");
            return Convert.ToInt32(filename.Substring(index1+1,index2-index1-1));
        }
    }
}
