using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V129.HeapProfiler;
using OpenQA.Selenium.DevTools.V129.Input;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using PermitService.Sources;
using System.Diagnostics.Eventing.Reader;

namespace PermitService.Helpers
{
    public class PermitChecker(ITeideWebPageClawler webPageCrawler)
    {
        private HtmlDocument _htmlDocumemt = new HtmlDocument();
        private const int MAX_NEXT_MONTHS_CHECK = 11;

        public IDictionary<Month, List<int>> GetAvailableDays(List<Month> monthsToCheck)
        {
            webPageCrawler.ClickNextStepLink();

            var distinctMonthsToCheck = monthsToCheck.Distinct().ToList();
            return GetAvailableDaysForDistinctMonths(distinctMonthsToCheck);
        }

        private Dictionary<Month, List<int>> GetAvailableDaysForDistinctMonths(List<Month> distinctMonthsToCheck)
        {
            var result = new Dictionary<Month, List<int>>();

            for (int i = 0; i < MAX_NEXT_MONTHS_CHECK && distinctMonthsToCheck.Count > 0; i++)
            {
                _htmlDocumemt.LoadHtml(webPageCrawler.PageSource);

                var displayedMonth = GetCurrenlyDisplayedMonth();
                if (distinctMonthsToCheck.Any(month => month == displayedMonth))
                {
                    var availableDays = GetAvailableDaysForCurrentlyDisplayedMonth();
                    if (availableDays.Count > 0)
                        result[displayedMonth] = availableDays;

                    distinctMonthsToCheck.Remove(displayedMonth);
                }

                ClickNextMonthIfAnyMonthToCheckLeft(distinctMonthsToCheck);
            }

            return result;
        }

        private void ClickNextMonthIfAnyMonthToCheckLeft(List<Month> monthsToCheck)
        {
            if (monthsToCheck.Count > 0)
                webPageCrawler.ClickNextMonthLink();
        }

        private Month GetCurrenlyDisplayedMonth()
        {
            var tdMessesCollection = _htmlDocumemt.DocumentNode.SelectNodes("//table[@class='meses']//td[@align='center']") ??
                throw new InvalidOperationException("Cannot get currently displayed month name. Website seems to have incorrect format.");
            
            var displayedMonthWithYear = tdMessesCollection.First().InnerHtml;
            var displayedMonthText = displayedMonthWithYear[0..displayedMonthWithYear.IndexOf(' ')];
            return SpanishMonthTranslator.CreateMonthFromSpanishName(displayedMonthText);
        }

        private List<int> GetAvailableDaysForCurrentlyDisplayedMonth()
        {
            var result = new List<int>();
            var tdDiasCollection = _htmlDocumemt.DocumentNode.SelectNodes("//td[@class=\"dias\"]") ??
                throw new InvalidOperationException("Cannot get information about days in website callendar. Website seems to have incorrect format.");

            foreach (var td in tdDiasCollection)
            {
                var dayNumber = GetDayNumberIfPermitAvailable(td);
                if (dayNumber.HasValue)
                    result.Add(dayNumber.Value);
            }
            return result;
        }

        private static int? GetDayNumberIfPermitAvailable(HtmlNode callendarDayCell)
        {
            var styleAttribute = callendarDayCell.Attributes["style"] ?? callendarDayCell.Attributes["bgcolor"] ??
                throw new InvalidOperationException("Cannot get the day background color style from the website. Website seems to have incorrect style format.");

            if (styleAttribute.Value.Contains("WhiteSmoke"))
                return Int32.Parse(callendarDayCell.ChildNodes.Where(x => x.Name == "a").First().InnerText);

            return null;
        }
    }
}