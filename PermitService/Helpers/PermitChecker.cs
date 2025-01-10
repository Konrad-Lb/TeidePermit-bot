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
        private HtmlDocument? _htmlDocumemt;

        public IDictionary<Month, List<int>> GetAvailableDays(List<Month> months)
        {
            ClickNextStepLink();

            var result = new Dictionary<Month, List<int>>();
            
            for(int i = 0; i < months.Count; i++)
            {
                if (i > 0)
                    ClickNectMonthLink();
                
                LoadHtmlDocument();

                var displayedMonth = GetCurrenlyDisplayedMonth();
                if (months.Any(month => month == displayedMonth))
                {
                    var availableDays = GetAvailableDaysForCurrentlyDisplayedMonth();
                    if (availableDays.Count > 0)
                        result[displayedMonth] = availableDays;
                }
            }

            return result;
        }

        private void LoadHtmlDocument()
        {
            _htmlDocumemt = new HtmlDocument();
            _htmlDocumemt.LoadHtml(webDriver.PageSource);
        }

        private void ClickNextStepLink()
        {
            var nextStepLink = webDriver.FindElement(By.Id("Button1"));
            nextStepLink.Click();
        }

        private void ClickNectMonthLink()
        {
            var nextMonthLink = webDriver.FindElement(By.CssSelector("a[title='Ir al mes siguiente.']"));
            nextMonthLink.Click();
        }

        private Month GetCurrenlyDisplayedMonth()
        {
            var tdMessesCollection = _htmlDocumemt?.DocumentNode.SelectNodes("//table[@class='messes']//td[@align='center']");
            if (tdMessesCollection != null)
            {
                var displayedMonthWithYear = tdMessesCollection.First().InnerHtml;
                var displayedMonthText = displayedMonthWithYear[0..displayedMonthWithYear.IndexOf(' ')];
                return SpanishMonthTranslator.CreateMonthFromSpanishName(displayedMonthText);
            }

            throw new InvalidOperationException("Cannot get currently displayed month. Website seems to have incorrect format.");
        }

        private List<int> GetAvailableDaysForCurrentlyDisplayedMonth()
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

       
    }
}