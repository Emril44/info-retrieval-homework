using ir1_khomenko;
using System.Diagnostics;


// Reading files (IMPORTANT: files for reading located in folder "novels" in Documents to save space; change this & path in FileReader.cs (line 23) if needed
string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
FileReader fileReader = new FileReader(baseDir);
List<string> files = fileReader.ReadTextFiles();

// Creating the dictionary
DictionaryBuilder dictionary = new();
Dictionary<string, int> wordCount = dictionary.BuildDictionary(files);

// Saving to files
string dictionaryFilePathJson = Path.Combine(baseDir, "dictionary.json");
string dictionaryFilePathTxt = Path.Combine(baseDir, "dictionary.txt");

DictionaryProcessor dictionaryProcessor = new DictionaryProcessor();

// Timing how quick Json saves
Stopwatch stopwatch = new();

dictionaryProcessor.SaveDictionaryJSON(wordCount, dictionaryFilePathJson);
dictionaryProcessor.SaveDictionaryTXT(wordCount, dictionaryFilePathTxt);

SearchEngine searchEngine = new(
    dictionary.InvertedIndex,
    dictionary.PhrasalIndex,
    dictionary.CoordinateInvertedIndex
    );

//Console.WriteLine("BOOLEAN SEARCH:");
//HashSet<int> boolResults = searchEngine.BooleanSearch("diagram AND corrugated OR philadelphia");
//PrintResults(boolResults);
//Console.WriteLine("--------------------------------\n");

Console.WriteLine("PHRASAL SEARCH:");
HashSet<int> phrasalResults = searchEngine.PhraseSearch("pennywise the clown");
PrintResults(phrasalResults);
Console.WriteLine("--------------------------------\n");

Console.WriteLine("DISTANCE-BASED SEARCH:");
HashSet<int> distanceResults = searchEngine.DistanceBasedSearch("pennywise", "clown", 2);
PrintResults(distanceResults);
Console.WriteLine("--------------------------------\n");

static void PrintResults(HashSet<int> results)
{
    foreach(int result in results) Console.WriteLine($"Document ID: {result}");
}