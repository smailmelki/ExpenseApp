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
        await Navigation.PushAsync(new CatigoryPage());
    }

    private void SwitchMode_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
            Application.Current.UserAppTheme = AppTheme.Dark;
        else
            Application.Current.UserAppTheme = AppTheme.Light;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        btn3.BackgroundColor =
        btn6.BackgroundColor =
        btn12.BackgroundColor =
        Colors.Gray;
        var btn = sender as Button;
        btn.BackgroundColor = Color.FromArgb("#FFFFFF");
    }
}