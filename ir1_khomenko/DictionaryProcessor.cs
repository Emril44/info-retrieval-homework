using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ir1_khomenko
{
    class DictionaryProcessor
    {
        public void SaveDictionaryJSON(Dictionary<string, List<int>> wordCount, string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(wordCount);

                File.WriteAllText(filePath, json);
                Console.WriteLine($"Dictionary saved to {filePath}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public Dictionary<string, int> ReadDictionaryJSON(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Dictionary<string, int>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public Dictionary<string, int> ReadDictionaryTXT(string filePath)
        {
            try
            {
                Dictionary<string, int> dictionary = new();
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if(parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        int value = int.Parse(parts[1].Trim());
                        dictionary[key] = value;
                    }
                }
                return dictionary;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public void SaveDictionaryTXT(Dictionary<string, int> wordCount, string filePath)
        {
            try
            {
                using(StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach(var pair in wordCount)
                    {
                        writer.WriteLine($"{pair.Key}: {pair.Value}");
                    }
                }
                Console.WriteLine($"Dictionary saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public long CalculateDictionarySize(Dictionary<string, int> wordCount)
        {
            long size = wordCount.Sum(pair => pair.Key.Length * sizeof(char) + sizeof(int));
            return (size / 1024);
        }

        public int CalculateTotalWordCount(Dictionary<string, int> wordCount)
        {
            int totalWordCount = wordCount.Values.Sum();
            return totalWordCount;
        }

        public int CalculateFileNum(List<string> allText)
        {
            int fileNum = allText.Count();
            return fileNum;
        }

        public long CalculateSpaceUsage(List<string> allText)
        {
            long totalSpaceUsage = allText.Sum(text => Encoding.UTF8.GetByteCount(text));
            return (totalSpaceUsage / 1024);
        }
    }
}
