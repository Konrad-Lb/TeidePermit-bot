using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Sources
{
    public class TeideWebPageClawler(IWebDriver webDriver) : ITeideWebPageClawler
    {
        public void ClickNextStepLink()
        {
            try
            {
                var nextStepLink = webDriver.FindElement(By.Id("Button1"));
                nextStepLink.Click();
            }
            catch (WebDriverException e)
            {
                throw new InvalidOperationException("Permit website has invalid html content. Cannot click on the 'Next Step >>'. For more details see inner exception.", e);
            }
        }

        public void ClickNextMonthLink()
        {
            try
            {
                var nextMonthLink = webDriver.FindElement(By.CssSelector("a[title='Ir al mes siguiente.']"));
                nextMonthLink.Click();
            }
            catch (WebDriverException e)
            {
                throw new InvalidOperationException("Permit website has invalid html content. Cannot click on the next month link'. For more details see inner exception.", e);
            }
        }

        public string PageSource { get => webDriver.PageSource; }

    }
}
