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
}