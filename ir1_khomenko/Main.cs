using ir1_khomenko;
using System.Diagnostics;


// Reading files (IMPORTANT: files for reading located in folder "novels" in Documents to save space; change this & path in FileReader.cs (line 23) if needed
string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
FileReader fileReader = new FileReader(baseDir);
List<string> files = fileReader.ReadTextFiles();

// Creating the dictionary
DictionaryBuilder dictionary = new();
Trie trie = dictionary.BuildTrie(files);
Dictionary<string, List<int>> wordCount = dictionary.BuildTrigramIndex(files);
Dictionary<string, List<string>> perms = dictionary.BuildPermutationIndex(files);

// Saving to files
string dictionaryFilePathJson = Path.Combine(baseDir, "dictionary.json");
string dictionaryFilePathTxt = Path.Combine(baseDir, "dictionary.txt");

DictionaryProcessor dictionaryProcessor = new DictionaryProcessor();



static void PrintResults(HashSet<int> results)
{
    foreach(int result in results) Console.WriteLine($"Document ID: {result}");
}