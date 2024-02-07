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

        public SearchEngine(Dictionary<string, HashSet<int>> invertedIndex, bool[,] incidenceMatrix)
        {
            this.invertedIndex = invertedIndex;
            this.incidenceMatrix = incidenceMatrix;
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
                results = EvaluateIncidenceMatrix(terms);
            }

            return results;
        }

        private HashSet<int> EvaluateIncidenceMatrix(List<string> terms)
        {
            HashSet<int> results = new HashSet<int>();

            for (int i = 0; i < incidenceMatrix.GetLength(0); i++)
            {
                results.Add(i);
            }

            HashSet<int> tempResults = new HashSet<int>();

            for (int termIndex = 0; termIndex < terms.Count; termIndex++)
            {
                string term = terms[termIndex];

                if (term == "AND" || term == "OR" || term == "NOT")
                {
                    continue;
                }

                HashSet<int> newResults = new HashSet<int>();
                for (int docId = 0; docId < incidenceMatrix.GetLength(0); docId++)
                {
                    if (incidenceMatrix[docId, termIndex])
                    {
                        newResults.Add(docId);
                    }
                }

                if (termIndex > 0)
                {
                    string previousTerm = terms[termIndex - 1];
                    if (previousTerm == "AND")
                    {
                        if (tempResults.Count == 0)
                        {
                            tempResults.UnionWith(newResults);
                        }
                        else
                        {
                            tempResults.IntersectWith(newResults);
                        }
                    }
                    else if (previousTerm == "OR")
                    {
                        tempResults.UnionWith(newResults);
                    }
                    else if (previousTerm == "NOT")
                    {
                        tempResults.ExceptWith(newResults);
                    }
                }
                else
                {
                    tempResults = newResults;
                }
            }

            results.IntersectWith(tempResults);

            return results;
        }

        private HashSet<int> EvaluateInvertedIndex(List<string> terms)
        {
            HashSet<int> results = new HashSet<int>(invertedIndex.Values.SelectMany(ids => ids));

            HashSet<int> tempResults = new();

            foreach(string term in terms)
            {
                HashSet<int> newResults = new();

                if (term == "AND" || term == "OR" || term == "NOT")
                {
                    continue;
                }

                if (invertedIndex.ContainsKey(term))
                {
                    newResults = invertedIndex[term];
                }

                if (tempResults.Count == 0)
                {
                    tempResults = newResults;
                }
                else
                {
                    if (terms[terms.IndexOf(term) - 1] == "AND")
                    {
                        results.IntersectWith(newResults);
                    }
                    else if (terms[terms.IndexOf(term) - 1] == "OR")
                    {
                        results.UnionWith(newResults);
                    }
                    else if (terms[terms.IndexOf(term) - 1] == "NOT")
                    {
                        results.ExceptWith(newResults);
                    }
                }
            }

            results.IntersectWith(tempResults);

            return results;
        }
    }
}
