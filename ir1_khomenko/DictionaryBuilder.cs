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
			Dictionary<string, int> wordCount = new Dictionary<string, int>();

			foreach (string text in allText)
			{
				string[] words = text.Split(new char[] {' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”'}, StringSplitOptions.RemoveEmptyEntries);

				foreach (string word in words)
				{
					string cleanWord = word.ToLower();

					if (wordCount.ContainsKey(cleanWord))
					{
						wordCount[cleanWord]++;
					}
					else
					{
						wordCount[cleanWord] = 1;
					}
				}
			}

			return wordCount;
		}
	}
}