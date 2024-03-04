using ir1_khomenko;
using System.Diagnostics;


// Reading files (IMPORTANT: files for reading located in folder "novels" in Documents to save space; change this & path in FileReader.cs (line 23) if needed
string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
FileReader fileReader = new FileReader(Path.Combine(baseDir, "gutenberg"));
List<string> files = fileReader.ReadTextFiles();

// Creating the dictionary
DictionaryBuilder dictionary = new();
string[] fileNames = files.ToArray();

// Saving to files
string invertedIndexPath = Path.Combine(baseDir, "inverted_index.json");
dictionary.BuildInvertedIndex(fileNames, invertedIndexPath);