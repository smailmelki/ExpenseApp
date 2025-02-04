using ExpenseApp.Classes;
using ExpenseApp.Pages;

namespace ExpenseApp
{
    public partial class MainPage
    {
        //private static readonly Lazy<MainPage> _instance = new(() =>
        //{
        //    var notificationService = DependencyService.Get<INotificationService>();
        //    return new MainPage(notificationService);
        //});

        // خاصية للوصول إلى المثيل الساكن
        //public static MainPage Instance => _instance.Value;

        public MainPage()
        {
            InitializeComponent();
            if (Application.Current != null)
                Application.Current.UserAppTheme = Tools.Mode == "Dark" ? AppTheme.Dark : AppTheme.Light;
            Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
       
    }
}
