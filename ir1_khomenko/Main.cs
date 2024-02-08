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

stopwatch.Start();
dictionaryProcessor.SaveDictionaryJSON(wordCount, dictionaryFilePathJson);
stopwatch.Stop();
TimeSpan jsonParsingTime = stopwatch.Elapsed;

// Timing how quick txt saves
stopwatch.Reset();

stopwatch.Start();
dictionaryProcessor.SaveDictionaryTXT(wordCount, dictionaryFilePathTxt);
stopwatch.Stop();
TimeSpan txtParsingTime = stopwatch.Elapsed;


Console.WriteLine("---------------------------------------------");
// Query for both searches
string query = "diagram AND corrugated OR philadelphia";

Console.WriteLine("INVERTED INDEX");
SearchEngine indexSearch = new(dictionary.InvertedIndex, null, null);

stopwatch.Reset();
stopwatch.Start();
HashSet<int> indexSearchResults = indexSearch.BooleanSearch(query);
stopwatch.Stop();

Console.WriteLine($"Search results for {query}: ");
foreach (int result in indexSearchResults)
{
    Console.WriteLine($"Document ID: {result}");
}
Console.WriteLine($"Search time: {stopwatch.ElapsedMilliseconds}ms");

Console.WriteLine("---------------------------------------------");
Console.WriteLine("INCIDENCE MATRIX");
Dictionary<string, int> termIndexMap = dictionary.GetTermIndexMap();
SearchEngine matrixSearch = new(null, dictionary.IncidenceMatrix, termIndexMap);

stopwatch.Reset();
stopwatch.Start();
HashSet<int> searchResults = matrixSearch.BooleanSearch(query);
stopwatch.Stop();

Console.WriteLine($"Search results for {query}: ");
foreach (int result in searchResults)
{
    Console.WriteLine($"Document ID: {result}");
}
Console.WriteLine($"Search time: {stopwatch.ElapsedMilliseconds}ms");
