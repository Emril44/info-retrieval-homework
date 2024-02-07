using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
    internal class FileReader
    {
        private string baseDirectory;
        public FileReader(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
        }

        public List<string> ReadTextFiles()
        {
            List<string> fileContents = new List<string>();
            try
            {
                string directoryPath = Path.Combine(baseDirectory, "novels");

                if(Directory.Exists(directoryPath))
                {
                    string[] entries = Directory.GetFiles(directoryPath, "*.txt");

                    foreach (string entry in entries)
                    {
                        string fileContent = File.ReadAllText(entry);
                        fileContents.Add(fileContent);
                    }
                }
                else
                {
                    Console.WriteLine("Directory not found: " + directoryPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to read file!\n{e}");
            }

            return fileContents;
        }
    }
}
