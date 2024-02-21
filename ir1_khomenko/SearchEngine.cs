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

        public SearchEngine(
            Dictionary<string,HashSet<int>> invertedIndex,
            Dictionary<string, List<Tuple<int, int>>> phrasalIndex,
            Dictionary<string, Dictionary<int, List<int>>> coordinateInvertedIndex)
        {
            this.invertedIndex = invertedIndex;
            this.phrasalIndex = phrasalIndex;
            this.coordinateInvertedIndex = coordinateInvertedIndex;
        }

        public HashSet<int> BooleanSearch(string query)
        {
            if(invertedIndex != null)
            {
                return EvaluateQuery(query);
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

        public HashSet<int> DistanceBasedSearch(string word1, string word2, int distance)
        {
            HashSet<int> results = new();

            // toLower both terms for case insensitivity
            word1 = word1.ToLower();
            word2 = word2.ToLower();

            // Check if terms exist in the index
            if(coordinateInvertedIndex.ContainsKey(word1) && coordinateInvertedIndex.ContainsKey(word2))
            {
                // Get posting lists from both terms
                var postings1 = coordinateInvertedIndex[word1];
                var postings2 = coordinateInvertedIndex[word2];

                // Iterate over common docs between posting lists
                foreach (var docEntry in postings1.Keys.Intersect(postings2.Keys))
                {
                    // Get positions of both terms in the doc
                    List<int> positions1 = postings1[docEntry];
                    List<int> positions2 = postings2[docEntry];

                    // Check if occurrence happens
                    if(HasOccurrenceWithinDistance(positions1, positions2, distance))
                    {
                        results.Add(docEntry);
                    }
                }    
            }

            return results;
        }

        // Check if occurrence happens within specified distance
        private bool HasOccurrenceWithinDistance(List<int> positions1, List<int> positions2, int distance)
        {
            foreach(var pos1 in positions1)
            {
                foreach(var pos2 in positions2)
                {
                    if(Math.Abs(pos1 - pos2) < distance)
                    {
                        // Found within distance
                        return true;
                    }
                    else if(pos2 > pos1 + distance)
                    {
                        // Positions too far apart, move to next pos of term 1
                        break;
                    }
                }
            }

            // Nothing found
            return false;
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

        private HashSet<int> EvaluateQuery(string query)
        {
            HashSet<int> results;

            List<string> terms = ParseQuery(query);

            results = EvaluateInvertedIndex(terms);

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
