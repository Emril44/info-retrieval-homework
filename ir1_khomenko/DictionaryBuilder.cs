using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
	class DictionaryBuilder
	{
        public Dictionary<string, HashSet<int>> InvertedIndex {  get; private set; }
        public bool[,] IncidenceMatrix { get; private set; }
        private Dictionary<string, int> termIndexMap;

		public Dictionary<string, int> BuildDictionary(List<string> allText)
		{
            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            InvertedIndex = new Dictionary<string, HashSet<int>>();
            List<string> termsList = new List<string>();

            foreach (string text in allText)
            {
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string word in words)
                {
                    string cleanWord = word.ToLower();

                    if (!wordCount.ContainsKey(cleanWord))
                    {
                        wordCount[cleanWord] = 1;
                        termsList.Add(cleanWord);
                        InvertedIndex[cleanWord] = new HashSet<int>();
                    }
                    else
                    {
                        wordCount[cleanWord]++;
                    }

                    InvertedIndex[cleanWord].Add(allText.IndexOf(text));
                }
            }

            // Debug statement to print termsList
            //Console.WriteLine("Terms List:");
            //foreach (var term in termsList)
            //{
            //    Console.WriteLine(term);
            //}

            termIndexMap = new();

            for(int index = 0; index < termsList.Count; index++)
            {
                termIndexMap[termsList[index]] = index;
            }

            //Console.WriteLine("Term Index Map:");
            //foreach (var kvp in termIndexMap)
            //{
            //    Console.WriteLine($"Term: {kvp.Key}, Index: {kvp.Value}");
            //}

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