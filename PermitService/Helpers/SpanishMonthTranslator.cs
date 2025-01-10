
namespace PermitService.Helpers
{
    public static class SpanishMonthTranslator
    {
        public static Month CreateMonthFromSpanishName(string monthName)
        {
            return monthName.ToLower() switch
            {
                "enero" => Month.January,
                "febrero" => Month.February,
                "marzo" => Month.March,
                "abril" => Month.April,
                "mayo" => Month.May,
                "junio" => Month.June,
                "julio" => Month.July,
                "agosto" => Month.August,
                "septiembre" => Month.September,
                "octubre" => Month.October,
                "noviembre" => Month.November,
                "diciembre" => Month.December,
                _ => throw new InvalidOperationException($"Not recognized Spanish month name: {monthName}")
            };
        }
    }
}
