﻿using System;
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
        public Dictionary<string, HashSet<int>> InvertedIndex { get; private set; }
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

        public Dictionary<string, int> BuildDictionary(List<string> allText)
        {
            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            InvertedIndex = new Dictionary<string, HashSet<int>>();
            List<string> termsList = new List<string>();

            string pattern = @"\b\w+\b";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);

            for (int documentID = 0; documentID < allText.Count; documentID++)
            {
                string text = allText[documentID];

                MatchCollection matches = regex.Matches(text);

                foreach (Match match in matches)
                {
                    string cleanWord = match.Value.ToLower();

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

                    InvertedIndex[cleanWord].Add(documentID);
                }
            }

            return wordCount;
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