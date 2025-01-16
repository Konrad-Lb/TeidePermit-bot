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
                    var permitRequestData = await ReadPermitRequestData(inputFilePath);
                    fileProvider.DeleteFile(inputFilePath);
                    return WhereStartDateTimeIsLessOrEqualEndDateTime(permitRequestData);
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

        private IEnumerable<PermitRequestData> WhereStartDateTimeIsLessOrEqualEndDateTime(IEnumerable<PermitRequestData> data)
        {
            return data.Where(x =>
            {
                if (x.StartDate > x.EndDate)
                {
                    logger.Warning($"Entry in input file {{StartDate = {x.StartDate.ToString("yyyy-MM-dd")}, EndDate = {x.EndDate.ToString("yyyy-MM-dd")} EmailAddress = {x.EmailAddress}}} has bigger StartDate then EndDate. It will be ignored");
                    return false;
                }
                return true;
            });
        }
    }
}
