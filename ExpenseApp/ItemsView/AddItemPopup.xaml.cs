using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ExpenseApp.Models;
using ExpenseApp.Resources.languag;
using System.Text.RegularExpressions;

namespace ExpenseApp.ItemsView;
public partial class AddItemPopup
{
    DBContext db = new DBContext();
    DetailItem detailItem;
    public AddItemPopup()
	{
		InitializeComponent();
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            borderContainer.FlowDirection = FlowDirection.RightToLeft;
        else
            borderContainer.FlowDirection = FlowDirection.LeftToRight;
        PkrCat.ItemsSource = db.TreeItems.ToList();
        detailItem = new DetailItem();       
    }
    public AddItemPopup(DetailItem detail)
    {
        InitializeComponent();
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            borderContainer.FlowDirection = FlowDirection.RightToLeft;
        else
            borderContainer.FlowDirection = FlowDirection.LeftToRight;
        btnAdd.Text = AppResource.btnSave;
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
        detailItem.Note = !string.IsNullOrWhiteSpace(TxtNote.Text) ? TxtNote.Text : AppResource.lbl_NoDetails;
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
            await Toast.Make(AppResource.msg_error1, ToastDuration.Short, 14).Show();
            return false;
        }
        if (string.IsNullOrWhiteSpace(TxtAmount.Text))
        {
            await Toast.Make(AppResource.msg_error2, ToastDuration.Short, 14).Show();
            return false;
        }
        if (!Regex.IsMatch(TxtAmount.Text, @"^\d+(\.\d{1,2})?$"))
        {
            await Toast.Make(AppResource.msg_error3, ToastDuration.Short, 14).Show();
            return false;
        }
        if (!string.IsNullOrWhiteSpace(TxtNote.Text) && TxtNote.Text.Length > 12)
        {
            await Toast.Make(AppResource.msg_Text_Length, ToastDuration.Short, 14).Show();
            return false;
        }
        return true;
    }

    private async void btnCancel_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }
}