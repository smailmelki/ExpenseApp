using CommunityToolkit.Maui.Views;
using ExpenseApp.ItemsView;

namespace ExpenseApp.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}

    private async void btnAddItem_Clicked(object sender, EventArgs e)
    {
        var popup = new AddItemPopup();
        var result = await this.ShowPopupAsync(popup);
    }
}