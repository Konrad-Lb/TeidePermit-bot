﻿using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using PermitService.Helpers;
using PermitService.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitServiceTest.Sources
{
    [TestFixture]
    public class PermitCheckerTest
    {
        private Mock<ITeideWebPageClawler> _webPageCrawlerMock = null!;

        [SetUp]
        public void TestSetUp()
        {
            _webPageCrawlerMock = new Mock<ITeideWebPageClawler>();
        }

        [Test]
        public void GetAvailableDays_NoAvailableDays_DictionaryIsEmpty()
        {
            var currMonthCallendar = GenerateHtmlTwoDaysMonthCallendar("mayo", "Gray", "Black");
            _webPageCrawlerMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());

            var availableDays = new PermitChecker(_webPageCrawlerMock.Object).GetAvailableDays([Month.May]);

            _webPageCrawlerMock.Verify(x => x.ClickNextStepLink(), Times.Once);
            _webPageCrawlerMock.Verify(x => x.ClickNextMonthLink(), Times.Never);
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
            currMonthCallendar.Append("     <table class='meses'><tbody><tr>");
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

            _webPageCrawlerMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());

            var availableDays = new PermitChecker(_webPageCrawlerMock.Object).GetAvailableDays([Month.May]);

            _webPageCrawlerMock.Verify(x => x.ClickNextStepLink(), Times.Once);
            _webPageCrawlerMock.Verify(x => x.ClickNextMonthLink(), Times.Never);
            Assert.That(availableDays.ContainsKey(Month.May), Is.True);
            Assert.That(availableDays[Month.May].Count, Is.EqualTo(1));
            Assert.That(availableDays[Month.May].First, Is.EqualTo(dayNumber));
        }

        [Test]
        public void GetAvailableDays_BgColorStyleInWebsiteIsSupported_DictionaryHasOneEntry()
        {
            var currMonthCallendar = new StringBuilder();
            currMonthCallendar.Append("<html><table><tbody>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <table class='meses'><tbody><tr>");
            currMonthCallendar.Append("         <td align='center'>mayo de 2025</td>");
            currMonthCallendar.Append("     </tr></tbody></table>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <td class='dias' bgcolor='WhiteSmoke'>");
            currMonthCallendar.Append($"        <a href='' style='color:Black' title='5 de mayo'>5</a>");
            currMonthCallendar.Append("     </td>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append(" </tbody>");
            currMonthCallendar.Append("</tbody></table></html>");

            _webPageCrawlerMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());

            var availableDays = new PermitChecker(_webPageCrawlerMock.Object).GetAvailableDays([Month.May]);

            _webPageCrawlerMock.Verify(x => x.ClickNextStepLink(), Times.Once);
            _webPageCrawlerMock.Verify(x => x.ClickNextMonthLink(), Times.Never);
            Assert.That(availableDays.ContainsKey(Month.May), Is.True);
            Assert.That(availableDays[Month.May].Count, Is.EqualTo(1));
            Assert.That(availableDays[Month.May].First, Is.EqualTo(5));
        }


        [Test]
        public void GetAvailableDays_PermitIsAvailableButForDiffrentMonthThenExpected_DictionaryIsEmpty()
        {
            var currMonthCallendar = new StringBuilder();
            currMonthCallendar.Append("<html><table><tbody>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <table class='meses'><tbody><tr>");
            currMonthCallendar.Append("         <td align='center'>mayo de 2025</td>");
            currMonthCallendar.Append("     </tr></tbody></table>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <td class='dias' style='background-color:WhiteSmoke;'>");
            currMonthCallendar.Append("         <a href='' style='color:Black' title='5 de mayo'>5</a>");
            currMonthCallendar.Append("     </td>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append("</tbody></table></html>");

            _webPageCrawlerMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());

            var availableDays = new PermitChecker(_webPageCrawlerMock.Object).GetAvailableDays([Month.February]);

            _webPageCrawlerMock.Verify(x => x.ClickNextStepLink(), Times.Once);
            Assert.That(availableDays.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetAvailableDays_CheckUniqueMultipleMonths_DictionaryFilledProperly()
        {
            _webPageCrawlerMock.SetupSequence(x => x.PageSource)
                 .Returns(() => { return GenerateHtmlTwoDaysMonthCallendar("enero", "Black", "WhiteSmoke"); })
                 .Returns(() => { return GenerateHtmlTwoDaysMonthCallendar("mayo", "Gray", "Gray"); })
                 .Returns(() => { return GenerateHtmlTwoDaysMonthCallendar("agosto", "WhiteSmoke", "WhiteSmoke"); });

            var availableDays = new PermitChecker(_webPageCrawlerMock.Object).GetAvailableDays([Month.August, Month.May, Month.January]);

            _webPageCrawlerMock.Verify(x => x.ClickNextStepLink(), Times.Once);
            Assert.That(availableDays.Count, Is.EqualTo(2));
            Assert.That(availableDays[Month.January].Count, Is.EqualTo(1));
            Assert.That(availableDays[Month.January].First, Is.EqualTo(6));
            Assert.That(availableDays[Month.August].Count, Is.EqualTo(2));
            Assert.That(availableDays[Month.August], Is.EqualTo([5, 6]));
        }

        [Test]
        public void GetAvailableDays_MonthsToCheckAreNotUnique_DictionaryFilledProperly()
        {
            _webPageCrawlerMock.SetupSequence(x => x.PageSource)
                 .Returns(() => { return GenerateHtmlTwoDaysMonthCallendar("enero", "Black", "WhiteSmoke"); })
                 .Returns(() => { return GenerateHtmlTwoDaysMonthCallendar("mayo", "Gray", "Gray"); })
                 .Returns(() => { return GenerateHtmlTwoDaysMonthCallendar("agosto", "WhiteSmoke", "WhiteSmoke"); });

            var availableDays = new PermitChecker(_webPageCrawlerMock.Object).GetAvailableDays([Month.August, Month.January, Month.May, Month.August, Month.January]);

            _webPageCrawlerMock.Verify(x => x.ClickNextStepLink(), Times.Once);
            Assert.That(availableDays.Count, Is.EqualTo(2));
            Assert.That(availableDays[Month.January].Count, Is.EqualTo(1));
            Assert.That(availableDays[Month.January].First, Is.EqualTo(6));
            Assert.That(availableDays[Month.August].Count, Is.EqualTo(2));
            Assert.That(availableDays[Month.August], Is.EqualTo([5, 6]));
        }

        [Test]
        public void GetAvailableDays_RequiredMonthNotFoundOnTheWebsite_ElevenNextMonthsChecked()
        {
            _webPageCrawlerMock.Setup(x => x.PageSource).Returns(() => { return GenerateHtmlTwoDaysMonthCallendar("enero", "Black", "Black"); });

            var availableDays = new PermitChecker(_webPageCrawlerMock.Object).GetAvailableDays([Month.May]);

            _webPageCrawlerMock.Verify(x => x.ClickNextStepLink(), Times.Once);
            _webPageCrawlerMock.Verify(x => x.ClickNextMonthLink(), Times.Exactly(11));
        }

        private string GenerateHtmlTwoDaysMonthCallendar(string monthName, string firstDayColorName, string secondDayColorName)
        {
            var montCalendar = new StringBuilder();
            montCalendar.Append("<html><table><tbody>");
            montCalendar.Append(" <tr>");
            montCalendar.Append("     <table class='meses'><tbody><tr>");
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

        [Test]
        public void GetAvailableDays_MissingMonthNameInCallendar_ThrowInvalidOperationException()
        {
            var currMonthCallendar = new StringBuilder();
            currMonthCallendar.Append("<html><table><tbody>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <td class='dias' style='background-color:WhiteSmoke;'>");
            currMonthCallendar.Append("         <a href='' style='color:Black' title='5 de mayo'>5</a>");
            currMonthCallendar.Append("     </td>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append("</tbody></table></html>");

            _webPageCrawlerMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());

            var availableDays = new PermitChecker(_webPageCrawlerMock.Object);

            var exception = Assert.Throws<InvalidOperationException>(() => availableDays.GetAvailableDays([Month.May]));
            Assert.That(exception?.Message, Is.EqualTo("Cannot get currently displayed month name. Website seems to have incorrect format."));
        }

        [Test]
        public void GetAvailableDays_NoDaysInWebsiteCallendar_ThrowInvalidOperationException()
        {
            var currMonthCallendar = new StringBuilder();
            currMonthCallendar.Append("<html><table><tbody>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <table class='meses'><tbody><tr>");
            currMonthCallendar.Append("         <td align='center'>mayo de 2025</td>");
            currMonthCallendar.Append("     </tr></tbody></table>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append("</tbody></table></html>");

            _webPageCrawlerMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());

            var availableDays = new PermitChecker(_webPageCrawlerMock.Object);

            var exception = Assert.Throws<InvalidOperationException>(() => availableDays.GetAvailableDays([Month.May]));
            Assert.That(exception?.Message, Is.EqualTo("Cannot get information about days in website callendar. Website seems to have incorrect format."));
        }

        [Test]
        public void GetAvailableDays_NoColorStyleInWebsite_ThrowInvalidOperationExeption()
        {
            var currMonthCallendar = new StringBuilder();
            currMonthCallendar.Append("<html><table><tbody>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <table class='meses'><tbody><tr>");
            currMonthCallendar.Append("         <td align='center'>mayo de 2025</td>");
            currMonthCallendar.Append("     </tr></tbody></table>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append(" <tr>");
            currMonthCallendar.Append("     <td class='dias'>");
            currMonthCallendar.Append($"        <a href='' style='color:Black' title='5 de mayo'>5</a>");
            currMonthCallendar.Append("     </td>");
            currMonthCallendar.Append(" </tr>");
            currMonthCallendar.Append(" </tbody>");
            currMonthCallendar.Append("</tbody></table></html>");
            _webPageCrawlerMock.Setup(x => x.PageSource).Returns(currMonthCallendar.ToString());


            var availableDays = new PermitChecker(_webPageCrawlerMock.Object);

            var exception = Assert.Throws<InvalidOperationException>(() => availableDays.GetAvailableDays([Month.May]));
            Assert.That(exception?.Message, Is.EqualTo("Cannot get the day background color style from the website. Website seems to have incorrect style format."));
        }


    }
}
