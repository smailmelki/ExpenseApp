using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ExpenseApp.Models;

namespace ExpenseApp.ItemsView;

public partial class AddItemPopup
{
    DBContext db = new DBContext();
    DetailItem detailItem;
    public AddItemPopup()
	{
		InitializeComponent();
        PkrCat.ItemsSource = db.TreeItems.ToList();
        detailItem = new DetailItem();
    }
    public AddItemPopup(DetailItem detail)
    {
        InitializeComponent();
        btnAdd.Text = "Õ›Ÿ";
        PkrCat.ItemsSource = db.TreeItems.ToList();
        detailItem = detail;
        GetData();
    }

    private async void btnAdd_Clicked(object sender, EventArgs e)
    {
        if (await IsDataValid())
        {
            SetData();
            await CloseAsync(detailItem);
        }
    }
    void SetData()
    {
        detailItem.ParentID = ((dynamic)PkrCat.SelectedItem).ID;
        detailItem.Date = DateTime.Now;
        detailItem.Amount = Convert.ToDouble(TxtAmount.Text);
        detailItem.Note = !string.IsNullOrWhiteSpace(TxtNote.Text) ? TxtNote.Text : "»œÊ‰  ›«’Ì·";
    }
    void GetData()
    {
        PkrCat.SelectedItem = db.TreeItems.Find(detailItem.ParentID);
        TxtAmount.Text = detailItem.Amount.ToString();
        TxtNote.Text = detailItem.Note;
    }

    private async Task<bool> IsDataValid()
    {
        if (PkrCat.SelectedItem == null)
        {
            await Toast.Make("ÌÃ» «Œ Ì«— «·›∆…", ToastDuration.Short, 14).Show();
            return false;
        }
        if (string.IsNullOrWhiteSpace(TxtAmount.Text))
        {
            await Toast.Make("ÌÃ» ≈œŒ«· «·„»·€", ToastDuration.Short, 14).Show();
            return false;
        }
        if (!double.TryParse(TxtAmount.Text, out _))
        {
            await Toast.Make("ÌÃ» ≈œŒ«· «·„»·€ »‘ﬂ· ’ÕÌÕ", ToastDuration.Short, 14).Show();
            return false;
        }
        if (!string.IsNullOrWhiteSpace(TxtNote.Text) && TxtNote.Text.Length > 12)
        {
            await Toast.Make("ÌÃ» √‰ ·«   Ã«Ê“ «·‰’ 12 Õ—›«", ToastDuration.Short, 14).Show();
            return false;
        }
        return true;
    }

    private async void btnCancel_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }
}