using PermitService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Sources
{
    public class CsvFileManager(IFileProvider fileProvider, char fieldDelimeter)
    {
        private readonly char _fieldDelimeter = fieldDelimeter;


        public async Task<List<PermitRequestData>> ReadInputDataAsync(string inputFilePath)
        {
            var result = new List<PermitRequestData>();
            var inputFileData = await fileProvider.ReadLines(inputFilePath);
            foreach(var inputDataLine in inputFileData)
            {
                var currentLine = inputDataLine;
                result.Add(PermitRequestData.FromCsvString(currentLine,_fieldDelimeter));
            }

            return result;
        }
    }
}
