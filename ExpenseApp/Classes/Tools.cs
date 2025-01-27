namespace ExpenseApp.Classes
{
    public static class Tools
    {
        public static string Long { get; set; } = "ar";
        public static string Mode { get; set; } = "Light";
        public static string Name { get; set; } = "ExpenseApp";
        public static string Amount { get; set; } = "0";
        public static string currency { get; set; } = "د.ج";
        public static string MyCultureInfo { get; set; } = "ar-DZ";
        public static bool Notify { get; set; } = false;
        public static string NotifyTime { get; set; } = "3";
        public static void SaveLong()
        {
            // حفظ البيانات
            Preferences.Default.Set<string>("Long", Long);
        }
        public static void SaveMode()
        {
            Preferences.Default.Set<string>("Mode", Mode);
        }
        public static void SaveName()
        {
            Preferences.Default.Set<string>("Name", Name);
        }
        public static void SaveAmount()
        {
            Preferences.Default.Set<string>("Amount", Amount);
            Preferences.Default.Set<string>("Caruncy", currency);
            Preferences.Default.Set<string>("MyCultureInfo", MyCultureInfo);
        }
        public static void SaveNotify()
        {
            Preferences.Default.Set<bool>("Notify", Notify);
            Preferences.Default.Set<string>("NotifyTime", NotifyTime);
        }
        public static void Load()
        {
            // تحميل البيانات
            Long = Preferences.Default.Get<string>("Long", "ar");
            Mode = Preferences.Default.Get<string>("Mode", "Light");
            Name = Preferences.Default.Get<string>("Name", "ExpenseApp");
            Amount = Preferences.Default.Get<string>("Amount", "0");
            currency = Preferences.Default.Get<string>("Caruncy", "دج");
            MyCultureInfo = Preferences.Default.Get<string>("MyCultureInfo", "ar-DZ");
            Notify = Preferences.Default.Get<bool>("Notify", false);
            NotifyTime = Preferences.Default.Get<string>("NotifyTime", "3");
        }
    }
}
