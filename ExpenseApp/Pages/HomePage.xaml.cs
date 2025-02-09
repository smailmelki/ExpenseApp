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
    CultureInfo culture = CultureManeger.GetCultureInfo(Tools.MyCultureInfo);
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
                // „Õ«Ê·…  ÕÊÌ· «·‰’ ≈·Ï double
                if (double.TryParse(s.Amount, NumberStyles.Any, culture, out double amount))
                {
                    return amount;
                }
                return 0; // ≈–« ›‘· «· ÕÊÌ·° Ì „ ≈—Ã«⁄ 0
            })
            .Sum();
        AmountDay.Text = SumDay.ToString("C", culture);
        SumMonth = db.DetailItems.Where(b => b.Date.Year == DateTime.Now.Year && b.Date.Month == DateTime.Now.Month).Sum(s => s.Amount);
        AmountMonth.Text = SumMonth.ToString("C", culture);
        //brdrMonth.BackgroundColor = (SumMonth > double.Parse(Tools.Amount)) ? Colors.OrangeRed : Colors.Transparent;
        brdrMonth.Stroke = (SumMonth > double.Parse(Tools.Amount)) ? Colors.OrangeRed : Colors.Gray;
        //brdrMonth.StrokeThickness = 1;  //  ⁄ÌÌ‰ ”„ﬂ «·ÕœÊœ ≈·Ï 2
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

    private async void Menu_Delete_Clicked(object sender, EventArgs e)
    {
        // «” —Ã«⁄ «·‹ CommandParameter «·–Ì ÌÕ„· «·‹ ID
        var swipeItem = sender as SwipeItem;
        if (swipeItem != null && 
            swipeItem.CommandParameter != null && 
            int.TryParse(swipeItem?.CommandParameter?.ToString(), out int id))
        {
            var selectedItem = items.FirstOrDefault(i => i.ID == id);
            if (selectedItem == null)
            {
                await DisplayAlert("Error", "Item not found", "OK");
                return;
            }
            bool confirmDelete = await DisplayAlert(
                                AppResource.lblDelete,
                                $"{AppResource.lblDeleteMsg} {selectedItem.Title}?",
                                AppResource.lblYes,
                                AppResource.lblNo
                            );
            // ‰›– «·⁄„·Ì… «·Œ«’… »«·Õ–›
            if (confirmDelete)
            {
                var itemToDelete = db.DetailItems.Find(selectedItem.ID);
                if (itemToDelete != null)
                {
                    double amount = itemToDelete.Amount;
                    var selectedIndex = items.IndexOf(selectedItem);
                    // ≈“«·… «·⁄‰’— „ƒﬁ « „‰ Ê«ÃÂ… «·„” Œœ„
                    items.Remove(selectedItem);
                    // Ã„Ìœ «·Ê«ÃÂ… «À‰«¡ «·Õ–›
                    itemCollection.IsEnabled = false;
                    btnAddItem.IsEnabled = false;

                    SumDay -= amount;
                    SumMonth -= amount;
                    AmountDay.Text = SumDay.ToString("C", culture);
                    AmountMonth.Text = SumMonth.ToString("C", culture);

                    bool backDelete = false;// ≈⁄œ«œ „ €Ì— ·· —«Ã⁄
                    var snackbarOptions = new SnackbarOptions
                    {
                        BackgroundColor = Colors.DarkGray,
                        TextColor = Colors.White,
                        ActionButtonTextColor = Colors.Yellow,
                        CornerRadius = new CornerRadius(10)
                    };
                    // ≈‰‘«¡ ﬂ«∆‰ TaskCompletionSource ·≈œ«—… «·«‰ Ÿ«—
                    var tcs = new TaskCompletionSource<bool>();

                    // ≈‰‘«¡ Snackbar
                    var snackbar = Snackbar.Make(
                        AppResource.lblDeleteSecces,
                        action: () =>
                        {
                            backDelete = true; // «· —«Ã⁄ ⁄‰ «·Õ–›
                            tcs.TrySetResult(true); // ≈ﬂ„«· «·„Â„…
                        },
                        actionButtonText: AppResource.btnBack,
                        duration: TimeSpan.FromSeconds(5),
                        visualOptions: snackbarOptions
                    );

                    // ⁄—÷ Snackbar
                    await snackbar.Show();

                    // «·«‰ Ÿ«— Õ Ï «‰ Â«¡ Snackbar √Ê «·÷€ÿ ⁄·Ï " —«Ã⁄"
                    await Task.WhenAny(tcs.Task, Task.Delay(5000));

                    if (backDelete)
                    {
                        // «” —Ã«⁄ «·⁄‰’— ≈·Ï «·ﬁ«∆„…
                        items.Insert(selectedIndex, selectedItem); // ≈⁄«œ… «·⁄‰’—
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
                        // «·Õ–› «·‰Â«∆Ì „‰ ﬁ«⁄œ… «·»Ì«‰« 
                        db.DetailItems.Remove(itemToDelete);
                        db.SaveChanges();
                        GetData();
                    }
                    //«·€«¡  Ã„Ìœ «·Ê«ÃÂ…
                    itemCollection.IsEnabled = true;
                    btnAddItem.IsEnabled = true;
                }

            }
        }
    }

    private async void Menu_Edit_Clicked(object sender, EventArgs e)
    {
        // «” —Ã«⁄ «·‹ CommandParameter «·–Ì ÌÕ„· «·‹ ID
        var swipeItem = sender as SwipeItem;
        if (swipeItem != null && 
            swipeItem.CommandParameter != null && 
            int.TryParse(swipeItem?.CommandParameter?.ToString(), out int id))
        {
            var selectedItem = items.FirstOrDefault(i => i.ID == id);
            if (selectedItem == null)
            {
                await DisplayAlert("Error", "Item not found", "OK");
                return;
            }
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