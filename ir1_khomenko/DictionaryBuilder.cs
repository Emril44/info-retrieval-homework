using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
    /*
     TODO:
     Implement SPIMI
     */

    class DictionaryBuilder
    {
        private Trie _trie;

        public Dictionary<string, List<string>> PermutationIndex { get; private set; }
        public Dictionary<string, List<int>> TrigramIndex { get; private set; }
     
        public Trie BuildTrie(List<string> allText)
		{
            _trie = new Trie();

            foreach (string text in allText)
            {
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < words.Length - 1; i++)
                {
                    string cleanWord = words[i].ToLower();

                    _trie.Insert(cleanWord);
                }
            }
    
            return _trie;
		}

        public Dictionary<string, HashSet<int>> BuildPermutationIndex(List<string> allText)
        {
            int documentID = 0;

            Dictionary<string, HashSet<int>> permutationIndex = new Dictionary<string, HashSet<int>>();

            foreach (string text in allText)
            {
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

                foreach(string word in words)
                {
                    List<string> permutations = PermutationGenerator.GeneratePermutations(word.ToLower());

                    foreach(string permutation in permutations)
                    {
                        if(!permutationIndex.ContainsKey(permutation))
                        {
                            permutationIndex[permutation] = new HashSet<int>();
                        }
                        permutationIndex[permutation].Add(documentID);
                    }

                    documentID++;
                }
            }

            return permutationIndex;
        }
        public Dictionary<string, List<int>> BuildTrigramIndex(List<string> allText)
        {
            Dictionary<string, List<int>> trigramIndex = new Dictionary<string, List<int>>();

            int documentID = 0;

            foreach(string text in allText)
            {
                documentID++;

                for(int i = 0; i < text.Length - 2; i++)
                {
                    string trigram = text.Substring(i, 3).ToLower();

                    if(!trigramIndex.ContainsKey(trigram))
                    {
                        trigramIndex[trigram] = new List<int>();
                    }

                    if (!trigramIndex[trigram].Contains(documentID))
                    {
                        trigramIndex[trigram].Add(documentID);
                    }
                }
            }

            return trigramIndex;
        }
	}
}