using CommunityToolkit.Maui.Views;
using ExpenseApp.Models;

namespace ExpenseApp.ItemsView;

public partial class ItemsPage : ContentPage
{
    DBContext db = new DBContext();
    public ItemsPage()
	{
		InitializeComponent();
        collectionItem.ItemsSource = db.TreeItems.ToList();
    }

    private async void btnAddItem_Clicked(object sender, EventArgs e)
    {
        var popup = new AddCatPopup(new TreeItem());
        var result = await this.ShowPopupAsync(popup);

        if (result is TreeItem && result != null)
        {
            db.TreeItems.Add(result as TreeItem);
            db.SaveChanges();
            collectionItem.ItemsSource = db.TreeItems.ToList();
        }
    }

    private async void btnEdit_Clicked(object sender, EventArgs e)
    {
        TreeItem? item = (sender as ImageButton).BindingContext as TreeItem;
        if (item!=null)
        {
            var popup = new AddCatPopup(item);
            var result = await this.ShowPopupAsync(popup);

            if (result is TreeItem && result != null)
            {
                db = new DBContext();
                db.TreeItems.Update(result as TreeItem);
                db.SaveChanges();
                collectionItem.ItemsSource = db.TreeItems.ToList();
            }
        }
        
    }

    private async void btnDelete_Clicked(object sender, EventArgs e)
    {
        TreeItem? item = (sender as ImageButton).BindingContext as TreeItem;
        if (item != null)
        {
            bool answer = await DisplayAlert("delete?", "Would you want to remove", "Yes", "No");
            if (answer)
            {
                db = new DBContext();
                db.TreeItems.Remove(item);
                db.SaveChanges();
                collectionItem.ItemsSource = db.TreeItems.ToList();
            }
        }
    }
}