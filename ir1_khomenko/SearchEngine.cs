using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
    public class SearchEngine
    {
        private readonly Dictionary<string, HashSet<int>> invertedIndex;
        private readonly bool[,] incidenceMatrix;
        private Dictionary<string, int> termIndexMap;

        public SearchEngine(Dictionary<string, HashSet<int>> invertedIndex, bool[,] incidenceMatrix, Dictionary<string, int> termIndexMap)
        {
            this.invertedIndex = invertedIndex;
            this.incidenceMatrix = incidenceMatrix;
            this.termIndexMap = termIndexMap;
        }

        public HashSet<int> BooleanSearch(string query)
        {
            if(invertedIndex != null)
            {
                return EvaluateQuery(query, true);
            }
            else if(incidenceMatrix != null)
            {
                return EvaluateQuery(query, false);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private List<string> ParseQuery(string query)
        {
            List<string> terms = new List<string>();

            string[] tokens = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach(string token in tokens)
            {
                string term;

                if (token == "AND" || token == "OR" || token == "NOT")
                {
                    term = token.Trim();
                }
                else
                {
                    term = token.Trim().ToLower();
                }

                terms.Add(term);
            }

            return terms;
        }

        private HashSet<int> EvaluateQuery(string query, bool useInvertedIndex)
        {
            HashSet<int> results;

            List<string> terms = ParseQuery(query);

            if (useInvertedIndex)
            {
                results = EvaluateInvertedIndex(terms);
            }
            else
            {
                results = EvaluateIncidenceMatrix(terms, termIndexMap);
            }

            return results;
        }

        private HashSet<int> EvaluateIncidenceMatrix(List<string> terms, Dictionary<string, int> termIndexMap)
        {
            HashSet<int> results = new HashSet<int>();
            for (int i = 0; i < incidenceMatrix.GetLength(0); i++)
            {
                results.Add(i);
            }

            HashSet<int> tempResults = new HashSet<int>();

            string logicalOperator = "OR"; // default
            foreach(string term in terms)
            {
                if(term == "AND" || term == "OR" || term == "NOT")
                {
                    logicalOperator = term;
                }
                else
                {
                    HashSet<int> newResults = new();

                    for(int docID = 0; docID < incidenceMatrix.GetLength(0); docID++)
                    {
                        if (incidenceMatrix[docID, termIndexMap[term]])
                        {
                            newResults.Add(docID);
                        }
                    }

                    if(logicalOperator == "AND")
                    {
                        tempResults.IntersectWith(newResults);
                    }
                    else if(logicalOperator == "OR")
                    {
                        tempResults.UnionWith(newResults);
                    }
                    else if(logicalOperator == "NOT")
                    {
                        tempResults.ExceptWith(newResults);
                    }
                }
            }

            results.IntersectWith(tempResults);

            return results;
        }

        private HashSet<int> EvaluateInvertedIndex(List<string> terms)
        {
            HashSet<int> results = new HashSet<int>(invertedIndex.Values.SelectMany(ids => ids));

            HashSet<int> tempResults = new();

            string logicalOperator = "OR";

            foreach(string term in terms)
            {
                if (term == "AND" || term == "OR" || term == "NOT")
                {
                    logicalOperator = term;
                    continue;
                }

                HashSet<int> newResults = invertedIndex.ContainsKey(term) ? invertedIndex[term] : new HashSet<int>();

                if (tempResults.Count == 0)
                {
                    tempResults = newResults;
                }
                else
                {
                    if (logicalOperator == "AND")
                    {
                        tempResults.IntersectWith(newResults);
                    }
                    else if (logicalOperator == "OR")
                    {
                        tempResults.UnionWith(newResults);
                    }
                    else if (logicalOperator == "NOT")
                    {
                        tempResults.ExceptWith(newResults);
                    }
                }
            }

            results.IntersectWith(tempResults);

            return results;
        }
    }
}
