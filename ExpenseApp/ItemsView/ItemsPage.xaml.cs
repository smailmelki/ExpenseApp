using CommunityToolkit.Maui.Views;

namespace ExpenseApp.ItemsView;

public partial class ItemsPage : ContentPage
{
	public ItemsPage()
	{
		InitializeComponent();
	}

    private async void btnAddItem_Clicked(object sender, EventArgs e)
    {
        var popup = new AddItemPopup();
        var result = await this.ShowPopupAsync(popup); 

        if (result != null)
        {

        }

    }
}