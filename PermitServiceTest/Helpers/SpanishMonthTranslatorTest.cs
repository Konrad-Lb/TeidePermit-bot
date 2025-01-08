using NUnit.Framework;
using PermitService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitServiceTest.Helpers
{
    [TestFixture]
    public class SpanishMonthTranslatorTest
    {
        [Test]
        public void GetTranslation_CheckAllTranslations_ProperTranslationReturned()
        {
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.January), Is.EqualTo("Enero"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.February), Is.EqualTo("Febrero"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.March), Is.EqualTo("Marzo"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.April), Is.EqualTo("Abril"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.May), Is.EqualTo("Mayo"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.June), Is.EqualTo("Junio"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.July), Is.EqualTo("Julio"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.August), Is.EqualTo("Agosto"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.September), Is.EqualTo("Septiembre"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.October), Is.EqualTo("Octubre"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.November), Is.EqualTo("Noviembre"));
            Assert.That(SpanishMonthTranslator.GetTranslation(Month.December), Is.EqualTo("Diciembre"));
        }

        [Test]
        public void GetTranslationLowerCase()
        {
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.January), Is.EqualTo("enero"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.February), Is.EqualTo("febrero"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.March), Is.EqualTo("marzo"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.April), Is.EqualTo("abril"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.May), Is.EqualTo("mayo"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.June), Is.EqualTo("junio"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.July), Is.EqualTo("julio"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.August), Is.EqualTo("agosto"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.September), Is.EqualTo("septiembre"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.October), Is.EqualTo("octubre"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.November), Is.EqualTo("noviembre"));
            Assert.That(SpanishMonthTranslator.GetTranslationLowerCase(Month.December), Is.EqualTo("diciembre"));
        }
    }
}
