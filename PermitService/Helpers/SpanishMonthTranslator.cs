
namespace PermitService.Helpers
{
    public static class SpanishMonthTranslator
    {
        public static string GetTranslation(Month month)
        {
            return month switch
            {
                Month.January => "Enero",
                Month.February => "Febrero",
                Month.March => "Marzo",
                Month.April => "Abril",
                Month.May => "Mayo",
                Month.June => "Junio",
                Month.July => "Julio",
                Month.August => "Agosto",
                Month.September => "Septiembre",
                Month.October => "Octubre",
                Month.November => "Noviembre",
                Month.December => "Diciembre",
                _ => "Invalid month"
            };
        }

        public static string GetTranslationLowerCase(Month month)
        {
            return GetTranslation(month).ToLower();
        }
    }
}
