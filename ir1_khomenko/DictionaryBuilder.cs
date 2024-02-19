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
                    string wordPair = $"{words[i].ToLower()} {words[i + 1].ToLower()}";

                    if (!wordCount.ContainsKey(wordPair))
                    {
                        wordCount[wordPair] = 1;
                        termsList.Add(wordPair);
                        PhrasalIndex[wordPair] = new List<Tuple<int, int>>();
                        CoordinateInvertedIndex[wordPair] = new Dictionary<int, List<int>>();
                        InvertedIndex[wordPair] = new HashSet<int>();
                    }
                    else
                    {
                        wordCount[wordPair]++;
                    }

                    int documentId = allText.IndexOf(text);
                    PhrasalIndex[wordPair].Add(new Tuple<int, int>(documentId, i));

                    if (!CoordinateInvertedIndex[wordPair].ContainsKey(documentId))
                    {
                        CoordinateInvertedIndex[wordPair][documentId] = new List<int>();
                    }
                    CoordinateInvertedIndex[wordPair][documentId].Add(i);

                    InvertedIndex[wordPair].Add(documentId);
                }
            }

            termIndexMap = new();

            for(int index = 0; index < termsList.Count; index++)
            {
                termIndexMap[termsList[index]] = index;
            }

            IncidenceMatrix = new bool[allText.Count, termsList.Count];

            for (int i = 0; i < allText.Count; i++)
            {
                string text = allText[i];
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string word in words)
                {
                    string cleanWord = word.ToLower();
                    if(termIndexMap.TryGetValue(cleanWord, out int termIndex))
                    {
                        IncidenceMatrix[i, termIndex] = true;
                    }
                }
            }

            return wordCount;
		}

        public Dictionary<string, int> GetTermIndexMap()
        {
            return termIndexMap;
        }
	}
}