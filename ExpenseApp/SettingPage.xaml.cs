using ExpenseApp.ItemsView;

namespace ExpenseApp;

public partial class SettingPage : ContentPage
{
	public SettingPage()
	{
		InitializeComponent();
	}

    private async void btnItems_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ItemsPage());
    }

    private void swMode_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
            Application.Current.UserAppTheme = AppTheme.Dark;
        else
            Application.Current.UserAppTheme = AppTheme.Light;
    }
}