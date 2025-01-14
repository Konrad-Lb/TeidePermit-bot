using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Helpers
{
    public class FileProvider : IFileProvider
    {
        public async Task<List<string>> ReadLines(string filePath)
        {
            var result = new List<string>();
            using var streamReader = new StreamReader(filePath);
            string? line;
            while ((line = await streamReader.ReadLineAsync()) != null)
                result.Add(line);

            return result;

        }
    }
}
