using CommunityToolkit.Maui.Views;
using ExpenseApp.Classes;
using ExpenseApp.ItemsView;
using ExpenseApp.Models;
using Plugin.LocalNotification;
using System.Globalization;

namespace ExpenseApp.Pages;

public partial class HomePage : ContentPage
{
    DBContext db ;
    private readonly INotificationService _notificationService;
    public string CurrentDate
    {
        get => DateTime.Now.ToString("MMMM yyyy", new CultureInfo(Tools.MyCultureInfo));
    }
    List<ExpensView> items = new List<ExpensView>();
    public HomePage(INotificationService notificationService)
	{
		InitializeComponent();
        _notificationService = notificationService;
        BindingContext = this;
        db = new DBContext();
        lblname.Text = Tools.Name;
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
                     Date = d.Date.ToString("dd MMMM, HH:mm", new CultureInfo(Tools.MyCultureInfo)),
                     Amount = d.Amount.ToString("C", new CultureInfo(Tools.MyCultureInfo)),
                 }).ToList();
        itemCollection.ItemsSource = items;
        AmountDay.Text = items
            .Select(s =>
            {
                // ������ ����� ���� ��� double
                if (double.TryParse(s.Amount, NumberStyles.Any, new CultureInfo(Tools.MyCultureInfo), out double amount))
                {
                    return amount;
                }
                return 0; // ��� ��� ������� ��� ����� 0
            })
            .Sum()
            .ToString("C", new CultureInfo(Tools.MyCultureInfo));
        AmountMonth.Text = db.DetailItems.Where(b => b.Date.Year == DateTime.Now.Year && b.Date.Month == DateTime.Now.Month).Sum(s => s.Amount).ToString("C", new CultureInfo(Tools.MyCultureInfo));
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


/* Unmerged change from project 'ExpenseApp (net9.0-android)'
Before:
    private void OnItemLongPressed(object sender, TappedEventArgs e)
    {
After:
    private async Task OnItemLongPressedAsync(object sender, TappedEventArgs e)
    {
*/
    private async void OnItemLongPressedAsync(object sender, TappedEventArgs e)
    {
        // ������ ��� ������ ������ �� ������
        if (sender is Grid grid && grid.BindingContext is ExpensView selectedItem)
        {
            string action = await DisplayActionSheet(
                "���� �������",
                "�����",
                null,
                "���",
                "�����"
            );

            switch (action)
            {
                case "���":
                    // ����� ����� �����
                    if (await DisplayAlert("���", $"�� ���� ��� {selectedItem.Title}", "�����", "�����"))
                    {
                        var item1 = db.DetailItems.Find(selectedItem.ID);
                        if (item1 != null)
                        {
                            db.DetailItems.Remove(item1);
                            db.SaveChanges();
                            GetData();
                        }
                    }
                    break;
                case "�����":
                    // ����� ����� �������
                    var item = db.DetailItems.Find(selectedItem.ID);
                    var popup = new AddItemPopup(item);
                    var result = await this.ShowPopupAsync(popup);
                    if (result is DetailItem detail && detail != null)
                    {
                        //db = new DBContext();
                        db.DetailItems.Update(detail);
                        db.SaveChanges();
                        GetData();
                    }

                    break;
                default:
                    // �����
                    break;
            }
        }
    }
}

public class ExpensView
{
    public int ID { get; set; }
    public int ParentID { get; set; } // ������� ������� ������� �� TreeItem
    public string Title { get; set; } = "";
    public string? Date { get; set; }
    public string Amount { get; set; }
}