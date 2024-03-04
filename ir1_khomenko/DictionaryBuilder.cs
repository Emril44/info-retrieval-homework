using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
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
        private DictionaryProcessor dictionaryProcessor;
        public Dictionary<string, List<int>> InvertedIndex { get; private set; }
        private Dictionary<string, List<int>> CurrentBlock;
        private const long MaxBlockSize = 17179869184; // 16 GB in bytes

        public DictionaryBuilder()
        {
            _trie = new Trie();
            PermutationIndex = new Dictionary<string, List<string>>();
            TrigramIndex = new Dictionary<string, List<int>>();

            dictionaryProcessor = new DictionaryProcessor();
            CurrentBlock = new Dictionary<string, List<int>>();
            InvertedIndex = new Dictionary<string, List<int>>();
        }

        public void BuildInvertedIndex(string[] fileNames, string outputPath)
        {
            long currentBlockSize = 0;

            string pattern = @"\b\w+\b";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);

            for (int documentID = 0; documentID < fileNames.Length; documentID++)
            {
                string text = fileNames[documentID];

                MatchCollection matches = regex.Matches(text);

                foreach (Match match in matches)
                {
                    string cleanWord = match.Value.ToLower();

                    if (!CurrentBlock.ContainsKey(cleanWord))
                    {
                        CurrentBlock[cleanWord] = new List<int> { documentID };
                    }
                    else
                    {
                        CurrentBlock[cleanWord].Add(documentID);
                    }

                    if (!InvertedIndex.ContainsKey(cleanWord))
                    {
                        InvertedIndex[cleanWord] = new List<int> { documentID };
                    }
                    else
                    {
                        InvertedIndex[cleanWord].Add(documentID);
                    }

                    int wordSize = cleanWord.Length;
                    int postingsListSize = sizeof(int) * InvertedIndex[cleanWord].Count;
                    int totalSize = wordSize + postingsListSize;
                    currentBlockSize += totalSize;

                    if (currentBlockSize > MaxBlockSize)
                    {
                        string blockFilePathJson = Path.Combine(outputPath, $"block_{DateTime.Now.Ticks}.json");
                        dictionaryProcessor.SaveDictionaryJSON(CurrentBlock, blockFilePathJson);

                        CurrentBlock.Clear();
                        currentBlockSize = 0;
                    }
                }
            }

            if(CurrentBlock.Count > 0)
            {
                string blockFilePathJson = Path.Combine(outputPath, $"block_{DateTime.Now.Ticks}.json");
                dictionaryProcessor.SaveDictionaryJSON(CurrentBlock, blockFilePathJson);

                CurrentBlock.Clear();
            }

            string finalBlockFilePathJson = Path.Combine(outputPath, $"final_block_{DateTime.Now.Ticks}.json");
            dictionaryProcessor.SaveDictionaryJSON(InvertedIndex, finalBlockFilePathJson);

            InvertedIndex.Clear();
        }

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

            Dictionary<string, HashSet<int>> PermutationIndex = new Dictionary<string, HashSet<int>>();

            foreach (string text in allText)
            {
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '—', '-', '(', ')', '[', ']', '{', '}', '<', '>', '\"', '\'', '\\', '/', '!', '?', '|', '_', '+', '=', '*', '&', '%', '$', '#', '@', '^', '~', '`', '“', '"', '”' }, StringSplitOptions.RemoveEmptyEntries);

                foreach(string word in words)
                {
                    List<string> permutations = PermutationGenerator.GeneratePermutations(word.ToLower());

                    foreach(string permutation in permutations)
                    {
                        if(!PermutationIndex.ContainsKey(permutation))
                        {
                            PermutationIndex[permutation] = new HashSet<int>();
                        }
                        PermutationIndex[permutation].Add(documentID);
                    }

                    documentID++;
                }
            }

            return PermutationIndex;
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