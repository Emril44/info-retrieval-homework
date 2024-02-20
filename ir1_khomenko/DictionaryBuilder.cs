using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
    class DictionaryBuilder
    {
        public Dictionary<string, HashSet<int>> InvertedIndex { get; private set; }
        public Dictionary<string, List<Tuple<int, int>>> PhrasalIndex { get; private set; }
        public Dictionary<string, Dictionary<int, List<int>>> CoordinateInvertedIndex { get; private set; }
        public bool[,] IncidenceMatrix { get; private set; }
        private Dictionary<string, int> termIndexMap;

        // TODO: Phrasal index & coordinate inverted index
		public Dictionary<string, int> BuildDictionary(List<string> allText)
		{
            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            InvertedIndex = new Dictionary<string, HashSet<int>>();
            PhrasalIndex = new Dictionary<string, List<Tuple<int, int>>>();
            CoordinateInvertedIndex = new Dictionary<string, Dictionary<int, List<int>>>();
            List<string> termsList = new List<string>();

            foreach (string text in allText)
            {
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

                for(int i = 0; i < words.Length - 1; i++)
                {
                    string cleanWord = words[i].ToLower();

                    if (!wordCount.ContainsKey(cleanWord))
                    {
                        wordCount[cleanWord] = 1;
                        termsList.Add(cleanWord);
                        InvertedIndex[cleanWord] = new HashSet<int>();
                        CoordinateInvertedIndex[cleanWord] = new Dictionary<int, List<int>>();
                    }
                    else
                    {
                        wordCount[cleanWord]++;
                    }

                    if(i < words.Length - 1)
                    {
                        string wordPair = $"{cleanWord} {words[i + 1].ToLower()}";

                        if(!wordCount.ContainsKey(wordPair))
                        {
                            wordCount[wordPair] = 1;
                            termsList.Add(wordPair);
                            PhrasalIndex[wordPair] = new();
                        }
                        else
                        {
                            wordCount[wordPair]++;
                        }

                        int documentId = allText.IndexOf(text);
                        PhrasalIndex[wordPair].Add(new Tuple<int, int>(documentId, i));

                        if (!CoordinateInvertedIndex[cleanWord].ContainsKey(documentId))
                        {
                            CoordinateInvertedIndex[cleanWord][documentId] = new List<int>();
                        }
                        CoordinateInvertedIndex[cleanWord][documentId].Add(i);

                        InvertedIndex[cleanWord].Add(documentId);
                    }
                }
            }

            //foreach(var termPair in CoordinateInvertedIndex)
            //{
            //    Console.WriteLine($"Term pair: {termPair.Key}");

            //    foreach(var docEntry in termPair.Value)
            //    {
            //        int docID = docEntry.Key;
            //        List<int> positions = docEntry.Value;

            //        Console.WriteLine($"Doc ID: {docID}");
            //        Console.WriteLine("Positions:");
            //        Console.WriteLine(string.Join(", ", positions));
            //    }
            //}

            //foreach(var termPair in PhrasalIndex)
            //{
            //    string term = termPair.Key;
            //    Console.WriteLine($"Term pair: {term}");

            //    List<Tuple<int,int>> tuples = termPair.Value;
            //    foreach(var tuple in tuples)
            //    {
            //        int docID = tuple.Item1;
            //        int pos = tuple.Item2;
            //        Console.WriteLine($"Doc ID: {docID}, Pos: {pos}");
            //    }
            //}

                //termIndexMap = new();

                //for(int index = 0; index < termsList.Count; index++)
                //{
                //    termIndexMap[termsList[index]] = index;
                //}

                //IncidenceMatrix = new bool[allText.Count, termsList.Count];

                //for (int i = 0; i < allText.Count; i++)
                //{
                //    string text = allText[i];
                //    string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

                //    foreach (string word in words)
                //    {
                //        string cleanWord = word.ToLower();
                //        if(termIndexMap.TryGetValue(cleanWord, out int termIndex))
                //        {
                //            IncidenceMatrix[i, termIndex] = true;
                //        }
                //    }
                //}

                return wordCount;
		}

        public Dictionary<string, int> GetTermIndexMap()
        {
            return termIndexMap;
        }
	}
}