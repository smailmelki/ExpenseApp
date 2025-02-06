using System.Globalization;
using ExpenseApp.Classes;

namespace ExpenseApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // تعيين اللغة الافتراضية (مثلاً العربية)
            CultureInfo.CurrentUICulture = CultureManeger.GetCultureInfo(Tools.Long);
            MainPage = new AppShell();
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new NavigationPage(new MainPage()));

        //}
    }
}