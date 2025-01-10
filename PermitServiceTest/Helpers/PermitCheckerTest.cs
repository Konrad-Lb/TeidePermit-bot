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
        private Mock<IWebElement> _nextStepLinkMock = null!;
        private Mock<IWebElement> _nextMonthLinkMock = null!;

        [SetUp]
        public void TestSetUp()
        {
            _webDriverMock = new Mock<IWebDriver>();
            _nextStepLinkMock = new Mock<IWebElement>();
            _webDriverMock.Setup(x => x.FindElement(By.Id("Button1"))).Returns(_nextStepLinkMock.Object);

            _nextMonthLinkMock = new Mock<IWebElement>();
            _webDriverMock.Setup(x => x.FindElement(By.CssSelector("a[title='Ir al mes siguiente.']"))).Returns(_nextMonthLinkMock.Object);
        }

        [TearDown]
        public void TestTearDown()
        {
            _nextStepLinkMock.Verify(x => x.Click(), Times.Once);
        }

        [Test]
        public void GetAvailableDays_NoAvailableDays_DictionaryIsEmpty()
        {
            var currMonthCallendar = GenerateHtmlTwoDaysMonthCallendar("mayo", "Gray", "Black");
            _webDriverMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());

            var availableDays = new PermitChecker(_webDriverMock.Object).GetAvailableDays([Month.May]);

            _nextMonthLinkMock.Verify(x => x.Click(), Times.Never);
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
            
            var availableDays = new PermitChecker(_webDriverMock.Object).GetAvailableDays([Month.May]);

            _nextMonthLinkMock.Verify(x => x.Click(), Times.Never);
            Assert.That(availableDays.ContainsKey(Month.May) , Is.True);
            Assert.That(availableDays[Month.May].Count, Is.EqualTo(1));
            Assert.That(availableDays[Month.May].First, Is.EqualTo(dayNumber));
        }

        [Test]
        public void GetAvailableDays_PermitIsAvailableButForDiffrentMonthThenExpected_DictionaryIsEmpty()
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
            currMonthCallendar.Append("         <a href='' style='color:Black' title='5 de mayo'>5</a>");
            currMonthCallendar.Append("     </td>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append("</tbody></table></html>");

            _webDriverMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());

            var availableDays = new PermitChecker(_webDriverMock.Object).GetAvailableDays([Month.February]);

            _nextMonthLinkMock.Verify(x => x.Click(), Times.Never);
            Assert.That(availableDays.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetAvailableDays_CheckMultipleMonths_DictionaryFilledProperly()
        {

            _webDriverMock.SetupSequence(x => x.PageSource)
                 .Returns(() => { return GenerateHtmlTwoDaysMonthCallendar("enero", "Black", "WhiteSmoke"); })
                 .Returns(() => { return GenerateHtmlTwoDaysMonthCallendar("mayo", "Gray", "Gray"); })
                 .Returns(() => { return GenerateHtmlTwoDaysMonthCallendar("agosto", "WhiteSmoke", "WhiteSmoke"); });

            var availableDays = new PermitChecker(_webDriverMock.Object).GetAvailableDays([Month.August, Month.May, Month.January]);

            _nextMonthLinkMock.Verify(x => x.Click(), Times.Exactly(2));
            Assert.That(availableDays.Count, Is.EqualTo(2));
            Assert.That(availableDays[Month.January].Count, Is.EqualTo(1));
            Assert.That(availableDays[Month.January].First, Is.EqualTo(6));
            Assert.That(availableDays[Month.August].Count, Is.EqualTo(2));
            Assert.That(availableDays[Month.August], Is.EqualTo([5, 6]));
        }

        private string GenerateHtmlTwoDaysMonthCallendar(string monthName, string firstDayColorName, string secondDayColorName)
        {
            var montCalendar = new StringBuilder();
            montCalendar.Append("<html><table><tbody>");
            montCalendar.Append(" <tr>");
            montCalendar.Append("     <table class='messes'><tbody><tr>");
            montCalendar.Append($"         <td align='center'>{monthName} de 2025</td>");
            montCalendar.Append("     </tr></tbody></table>");
            montCalendar.Append(" </tr>");
            montCalendar.Append(" <tr>");
            montCalendar.Append($"     <td class='dias' style='background-color:{firstDayColorName};'>");
            montCalendar.Append($"         <a href='' style='color:Black' title='5 de {monthName}'>5</a>");
            montCalendar.Append("     </td>");
            montCalendar.Append(" </tr>");
            montCalendar.Append(" <tr>");
            montCalendar.Append($"     <td class='dias' style='background-color:{secondDayColorName};'>");
            montCalendar.Append($"         <a href='' style='color:Black' title='6 de {monthName}'>6</a>");
            montCalendar.Append("     </td>");
            montCalendar.Append(" </tr>");
            montCalendar.Append("</tbody></table></html>");

            return montCalendar.ToString();
        }

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
        //handle if months for checking are uniqe
        //test handling "Cannot get currently displayed month. Website seems to have incorrect format."

    }
}
