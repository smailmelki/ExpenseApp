using ExpenseApp.Pages;

namespace ExpenseApp.ItemsView;

public partial class ItemDetailsPage : ContentPage
{

	public ItemDetailsPage(MonthlySummary item)
	{
		InitializeComponent();
        lblMonth.Text = item.MonthName +" "+item.YearName;

        CollectionDetails.ItemsSource = item.SupDetails;
        lblTotal.Text = "«·„Ã„Ê⁄ : "+ item.TotalAmount;
#if WINDOWS
        btnBack.IsVisible = true;
#endif
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopAsync();
    }
}