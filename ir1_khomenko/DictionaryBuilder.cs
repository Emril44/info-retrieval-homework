using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
	class DictionaryBuilder
	{
		public Dictionary<string, int> BuildDictionary(List<string> allText)
		{
			Dictionary<string, int> wordCount = new();
			
			Dictionary<string, HashSet<int>> invertedIndex = new();

			List<string> termsList = new();

			for (int i = 0; i < allText.Count; i++)
			{
				string text = allText[i];
				string[] words = text.Split(new char[] {' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”'}, StringSplitOptions.RemoveEmptyEntries);

				foreach (string word in words)
				{
					string cleanWord = word.ToLower();

					if (wordCount.TryGetValue(cleanWord, out int value))
					{
						wordCount[cleanWord] = ++value;
					}
					else
					{
						wordCount[cleanWord] = 1;
						termsList.Add(cleanWord);
					}

					if(!invertedIndex.ContainsKey(cleanWord))
					{
						invertedIndex[cleanWord] = new HashSet<int>();
					}
					invertedIndex[cleanWord].Add(i);
				}
			}

			// Test output
			//foreach (var term in invertedIndex)
			//{
			//	Console.Write($"{term.Key}: ");
			//	foreach (var docID in term.Value)
			//	{
			//		Console.Write($"{docID} ");
			//	}
			//	Console.WriteLine();
			//}

			bool[,] incidenceMatrix = new bool[allText.Count, termsList.Count];

			for (int i = 0; i < allText.Count; i++)
			{
				string text = allText[i];
				string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

				foreach (string word in words)
				{
					string cleanWord = word.ToLower();
					int termIndex = termsList.IndexOf(cleanWord);
					incidenceMatrix[i, termIndex] = true;
				}
			}

            // Test output
    //        for (int i = 0; i < allText.Count; i++)
    //        {
				//Console.WriteLine($"Document {i}:");
    //            for (int j = 0; j < termsList.Count; j++)
    //            {
    //                Console.Write($"{termsList[j]}: {(incidenceMatrix[i, j] ? "1" : "0")} ");
    //            }
    //            Console.WriteLine();
    //        }

            return wordCount;
		}
	}
}