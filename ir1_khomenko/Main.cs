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

// Calculating dictionary data
long dictionarySize = dictionaryProcessor.CalculateDictionarySize(wordCount);
int totalWordCount = dictionaryProcessor.CalculateTotalWordCount(wordCount);
int fileNum = dictionaryProcessor.CalculateFileNum(files);
long spaceUsage = dictionaryProcessor.CalculateSpaceUsage(files);

// File size for Json
FileInfo jsonFileInfo = new FileInfo(dictionaryFilePathJson);
long jsonFileSize = (jsonFileInfo.Length / 1024);

// File size for txt
FileInfo txtFileInfo = new FileInfo(dictionaryFilePathTxt);
long txtFileSize = (txtFileInfo.Length / 1024);

// Output
Console.WriteLine("---------------------------------------------");
Console.WriteLine($"Dictionary Size: {dictionarySize} KB ({dictionarySize / 1024} MB)");
Console.WriteLine($"Total Word Count: {totalWordCount}");
Console.WriteLine($"Number of Files: {fileNum}");
Console.WriteLine($"Space Usage: {spaceUsage} KB ({spaceUsage / 1024} MB)");

Console.WriteLine("---------------------------------------------");
Console.WriteLine($"Size of JSON file: {jsonFileSize} KB ({jsonFileSize / 1024} MB)");
Console.WriteLine($"Time to serialize and save to JSON: {jsonParsingTime}");
Console.WriteLine($"Size of TXT file: {txtFileSize} Kb ({txtFileSize / 1024} MB)");
Console.WriteLine($"Time to save to TXT: {txtParsingTime}");

Console.WriteLine("---------------------------------------------");
SearchEngine search = new(dictionary.InvertedIndex, null);

string query = "diagram AND corrugated";
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
//search = new(null, dictionary.IncidenceMatrix);

//string query = "diagram AND corrugated";
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