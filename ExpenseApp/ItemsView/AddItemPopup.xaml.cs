using ExpenseApp.Models;

namespace ExpenseApp.ItemsView;

public partial class AddItemPopup
{
    DBContext db = new DBContext();
    public AddItemPopup()
	{
		InitializeComponent();
        PkrCat.ItemsSource = db.TreeItems.ToList();
    }

    private void btnAdd_Clicked(object sender, EventArgs e)
    {

    }

    private async void btnCancel_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }
}