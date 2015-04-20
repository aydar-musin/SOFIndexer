using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Searcher
{
    class QueryProcessor
    {
        //Here will be the same things as in Indexer class

        //Bag of words for query
        public Dictionary<string,int> BagOfWords;
        public List<string> StopWords;

        public QueryProcessor()
        {
            StopWords = new List<string>();//implement stopwords loading!!!!
        }
        public void ProcessQuery(string query)
        {
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
                        ProcessToken(tokens[i]);
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
                        ProcessToken(tokens[i]);
                    }

                    ProcessTerm(term);//add this expression to terms list
                }
                else
                {
                    //do normalization

                    ProcessTerm(token);
                }
            }
        }
        public void BuildBagOfWords(string[] tokens)
        {

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
    }
}
