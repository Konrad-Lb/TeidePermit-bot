using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Helpers
{
    public class FileProvider : IFileProvider
    {
        public async Task<IEnumerable<string>> ReadLines(string filePath)
        {
            var result = new List<string>();
            using var streamReader = new StreamReader(filePath);
            string? line;
            while ((line = await streamReader.ReadLineAsync()) != null)
                result.Add(line);

            return result;

        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public async Task WriteLines(string filePath, IEnumerable<string> lines)
        { 
            using var streamWriter = new StreamWriter(filePath);
            foreach(var line in lines)
                await  streamWriter.WriteLineAsync(line);
        }
    }
}
