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

        public async Task<List<string>> ReadTextFilesAsync()
        {
            List<string> fileContents = new List<string>();

            await ProcessDirectoryAsync(baseDirectory, fileContents);

            return fileContents;
        }

        private async Task ProcessDirectoryAsync(string targetDirectory, List<string> fileContents)
        {
            try
            {
                foreach (string fileName in Directory.EnumerateFiles(targetDirectory, "*.txt"))
                {
                    string content = await ProcessFileAsync(fileName);
                    fileContents.Add(content);
                }

                foreach(string subdirectoryEntry in Directory.EnumerateDirectories(targetDirectory))
                {
                    await ProcessDirectoryAsync(subdirectoryEntry, fileContents);
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"Error processing directory: {ex.Message}");
            }
        }

        private async Task<string> ProcessFileAsync(string fileName)
        {
            try
            {
                string fileContent = await File.ReadAllTextAsync(fileName);
                return fileContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
