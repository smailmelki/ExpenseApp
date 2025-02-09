using CommunityToolkit.Maui.Views;
using ExpenseApp.Models;
using ExpenseApp.Resources.languag;
using System.Globalization;

namespace ExpenseApp.ItemsView;

public partial class CatigoryPage : ContentPage
{
    DBContext db = new DBContext();
    public CatigoryPage()
	{
		InitializeComponent();
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            FlowDirection = FlowDirection.RightToLeft;
        else
            FlowDirection = FlowDirection.LeftToRight;
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
        if ((sender as ImageButton).BindingContext is TreeItem item)
        {
            bool answer = await DisplayAlert(AppResource.lblDelete, AppResource.lblDeleteMsg, AppResource.lblYes, AppResource.lblNo);
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