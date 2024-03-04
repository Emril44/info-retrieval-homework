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

            ProcessDirectory(baseDirectory, fileContents);

            return fileContents;
        }

        private void ProcessDirectory(string targetDirectory, List<string> fileContents)
        {
            try
            {
                string[] fileEntries = Directory.GetFiles(baseDirectory, "*.txt");
                foreach (string fileName in fileEntries)
                {
                    string content = ProcessFile(fileName);
                    fileContents.Add(content);
                }

                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach(string subdirectoryEntry in subdirectoryEntries)
                {
                    ProcessDirectory(subdirectoryEntry, fileContents);
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"Error processing directory: {ex.Message}");
            }
        }

        private string ProcessFile(string fileName)
        {
            try
            {
                string fileContent = File.ReadAllText(fileName);
                return fileContent;
            } catch (Exception ex)
            {
                Console.WriteLine($"Error processing file: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
