using ExpenseApp.ItemsView;

namespace ExpenseApp.Pages;

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

    private void SwitchMode_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
            Application.Current.UserAppTheme = AppTheme.Dark;
        else
            Application.Current.UserAppTheme = AppTheme.Light;
    }
}