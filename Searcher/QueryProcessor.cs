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
        private int N=1000000;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=SOFIndex;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";

        public QueryProcessor()
        {
            StopWords = new List<string>();//stopwords loading
            var lines = File.ReadAllLines(@"C:\Users\Айдар\SkyDrive\Учеба\innopolis\IR\SOFIndexer\StopWords.txt").Select(l => l.Trim());
            StopWords=lines.ToList();
        }
        public List<QueryResult> ProcessQuery(string query)
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
            var search_result = Search();
            var rank_result = RankResults(search_result);

            return rank_result;
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
        private Dictionary<int,QueryResult> Search()
        {
            Dictionary<int, QueryResult> vectors = new Dictionary<int, QueryResult>();

            SqlConnection connection = new SqlConnection(this.connectionString);
            connection.Open();

            foreach (var term in BagOfWords)
            {
                SqlCommand cmd = new SqlCommand(
@"
declare @termId int
declare @df int
set @termId=(select top 1 Id from Terms where term=@term)
IF @termId!=0
BEGIN
	set @df=(select DocumentFrequency from Terms where Id=@termId)
	
	IF @df is null
	BEGIN
		set @df=(select count(TermId) from Postings where TermId=@termId)
		update Terms set DocumentFrequency=@df where Id=@termId
	END

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

                        double tfidf = (1 + Math.Log10((double)tf)) * Math.Log10(N / df);//tfidf calculating

                        if (vectors.ContainsKey(DocId))
                        {
                            var new_res = (QueryResult) vectors[DocId].Clone();
                            new_res.ContainsWords++;
                            new_res.TfiDf += tfidf;

                            vectors[DocId] = new_res;
                        }
                        else
                        {
                            vectors.Add(DocId, new QueryResult() { DocId=DocId, TfiDf=tfidf, ContainsWords=1 });
                        }
                    }
                } 
            }
            connection.Close();


            return vectors;
        }

        private List<QueryResult> RankResults(Dictionary<int,QueryResult> input)
        {
            var ordered_input = input.OrderByDescending(g => g.Value.ContainsWords);

            List<List<QueryResult>> tempGroups = new List<List<QueryResult>>();

            List<QueryResult> group = new List<QueryResult>();
            int last = -1;
            foreach (var q in ordered_input)
            {
                if (q.Value.ContainsWords == last||last==-1)
                    group.Add(q.Value);
                else
                {
                    tempGroups.Add(group);
                    group = new List<QueryResult>();
                    group.Add(q.Value);
                }
                last = q.Value.ContainsWords;
            }

            List<QueryResult> result = new List<QueryResult>();
            if (tempGroups.Count == 0 && group.Count != 0)
                tempGroups.Add(group);

            foreach (var g in tempGroups)
                result.AddRange(g.OrderByDescending(q => q.TfiDf));

            return result;

        }

    }

    public class QueryResult:ICloneable
    {
        public int DocId;
        public double TfiDf;
        public int ContainsWords;


        public object Clone()
        {
            return new QueryResult() 
            {
                DocId=this.DocId,
                TfiDf=this.TfiDf,
                ContainsWords=this.ContainsWords
            };
        }
    }
}
