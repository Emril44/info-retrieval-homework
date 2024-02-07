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
			foreach (var term in invertedIndex)
			{
				Console.Write($"{term.Key}: ");
				foreach (var docID in term.Value)
				{
					Console.Write($"{docID} ");
				}
				Console.WriteLine();
			}

			//bool[,] incidencematrix = new bool[alltext.count, termslist.count];

			//for (int i = 0; i < alltext.count; i++)
			//{
			//	string text = alltext[i];
			//	string[] words = text.split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, stringsplitoptions.removeemptyentries);

			//	foreach (string word in words)
			//	{
			//		string cleanword = word.tolower();
			//		int termindex = termslist.indexof(cleanword);
			//		incidencematrix[i, termindex] = true;
			//	}
			//}

			return wordCount;
		}
	}
}