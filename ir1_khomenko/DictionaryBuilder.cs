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
            
            IncidenceMatrix = new bool[allText.Count, termsList.Count];

            for (int i = 0; i < allText.Count; i++)
            {
                string text = allText[i];
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string word in words)
                {
                    string cleanWord = word.ToLower();
                    int termIndex = termsList.IndexOf(cleanWord);
                    IncidenceMatrix[i, termIndex] = true;
                }
            }

            // Test output (inverted index)
            //foreach (var term in invertedIndex)
            //{
            //	Console.Write($"{term.Key}: ");
            //	foreach (var docID in term.Value)
            //	{
            //		Console.Write($"{docID} ");
            //	}
            //	Console.WriteLine();
            //}

            // Test output (incidence matrix)
            //for (int i = 0; i < allText.Count; i++)
            //{
            //	Console.WriteLine($"Document {i}:");
            //	for (int j = 0; j < termsList.Count; j++)
            //	{
            //		Console.Write($"{termsList[j]}: {(incidenceMatrix[i, j] ? "1" : "0")} ");
            //	}
            //	Console.WriteLine();
            //}

            return wordCount;
		}
	}
}