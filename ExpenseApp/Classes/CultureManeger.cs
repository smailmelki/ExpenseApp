using ExpenseApp.Resources.languag;
using System.Globalization;

namespace ExpenseApp.Classes
{
    public static class CultureManeger
    {
        // قائمة العملات مع رموزها
        public static List<Currency> currencies = new List<Currency>
        {
            new Currency { Name = AppResource.Cur_DZD, Symbol = AppResource.Sym_DZD, Culture = "ar-DZ" },
            new Currency { Name = AppResource.Cur_SAR, Symbol = "﷼", Culture = "ar-SA" },
            new Currency { Name = AppResource.Cur_USD, Symbol = "$", Culture = "en-US" },
            new Currency { Name = AppResource.Cur_euro, Symbol = "€", Culture = "fr-FR" },
            new Currency { Name = AppResource.Cur_GBP, Symbol = "£", Culture = "en-GB" },
            new Currency { Name = AppResource.Cur_JPY, Symbol = "¥", Culture = "ja-JP" }
        };
        /// <summary>
        /// اعادة تحميل الصفحة الرئيسية
        /// </summary>
        /// <param name="languageCode"></param>
        public static void ChangeLanguage(string languageCode)
        {
            var culture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            // إعادة إنشاء الصفحة الرئيسية لتطبيق التغييرات
            App.Current.MainPage = new AppShell();
        }

        public static CultureInfo GetCultureInfo(string culture)
        {
            return CultureInfo.GetCultureInfo(culture);
            //return new CultureInfo(culture);
        }
    }
    // كائن لتمثيل العملة
    public class Currency
    {
        public string? Name { get; set; }
        public string? Symbol { get; set; }
        public string? Culture { get; set; }
    }
}
