using System.Globalization;

namespace ExpenseApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // تعيين اللغة الافتراضية (مثلاً العربية)
            var culture = new CultureInfo("ar");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            MainPage = new AppShell();
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new NavigationPage(new MainPage()));

        //}
    }
}