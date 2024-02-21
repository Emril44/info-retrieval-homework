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
     2. Побудувати перестановочний індекс для словника
     3. Побудувати 3-грамний індекс для словника
     4. Реалізувати підтримку запитів з джокерами
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

        public Dictionary<string, List<string>> BuildPermutationIndex(List<string> allText)
        {
            Dictionary<string, List<string>> permutationIndex = new Dictionary<string, List<string>>();

            foreach(string text in allText)
            {
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

                foreach(string word in words)
                {
                    List<string> permutations = GeneratePermutations(word.ToLower());

                    foreach(string permutation in permutations)
                    {
                        if(!permutationIndex.ContainsKey(permutation))
                        {
                            permutationIndex[permutation] = new List<string>();
                        }
                        permutationIndex[permutation].Add(word);
                    }
                }
            }

            return permutationIndex;
        }

        private List<string> GeneratePermutations(string word)
        {
            List<string> permutations = new List<string>();
            PermutationGenerator(word.ToCharArray(), 0, permutations);
            return permutations;
        }

        private void PermutationGenerator(char[] wordArray, int currentIndex,  List<string> permutations)
        {
            if(currentIndex == wordArray.Length - 1)
            {
                permutations.Add(new string(wordArray));
                Console.WriteLine($"Added permutation: {new string(wordArray)}");
                return;
            }

            for(int i = currentIndex; i < wordArray.Length; i++)
            {
                Swap(ref wordArray[currentIndex], ref wordArray[i]);
                Console.WriteLine($"Swapped characters at indices {currentIndex} and {i}");
                PermutationGenerator(wordArray, currentIndex + 1, permutations);
                Swap(ref wordArray[currentIndex], ref wordArray[i]);
                Console.WriteLine($"Reversed swap of characters at indices {currentIndex} and {i}");
            }
        }

        private void Swap(ref char a, ref char b)
        {
            char temp = a;
            a = b;
            b = temp;
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

            //foreach(var kvp in trigramIndex)
            //{
            //    string trigram = kvp.Key;
            //    List<int> documentIDs = kvp.Value;

            //    Console.WriteLine($"Trigram: {trigram}, Doc IDs: {string.Join(", ", documentIDs)}");
            //}

            return trigramIndex;
        }
	}
}