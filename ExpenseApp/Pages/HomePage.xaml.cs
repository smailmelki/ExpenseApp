using CommunityToolkit.Maui.Views;
using ExpenseApp.Classes;
using ExpenseApp.ItemsView;
using ExpenseApp.Models;
using ExpenseApp.Resources.languag;
using Plugin.LocalNotification;
using System.Globalization;

namespace ExpenseApp.Pages;

public partial class HomePage : ContentPage
{
    DBContext db;
    CultureInfo culture = new CultureInfo(Tools.MyCultureInfo);
    private readonly INotificationService _notificationService;
    public string CurrentDate
    {
        get => DateTime.Now.ToString("MMMM yyyy", culture);
    }
    List<ExpensView> items = new List<ExpensView>();
    public HomePage(INotificationService notificationService)
    {
        InitializeComponent();
        _notificationService = notificationService;
        BindingContext = this;
        db = new DBContext();
        lblname.Text = Tools.Name;
        culture.NumberFormat.CurrencySymbol = Tools.currency;
        GetData();
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            FlowDirection = FlowDirection.RightToLeft;
        else
            FlowDirection = FlowDirection.LeftToRight;
    }

    void GetData()
    {
        items = (from d in db.DetailItems.Where(b => b.Date.Date == DateTime.Now.Date)
                 from t in db.TreeItems.Where(i => i.ID == d.ParentID)
                 select new ExpensView
                 {
                     ID = d.ID,
                     ParentID = d.ParentID,
                     Title = t.Title + " (" + d.Note + ")",
                     Date = d.Date.ToString("dd MMMM, HH:mm", culture),
                     Amount = d.Amount.ToString("C", culture),
                 }).ToList();
        itemCollection.ItemsSource = items;
        AmountDay.Text = items
            .Select(s =>
            {
                // „Õ«Ê·…  ÕÊÌ· «·‰’ ≈·Ï double
                if (double.TryParse(s.Amount, NumberStyles.Any, culture, out double amount))
                {
                    return amount;
                }
                return 0; // ≈–« ›‘· «· ÕÊÌ·° Ì „ ≈—Ã«⁄ 0
            })
            .Sum()
            .ToString("C", culture);
        AmountMonth.Text = db.DetailItems.Where(b => b.Date.Year == DateTime.Now.Year && b.Date.Month == DateTime.Now.Month).Sum(s => s.Amount).ToString("C", culture);
    }

    private async void btnAddItem_Clicked(object sender, EventArgs e)
    {
        var popup = new AddItemPopup();
        var result = await this.ShowPopupAsync(popup);
        if (result is DetailItem detail && detail != null)
        {
            db.DetailItems.Add(detail);
            db.SaveChanges();
            GetData();
        }
    }
    private async void OnItemLongPressedAsync(object sender, TappedEventArgs e)
    {
        // «·Õ’Ê· ⁄·Ï «·⁄‰’— «·„Õœœ „‰ «·”Ì«ﬁ
        if (sender is Grid grid && grid.BindingContext is ExpensView selectedItem)
        {
            try
            {
                string action = await DisplayActionSheet(
                    AppResource.lblAction,
                    AppResource.btnCancel,
                    null,
                    AppResource.lblDelete,
                    AppResource.lblEdit
                );

                switch (action)
                {
                    case var a when a == AppResource.lblDelete:
                        //  √ﬂÌœ «·Õ–›
                        bool confirmDelete = await DisplayAlert(
                            AppResource.lblDelete,
                            $"{AppResource.lblDeleteMsg} {selectedItem.Title}?",
                            AppResource.lblYes,
                            AppResource.lblNo
                        );

                        if (confirmDelete)
                        {
                            var itemToDelete = db.DetailItems.Find(selectedItem.ID);
                            if (itemToDelete != null)
                            {
                                db.DetailItems.Remove(itemToDelete);
                                db.SaveChanges();
                                //await DisplayAlert(AppResource.lblSuccess, AppResource.lblDeleteSuccess, AppResource.btnOk);
                                GetData();
                            }
                        }
                        break;

                    case var b when b == AppResource.lblEdit:
                        //  ‰›Ì– ⁄„·Ì… «· ⁄œÌ·
                        var itemToEdit = db.DetailItems.Find(selectedItem.ID);
                        if (itemToEdit != null)
                        {
                            var popup = new AddItemPopup(itemToEdit);
                            var result = await this.ShowPopupAsync(popup);
                            if (result is DetailItem updatedItem && updatedItem != null)
                            {
                                db.DetailItems.Update(updatedItem);
                                db.SaveChanges();
                                //await DisplayAlert(AppResource.lblSuccess, AppResource.lblEditSuccess, AppResource.btnOk);
                                GetData();
                            }
                        }
                        break;

                    default:
                        // ·« Ì „  ‰›Ì– √Ì ≈Ã—«¡
                        break;
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert(AppResource.lblError, ex.Message, AppResource.btnOk);
            }
        }
    }
}
    public class ExpensView
{
    public int ID { get; set; }
    public int ParentID { get; set; } // «·„› «Õ «·Œ«—ÃÌ «·„— »ÿ »‹ TreeItem
    public string Title { get; set; } = "";
    public string? Date { get; set; }
    public string Amount { get; set; } = "0";
}