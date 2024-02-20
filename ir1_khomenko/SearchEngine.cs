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
        private readonly Dictionary<string, List<Tuple<int, int>>> phrasalIndex;
        private readonly Dictionary<string, Dictionary<int, List<int>>> coordinateInvertedIndex;
        private readonly bool[,] incidenceMatrix;
        private Dictionary<string, int> termIndexMap;

        public SearchEngine(
            Dictionary<string,HashSet<int>> invertedIndex,
            bool[,] incidenceMatrix,
            Dictionary<string, List<Tuple<int, int>>> phrasalIndex,
            Dictionary<string, Dictionary<int, List<int>>> coordinateInvertedIndex,
            Dictionary<string, int> termIndexMap)
        {
            this.invertedIndex = invertedIndex;
            this.incidenceMatrix = incidenceMatrix;
            this.phrasalIndex = phrasalIndex;
            this.coordinateInvertedIndex = coordinateInvertedIndex;
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

        public HashSet<int> PhraseSearch(string query)
        {
            HashSet<int> results = new();

            List<string> terms = ParseQuery(query);

            if (terms.Count > 2)
            {
                // More than two words
                HashSet<int> biword1 = new();
                HashSet<int> biword2 = new();

                List<string> biwords = GetBiwords(terms);
                foreach (var biword in biwords)
                {
                    Console.WriteLine(biword);
                    if (phrasalIndex.ContainsKey(biword))
                    {
                        foreach (var tuple in phrasalIndex[biword])
                        {
                            int documentId = tuple.Item1;
                            if(biword == biwords[0])
                            {
                                biword1.Add(documentId);
                            }
                            else
                            {
                                biword2.Add(documentId);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Biword {biword} not found");
                    }
                }

                // Intersect both biwords
                foreach (var documentID in biword1)
                {
                    if(biword2.Contains(documentID))
                    {
                        results.Add(documentID);
                    }
                }
            }
            else
            {
                // Only two words
                foreach (var phrasePair in GetPhrasePairs(terms))
                {
                    if (phrasalIndex.ContainsKey(phrasePair))
                    {
                        foreach (var tuple in phrasalIndex[phrasePair])
                        {
                            int documentId = tuple.Item1;

                            results.Add(documentId);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Pair not found");
                    }
                }
            }

            return results;
        }

        private List<string> GetBiwords(List<string> terms)
        {
            List<string> biwords = new();
            for (int i = 0; i < terms.Count - 1; i++)
            {
                biwords.Add($"{terms[i]} {terms[i + 1]}");
            }
            return biwords;
        }

        public HashSet<int> DistanceBasedSearch(string word1, string word2)
        {
            HashSet<int> results = new();

            string termPair = $"{word1.ToLower()} {word2.ToLower()}";

            if(coordinateInvertedIndex.ContainsKey(termPair))
            {
                foreach (var docEntry in coordinateInvertedIndex[termPair])
                {
                    List<int> positions = docEntry.Value;
                    for(int i = 0; i < positions.Count - 1; i++)
                    {
                        if(positions[i + 1] - positions[i] == 1) {
                            results.Add(docEntry.Key);
                            break;
                        }
                    }
                }    
            }

            return results;
        }

        private IEnumerable<string> GetPhrasePairs(List<string> terms)
        {
            for (int i = 0; i < terms.Count - 1; i++)
            {
                yield return $"{terms[i].ToLower()} {terms[i + 1].ToLower()}";
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
