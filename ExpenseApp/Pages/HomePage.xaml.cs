using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using ExpenseApp.Classes;
using ExpenseApp.ItemsView;
using ExpenseApp.Models;
using ExpenseApp.Resources.languag;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ExpenseApp.Pages;

public partial class HomePage : ContentPage
{
    DBContext db;
    CultureInfo culture = new CultureInfo(Tools.MyCultureInfo);
    double SumMonth;
    double SumDay;
    public string CurrentDate
    {
        get => DateTime.Now.ToString("MMMM yyyy", culture);
    }
    ObservableCollection<ExpensView> items = new ObservableCollection<ExpensView>();
    public HomePage()
    {
        InitializeComponent();
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
                 }).ToObservableCollection();
        itemCollection.ItemsSource = items;
        SumDay = items
            .Select(s =>
            {
                // ������ ����� ���� ��� double
                if (double.TryParse(s.Amount, NumberStyles.Any, culture, out double amount))
                {
                    return amount;
                }
                return 0; // ��� ��� ������� ��� ����� 0
            })
            .Sum();
        AmountDay.Text = SumDay.ToString("C", culture);
        SumMonth = db.DetailItems.Where(b => b.Date.Year == DateTime.Now.Year && b.Date.Month == DateTime.Now.Month).Sum(s => s.Amount);
        AmountMonth.Text = SumMonth.ToString("C", culture);
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
        // ������ ��� ������ ������ �� ������
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
                        // ����� �����
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
                                double amount = itemToDelete.Amount;
                                var selectedIndex = items.IndexOf(selectedItem);
                                // ����� ������ ������ �� ����� ��������
                                items.Remove(selectedItem);
                                //����� ������� ����� �����
                                itemCollection.IsEnabled = false;
                                btnAddItem.IsEnabled = false;

                                SumDay -= amount;
                                SumMonth -= amount;
                                AmountDay.Text = SumDay.ToString("C", culture);
                                AmountMonth.Text = SumMonth.ToString("C", culture);

                                bool backDelete = false;// ����� ����� �������
                                var snackbarOptions = new SnackbarOptions
                                {
                                    BackgroundColor = Colors.DarkGray,
                                    TextColor = Colors.White,
                                    ActionButtonTextColor = Colors.Yellow,
                                    CornerRadius = new CornerRadius(10)
                                };
                                // ����� ���� TaskCompletionSource ������ ��������
                                var tcs = new TaskCompletionSource<bool>();

                                // ����� Snackbar
                                var snackbar = Snackbar.Make(
                                    AppResource.lblDeleteSecces,
                                    action: () =>
                                    {
                                        backDelete = true; // ������� �� �����
                                        tcs.TrySetResult(true); // ����� ������
                                    },
                                    actionButtonText: AppResource.btnBack,
                                    duration: TimeSpan.FromSeconds(5),
                                    visualOptions: snackbarOptions
                                );

                                // ��� Snackbar
                                await snackbar.Show();

                                // �������� ��� ������ Snackbar �� ����� ��� "�����"
                                await Task.WhenAny(tcs.Task, Task.Delay(5000));

                                if (backDelete)
                                {
                                    // ������� ������ ��� �������
                                    items.Insert(selectedIndex, selectedItem); // ����� ������
                                    await Toast.Make(
                                         AppResource.lblDeleteBack,
                                        duration: ToastDuration.Short,
                                        textSize: 14
                                    ).Show();
                                    SumDay += amount;
                                    SumMonth += amount;
                                    AmountDay.Text = SumDay.ToString("C", culture);
                                    AmountMonth.Text = SumMonth.ToString("C", culture);
                                }
                                else
                                {
                                    // ����� ������� �� ����� ��������
                                    db.DetailItems.Remove(itemToDelete);
                                    db.SaveChanges();
                                }
                                //����� ����� �������
                                itemCollection.IsEnabled = true;
                                btnAddItem.IsEnabled = true;
                            }
                        }
                        break;

                    case var b when b == AppResource.lblEdit:
                        // ����� ����� �������
                        var itemToEdit = db.DetailItems.Find(selectedItem.ID);
                        if (itemToEdit != null)
                        {
                            var popup = new AddItemPopup(itemToEdit);
                            var result = await this.ShowPopupAsync(popup);
                            if (result is DetailItem updatedItem && updatedItem != null)
                            {
                                db.DetailItems.Update(updatedItem);
                                db.SaveChanges();
                                GetData();
                            }
                        }
                        break;
                    default:
                        // �� ��� ����� �� �����
                        break;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred during process\n {ex.Message}", "OK");
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
    public string Amount { get; set; } = "0";
}