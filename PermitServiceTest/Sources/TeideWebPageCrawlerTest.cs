using Moq;
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
    public class TeideWebPageCrawlerTest
    {
        private Mock<IWebDriver> _webDriverStub = null!;
        
        
        [SetUp]
        public void TestSetUp()
        {
            _webDriverStub = new Mock<IWebDriver>();
        }

        [Test]
        public void ClickNextStepLink_ThereIsButton1ElemendOnWebPage_NexStepLinkClickedOnce()
        {
            var nextStepLinkMock = new Mock<IWebElement>();
            _webDriverStub.Setup(x => x.FindElement(By.Id("Button1"))).Returns(nextStepLinkMock.Object);

            var crawler = new TeideWebPageClawler(_webDriverStub.Object);
            crawler.ClickNextStepLink();

            nextStepLinkMock.Verify(x => x.Click(), Times.Once());
        }

        [Test]
        public void ClickNextStepLink_ThereIsNoButton1ElementOnTheWebPage_ThrowsInvalidOperationException()
        {
            _webDriverStub.Setup(x => x.FindElement(By.Id("Button1"))).Throws<NoSuchElementException>();

            var crawler = new TeideWebPageClawler(_webDriverStub.Object);
            var exception = Assert.Throws<InvalidOperationException>(() => crawler.ClickNextStepLink());
            Assert.That(exception?.Message, Is.EqualTo("Permit website has invalid html content. Cannot click on the 'Next Step >>'. For more details see inner exception."));
            Assert.That(exception?.InnerException, Is.TypeOf<NoSuchElementException>());

        }

        [Test]
        public void ClickNextMonthLink_ThereIsNextMonthLinkOnTheWebPage_NextMonthLinkClickedOnce()
        {
            var nextMonthLinkMock = new Mock<IWebElement>();
            _webDriverStub.Setup(x => x.FindElement(By.CssSelector("a[title='Ir al mes siguiente.']"))).Returns(nextMonthLinkMock.Object);

            var crawler = new TeideWebPageClawler(_webDriverStub.Object);
            crawler.ClickNextMonthLink();

            nextMonthLinkMock.Verify(x => x.Click(), Times.Once());
        }

        [Test]
        public void ClickNextMonthLink_ThereIsNoNextMonthLinkOnTheWebPage_ThrowsInvalidOperationException()
        {
            var nextMonthLinkMock = new Mock<IWebElement>();
            _webDriverStub.Setup(x => x.FindElement(By.CssSelector("a[title='Ir al mes siguiente.']"))).Throws<NoSuchElementException>();

            var crawler = new TeideWebPageClawler(_webDriverStub.Object);
            var exception = Assert.Throws<InvalidOperationException>(() => crawler.ClickNextMonthLink());
            Assert.That(exception?.Message, Is.EqualTo("Permit website has invalid html content. Cannot click on the next month link'. For more details see inner exception."));
            Assert.That(exception?.InnerException, Is.TypeOf<NoSuchElementException>());
        }
    }
}
