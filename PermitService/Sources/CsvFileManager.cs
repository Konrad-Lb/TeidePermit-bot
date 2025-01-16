using PermitService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Sources
{
    public class CsvFileManager(ILog4NetAdapter logger, IFileProvider fileProvider, char fieldDelimeter)
    {
        public async Task<IEnumerable<PermitRequestData>> ReadInputDataAsync(string inputFilePath)
        {
            if (fileProvider.FileExists(inputFilePath))
            {
                try
                {
                    var result = await ReadPermitRequestData(inputFilePath);
                    fileProvider.DeleteFile(inputFilePath);
                    return result;
                }
                catch (IOException)
                {
                    logger.Warning("Input file cannot be opened or removed. Data in this file will be not consumed.");
                }
                
            }

            return [];
        }

        private async Task<IEnumerable<PermitRequestData>> ReadPermitRequestData(string inputFilePath)
        {
            var csvRawData = await fileProvider.ReadLines(inputFilePath);
            return csvRawData.Select(csvString => PermitRequestData.FromCsvString(csvString, fieldDelimeter));
        }
    }
}
