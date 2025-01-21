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
                    return permitRequestData.Where(x => !IsStartDateBiggerThanEndDate(x) && !DatePeriodSpansOverTwelveMonths(x));
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

        private bool IsStartDateBiggerThanEndDate(PermitRequestData permitRequestData)
        {
            if (permitRequestData.IsStartDateBiggerThanEndDate())
            {
                logger.Warning($"Entry in input file {{StartDate = {permitRequestData.StartDate:yyyy-MM-dd}, EndDate = {permitRequestData.EndDate:yyyy-MM-dd} EmailAddress = {permitRequestData.EmailAddress}}} has bigger StartDate then EndDate. It will be ignored");
                return true;
            }

            return false;
        }

        private bool DatePeriodSpansOverTwelveMonths(PermitRequestData permitRequestData)
        {
            if(permitRequestData.DatePeriodSpansOverTwelveMonths())
            {
                logger.Warning($"Entry in input file {{StartDate = {permitRequestData.StartDate:yyyy-MM-dd}, EndDate = {permitRequestData.EndDate:yyyy-MM-dd} EmailAddress = {permitRequestData.EmailAddress}}} spans more than 12 calendar months. It will be ignored.");
                return true;
            }

            return false;
        }

        public async Task SavePermitRequestData(string saveFilePath, List<PermitRequestData> permitRequestDataList)
        {
            if(permitRequestDataList.Count > 0)
            {
                DeleteSaveFileIfExists(saveFilePath);

                var csvStringList = permitRequestDataList.Select(x => x.ToCsvString(fieldDelimeter));
                await fileProvider.WriteLines(saveFilePath, csvStringList);
            }
           
        }

        private void DeleteSaveFileIfExists(string saveFilePath)
        {
            if (fileProvider.FileExists(saveFilePath))
                fileProvider.DeleteFile(saveFilePath);
        }
    }
}
