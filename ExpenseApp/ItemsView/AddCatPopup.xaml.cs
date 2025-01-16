using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using ExpenseApp.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace ExpenseApp.ItemsView;

public partial class AddCatPopup : Popup
{
    TreeItem treeItem;
	public AddCatPopup(TreeItem treeItem)
	{
		InitializeComponent();
        this.treeItem = treeItem;
        if (treeItem.ID != 0)
        {
            lblTitle.Text = " ⁄œÌ· «·⁄‰’—";
            GetItem();
        }
    }

    private void GetItem()
    {
        txtItemTitle.Text = treeItem.Title;
    }
    SKColor GetRandomColor()
    {
        Random rnd = new Random();
        // ≈‰‘«¡ ·Ê‰ ⁄‘Ê«∆Ì »«” Œœ«„ ﬁÌ„ RGB
        byte red = (byte)rnd.Next(256);
        byte green = (byte)rnd.Next(256);
        byte blue = (byte)rnd.Next(256);

        // ≈‰‘«¡ SKColor »«” Œœ«„ «·ﬁÌ„ «·⁄‘Ê«∆Ì…
        return new SKColor(red, green, blue);
    }
    private bool SetItem()
    {
        if (string.IsNullOrEmpty(txtItemTitle.Text))
        {
            Toast.Make("ÌÃ» „·√ «·»Ì«‰«  √Ê·«", ToastDuration.Short, 20).Show();
            return false;
        }
        else
        {
            treeItem.Title = txtItemTitle.Text;
            Color color = GetRandomColor().ToMauiColor();
            treeItem.color = color.ToHex();
            return true;
        }
    }

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        if (SetItem())
            await CloseAsync(treeItem);
    }

   
    private async void btnCancel_Clicked(object sender, EventArgs e)
    {
       await CloseAsync(null);
    }
}