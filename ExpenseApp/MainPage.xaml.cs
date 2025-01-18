using ExpenseApp.Pages;

namespace ExpenseApp
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            //Navigation.PushAsync(new HomePage());
            Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }       
    }
}
