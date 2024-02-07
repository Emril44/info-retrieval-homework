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
SearchEngine search = new(dictionary.InvertedIndex, null);

string query = "diagram AND corrugated OR crosstalk";
stopwatch.Reset();
stopwatch.Start();
HashSet<int> searchResults = search.BooleanSearch(query);
stopwatch.Stop();

Console.WriteLine($"Search results for {query}: ");
foreach (int result in searchResults)
{
    Console.WriteLine($"Document ID: {result}");
}
Console.WriteLine($"Search time: {stopwatch.ElapsedMilliseconds}ms");

//Console.WriteLine("---------------------------------------------");
//SearchEngine search = new(null, dictionary.IncidenceMatrix);

//string query = "diagram AND corrugated NOT crosstalk";
//stopwatch.Reset();
//stopwatch.Start();
//HashSet<int> searchResults = search.BooleanSearch(query);
//stopwatch.Stop();

//Console.WriteLine($"Search results for {query}: ");
//foreach (int result in searchResults)
//{
//    Console.WriteLine($"Document ID: {result}");
//}
//Console.WriteLine($"Search time: {stopwatch.ElapsedMilliseconds}ms");

//Console.WriteLine("---------------------------------------------");
//Dictionary<string, int> txtDictionary = dictionaryProcessor.ReadDictionaryTXT(dictionaryFilePathTxt);
//foreach (var pair in txtDictionary)
//{
//    Console.WriteLine($"{pair.Key}: {pair.Value}");
//}

//Console.WriteLine("---------------------------------------------");
//Dictionary<string, int> jsonDictionary = dictionaryProcessor.ReadDictionaryJSON(dictionaryFilePathJson);
//foreach (var pair in jsonDictionary)
//{
//    Console.WriteLine($"{pair.Key}: {pair.Value}");
//}