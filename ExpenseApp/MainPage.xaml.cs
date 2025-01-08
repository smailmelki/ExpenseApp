namespace ExpenseApp
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            CurrentPage = HomeItem;
#if ANDROID
        Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.TabbedPage.SetToolbarPlacement(this, Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.ToolbarPlacement.Bottom);
#endif

        }       
    }
}
