using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
    /*
     TODO:
     1. Побудувати пряме або обернене дерево термінів словника
     2. Побудувати перестановочний індекс для словника
     3. Побудувати 3-грамний індекс для словника
     */

    class DictionaryBuilder
    {
		public Trie BuildTrie(List<string> allText)
		{
            Trie trie = new Trie();

            foreach (string text in allText)
            {
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < words.Length - 1; i++)
                {
                    string cleanWord = words[i].ToLower();

                    trie.Insert(cleanWord);
                }
            }
    
            return trie;
		}
	}
}