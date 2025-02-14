using PermitService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Sources
{
    public class CsvFileManager(ILog4NetAdapter logger, IFileProvider fileProvider, IDateTimeService dateTimeService, char fieldDelimeter)
    {
        public async Task<IEnumerable<PermitRequestData>> ReadInputDataAsync(string inputFilePath)
        {
            try
            {
                Action<string> deleteInputFile = (inputFilePath) => { fileProvider.DeleteFile(inputFilePath); };
                return await ReadPermitRequestDataFromCsvFileAsync(inputFilePath, deleteInputFile);
            }
            catch (IOException)
            {
                logger.Warning("Input file cannot be opened or removed. Data in this file will be not consumed.");
            }

            return [];
        }

        public async Task<IEnumerable<PermitRequestData>> ReadSavedDataAsync(string inputFilePath)
        {
            try
            {
                Action<string> dontDeleteFile = (inputFilePath) => { };
                return await ReadPermitRequestDataFromCsvFileAsync(inputFilePath, dontDeleteFile);
            }
            catch (IOException)
            {
                logger.Warning("Saved file cannot be opended. Data saved in this file will be not read.");
            }

            return [];
        }

        private async Task<IEnumerable<PermitRequestData>> ReadPermitRequestDataFromCsvFileAsync(string inputFilePath, Action<string> deleteAction)
        {
            if (fileProvider.FileExists(inputFilePath))
            {
                var permitRequestData = await ReadPermitRequestData(inputFilePath);
               
                deleteAction(inputFilePath);
                return permitRequestData.Where(x => IsPermitRequestDataValid(x));
            }

            return [];
        }

        private bool IsPermitRequestDataValid(PermitRequestData permitRequestData)
        {
            try
            {
                if (!IsStartDateBiggerThanEndDate(permitRequestData) && !DatePeriodSpansOverTwelveMonths(permitRequestData))
                {
                    permitRequestData.AdjustStartDateToCurrentDate(dateTimeService);
                    return true;
                }
            }
            catch (InvalidOperationException) { }

            return false;
        }

        private async Task<IEnumerable<PermitRequestData>> ReadPermitRequestData(string inputFilePath)
        {
            var result = new List<PermitRequestData>();
            var csvRawData = await fileProvider.ReadLines(inputFilePath);
            foreach(var line in csvRawData)
            {
                try
                {
                    result.Add(PermitRequestData.FromCsvString(line, fieldDelimeter));
                }
                catch (InvalidOperationException ex)
                {
                    logger.Error(ex.Message);
                }
            }

            return result;
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
