using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace Searcher
{
    public class QueryProcessor
    {
        //Here will be the same things as in Indexer class

        //Bag of words for query
        public Dictionary<string,int> BagOfWords;
        public List<string> StopWords;
        private int N=10000;
        private string connectionString = "";

        public QueryProcessor()
        {
            StopWords = new List<string>();//stopwords loading
            var lines = File.ReadAllLines(@"C:\Users\Айдар\SkyDrive\Учеба\innopolis\IR\SOFIndexer\StopWords.txt").Select(l => l.Trim());
            StopWords=lines.ToList();
        }
        public Dictionary<int, double> ProcessQuery(string query)
        {
            BagOfWords = new Dictionary<string, int>();

            var tokens = Lexer.getTokens(query);
            tokens = tokens.Select(s => s.ToLower()).ToArray();

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                if (token == "\"")
                {
                    string term = "";
                    i++;
                    while (tokens[i] != "\"" && i < tokens.Length - 1)//waiting for the end of quotes
                    {
                        term += tokens[i];
                        i++;
                        ProcessTerm(tokens[i]);
                    }

                    ProcessTerm(term);//add this expression to terms list
                }
                else if (token == "\'")
                {
                    string term = "";
                    i++;
                    while (tokens[i] != "\'" && i < tokens.Length - 1)//waiting for the end of quotes
                    {
                        term += tokens[i];
                        i++;
                        ProcessTerm(tokens[i]);
                    }

                    ProcessTerm(term);//add this expression to terms list
                }
                else
                {
                    //do normalization
                    ProcessTerm(token);
                }
            }
            var result = Search();
            return result;
        }
        private void ProcessTerm(string term)
        {
            if (!StopWords.Contains(term))
            {
                if (BagOfWords.ContainsKey(term))
                    BagOfWords[term] = BagOfWords[term] + 1;
                else
                    BagOfWords.Add(term, 1);
            }
        }
        private Dictionary<int, double> Search()
        {
            Dictionary<int, double> vectors = new Dictionary<int, double>();

            SqlConnection connection = new SqlConnection(@"Data Source=tcp:178.217.40.162;Initial Catalog=SOFIndex;Integrated Security=False;User ID=arbitrclient;Password=159753;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
            connection.Open();

            foreach (var term in BagOfWords)
            {
                SqlCommand cmd = new SqlCommand(@"declare @termId int
                                                  declare @df int
                                                set @termId=(select Id from Terms where term=@term and DocumentFrequency is not null)
                                                IF @termId!=0
                                                BEGIN
	                                                set @df=(select DocumentFrequency from Terms where Id=@termId)
	                                                select @termId,@df,DocId, termFrequency from Postings where TermId=@termId
                                                END", connection);

                cmd.Parameters.AddWithValue("term", term.Key);

                using (var reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        int TermId = reader.GetInt32(0);
                        int df = reader.GetInt32(1);
                        int DocId = reader.GetInt32(2);
                        int tf = reader.GetInt32(3);
                        double tfidf = (1 + Math.Log10((double)tf)) * Math.Log10(N / df);

                        if (vectors.ContainsKey(DocId))
                            vectors[DocId] += tfidf;
                        else
                        {
                            vectors.Add(DocId, tfidf);
                        }
                    }
                } 
            }
            connection.Close();
            return vectors;
        }
    }
}
