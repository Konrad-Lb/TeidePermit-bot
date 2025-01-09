using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V129.HeapProfiler;
using OpenQA.Selenium.DevTools.V129.Input;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics.Eventing.Reader;

namespace PermitService.Helpers
{
    public class PermitChecker(IWebDriver webDriver)
    {
        private Month _currentMonth;
        private HtmlDocument? _htmlDocumemt;
        
        public IDictionary<Month,List<int>> GetAvailableDays(Month month)
        {
            _currentMonth = month;
            
            ClickNextStepLink();

            _htmlDocumemt = new HtmlDocument();
            _htmlDocumemt.LoadHtml(webDriver.PageSource);

            return GetPermits();
        }

        private void ClickNextStepLink()
        {
            var nextStepLink = webDriver.FindElement(By.Id("Button1"));
            nextStepLink.Click();
        }

        private bool PermitsForMonthShouldBeChecked()
        {
            var tdMessesCollection = _htmlDocumemt?.DocumentNode.SelectNodes("//table[@class='messes']//td[@align='center']");
            return tdMessesCollection != null && tdMessesCollection.First().InnerHtml.Contains(SpanishMonthTranslator.GetTranslationLowerCase(_currentMonth));
        }

        private List<int> GetAvailableDaysForMonth()
        {
            var result = new List<int>();
            var tdDiasCollection = _htmlDocumemt?.DocumentNode.SelectNodes("//td[@class=\"dias\"]");
            if (tdDiasCollection != null)
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
            var styleAttribute = callendarDayCell.Attributes["style"] ?? callendarDayCell.Attributes["bgcolor"];
            if (styleAttribute.Value != null && styleAttribute.Value.Contains("WhiteSmoke"))
                return Int32.Parse(callendarDayCell.ChildNodes.Where(x => x.Name == "a").First().InnerText);
            
            return null;
        }

        private IDictionary<Month, List<int>> GetPermits()
        {
            var result = new Dictionary<Month, List<int>>();
            if (PermitsForMonthShouldBeChecked())
            {
                var availableDays = GetAvailableDaysForMonth();
                if (availableDays.Count > 0)
                    result[_currentMonth] = availableDays;
            }

            return result;
        }
    }
}