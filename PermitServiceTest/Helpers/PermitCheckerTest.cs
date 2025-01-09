using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using PermitService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitServiceTest.Helpers
{
    [TestFixture]
    public class PermitCheckerTest
    {
        private Mock<IWebDriver> _webDriverMock = null!;
        
        [SetUp]
        public void TestSetUp()
        {
            _webDriverMock = new Mock<IWebDriver>();
            var nextStepLinkStubStub = new Mock<IWebElement>();
            _webDriverMock.Setup(x => x.FindElement(By.Id("Button1"))).Returns(nextStepLinkStubStub.Object);
        }

        [Test]
        public void GetAvailableDays_NoAvailableDays_DictionaryIsEmpty()
        {
            var currMonthCallendar = new StringBuilder();
            currMonthCallendar.Append("<html><table><tbody>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <table class='messes'><tbody><tr>");
            currMonthCallendar.Append("         <td align='center'>mayo de 2025</td>");    
            currMonthCallendar.Append("     </tr></tbody></table>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <td class='dias' style='background-color:Gray;'>");
            currMonthCallendar.Append("         <a href='' style='color:Black' title='5 de mayo'>5</a>");
            currMonthCallendar.Append("     </td>");
            currMonthCallendar.Append("     <td class='dias' style='background-color:Black;'>");
            currMonthCallendar.Append("         <a href='' style='color:Black' title='6 de mayo'>6</a>");
            currMonthCallendar.Append("     </td>");
            currMonthCallendar.Append(" </tr>");        
            currMonthCallendar.Append("</tbody></table></html>");

            _webDriverMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());

            var availableDays = new PermitChecker(_webDriverMock.Object).GetAvailableDays(Month.May);
            Assert.That(availableDays.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCase(5, TestName = "GetAvailableDays_OneAvailableDayOnFifthOfMay_DictonaryHasOneEntry")]
        [TestCase(13, TestName = "GetAvailableDays_OneAvailableDayOnThirteenthOfMay_DictonaryHasOneEntry")]
        public void GetAvailableDays_OneAvailableDayInMay_DictonaryHasOneEntry(int dayNumber)
        {
            var currMonthCallendar = new StringBuilder();
            currMonthCallendar.Append("<html><table><tbody>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <table class='messes'><tbody><tr>");
            currMonthCallendar.Append("         <td align='center'>mayo de 2025</td>");
            currMonthCallendar.Append("     </tr></tbody></table>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <td class='dias' style='background-color:WhiteSmoke;'>");
            currMonthCallendar.Append($"        <a href='' style='color:Black' title='{dayNumber} de mayo'>{dayNumber}</a>");
            currMonthCallendar.Append("     </td>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append(" </tbody>");
            currMonthCallendar.Append("</tbody></table></html>");

            _webDriverMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());
            
            var availableDays = new PermitChecker(_webDriverMock.Object).GetAvailableDays(Month.May);

            Assert.That(availableDays.ContainsKey(Month.May) , Is.True);
            Assert.That(availableDays[Month.May].Count, Is.EqualTo(1));
            Assert.That(availableDays[Month.May].First, Is.EqualTo(dayNumber));
        }

        //check 28 29 February
        //Check 30 April
        //diffrent date single days available test
        //style vs bgcolor attribute
        //diffrent months check
        //multiple in the same month check
        //optimize to check only given months
        //webelement Button1 not foound
        //test wait.Until
        //tes calendar script failed
        //no available days for 12 months
        //one day available - others are not available
        //current month detection on website
        //no page content
        //tdHtmlNode not found
        //month cannot be checked td not found
        //html document is null

    }
}
