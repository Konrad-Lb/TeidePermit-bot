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
        public void CreateMonthFromSpanishName_LoweCaseMonthNames_MonthsCreatedProperly()
        {
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("enero"), Is.EqualTo(Month.January));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("febrero"), Is.EqualTo(Month.February));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("marzo"), Is.EqualTo(Month.March));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("abril"), Is.EqualTo(Month.April));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("mayo"), Is.EqualTo(Month.May));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("junio"), Is.EqualTo(Month.June));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("julio"), Is.EqualTo(Month.July));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("agosto"), Is.EqualTo(Month.August));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("septiembre"), Is.EqualTo(Month.September));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("octubre"), Is.EqualTo(Month.October));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("noviembre"), Is.EqualTo(Month.November));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("diciembre"), Is.EqualTo(Month.December));
            Assert.Throws<InvalidOperationException>(() => SpanishMonthTranslator.CreateMonthFromSpanishName("someText"));
        }

        [Test]
        public void CreateMonthFromSpanishName_UpperCaseMonthNames_MonthCreatedProperly()
        {
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Enero"), Is.EqualTo(Month.January));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Febrero"), Is.EqualTo(Month.February));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Marzo"), Is.EqualTo(Month.March));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Abril"), Is.EqualTo(Month.April));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Mayo"), Is.EqualTo(Month.May));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Junio"), Is.EqualTo(Month.June));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Julio"), Is.EqualTo(Month.July));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Agosto"), Is.EqualTo(Month.August));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Septiembre"), Is.EqualTo(Month.September));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Octubre"), Is.EqualTo(Month.October));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Noviembre"), Is.EqualTo(Month.November));
            Assert.That(SpanishMonthTranslator.CreateMonthFromSpanishName("Diciembre"), Is.EqualTo(Month.December));
        }
    }
}
