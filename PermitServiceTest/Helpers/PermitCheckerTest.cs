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
            string currentMonthCallendar = "<html>" +
            "<table>" +
                "<tbody>" +
                    "<tr>" +
                        "<td class=\"dias\" style=\"background-color:Gray;\">" +
                            "<a href=\"\" style=\"color:Black\" title=\"5 de mayo\">5</a>" +
                        "</td>" +
                        "<td class=\"dias\" style=\"background-color:Black;\">" +
                            "<a href=\"\" style=\"color:Black\" title=\"6 de mayo\">6</a>" +
                        "</td>"+
                    "</tr>" +
                "</tbody>" +
            "</table>" +
            "</html>";
            _webDriverMock.Setup(x => x.PageSource).Returns(currentMonthCallendar);

            var availableDays = PermitChecker.GetAvailableDays(_webDriverMock.Object);
            Assert.That(availableDays.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetAvailableDays_OneAvailableDayOnFifthOfMay_DictonaryHasOneEntry()
        {
            string currentMonthCallendar = "<html>" +
            "<table>" +
                "<tbody>" +
                    "<tr>" +
                        "<td class=\"dias\" style=\"background-color:WhiteSmoke;\">" +
                            "<a href=\"\" style=\"color:Black\" title=\"5 de mayo\">5</a>" +
                        "</td>" +
                    "</tr>" +
                "</tbody>" +
            "</table>"+
            "</html>";
            _webDriverMock.Setup(x => x.PageSource).Returns(currentMonthCallendar);
            
            var availableDays = PermitChecker.GetAvailableDays(_webDriverMock.Object);
            
            Assert.That(availableDays.ContainsKey(Month.May) , Is.True);
            Assert.That(availableDays[Month.May].Count, Is.EqualTo(1));
            Assert.That(availableDays[Month.May].First, Is.EqualTo(5));
        }

        //diffrent date single days available test
        //style vs bgcolor attribute
        //webelement Button1 not foound
        //test wait.Until
        //tes calendar script failed
        //no available days for 12 months
        //one day available - others are not available
        //current month detection on website
        //no page content
    }
}
