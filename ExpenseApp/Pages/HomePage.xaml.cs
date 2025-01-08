using CommunityToolkit.Maui.Views;
using ExpenseApp.ItemsView;

namespace ExpenseApp.Pages;

public partial class HomePage : ContentPage
{
    public string CurrentDate
    {
        get => DateTime.Now.ToString("MMMM yyyy", new System.Globalization.CultureInfo("ar-SA"));
    }

    public HomePage()
	{
		InitializeComponent();
        BindingContext = this;
    }

    private async void btnAddItem_Clicked(object sender, EventArgs e)
    {
        var popup = new AddItemPopup();
        var result = await this.ShowPopupAsync(popup);
    }
}