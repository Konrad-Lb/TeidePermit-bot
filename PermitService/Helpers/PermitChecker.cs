using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics.Eventing.Reader;

namespace PermitService.Helpers
{
    public class PermitChecker(IWebDriver webDriver)
    { 
        public IDictionary<Month,List<int>> GetAvailableDays()
        {
            ClickNextStepLink();

            var result = new Dictionary<Month, List<int>>();
            if(IsPemitAvailable(webDriver.PageSource))
                result.Add(Month.May, [5]);

            return result;
        }

        private void ClickNextStepLink()
        {
            var nextStepLink = webDriver.FindElement(By.Id("Button1"));
            nextStepLink.Click();
        }

        private static bool IsPemitAvailable(string pageContent)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageContent);

            var tdHtmlNode = htmlDoc.DocumentNode.SelectNodes("//a[@title=\"5 de mayo\"]/parent::td").First();
            var styleAttribute = tdHtmlNode.Attributes["style"] ?? tdHtmlNode.Attributes["bgcolor"];

            return styleAttribute.Value != null && styleAttribute.Value.Contains("WhiteSmoke");
        }
    }
}