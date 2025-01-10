using CommunityToolkit.Maui.Views;
using ExpenseApp.Classes;
using ExpenseApp.ItemsView;
using ExpenseApp.Models;

namespace ExpenseApp.Pages;

public partial class HomePage : ContentPage
{
    DBContext db ;
    public string CurrentDate
    {
        get => DateTime.Now.ToString("MMMM yyyy", new System.Globalization.CultureInfo(Tools.MyCultureInfo));
    }
    List<ExpensView> items = new List<ExpensView>();
    public HomePage()
	{
		InitializeComponent();
        BindingContext = this;
        db = new DBContext();
        lblname.Text = Tools.Name;
        GetData();
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
                     Date = d.Date.ToString("dd MMMM, HH:mm", new System.Globalization.CultureInfo(Tools.MyCultureInfo)),
                     Amount = d.Amount,
                 }).ToList();
        itemCollection.ItemsSource = items;
        AmountDay.Text = items.Sum(s => s.Amount).ToString("C", new System.Globalization.CultureInfo(Tools.MyCultureInfo));
        AmountMonth.Text = db.DetailItems.Where(b => b.Date.Year == DateTime.Now.Year && b.Date.Month == DateTime.Now.Month).Sum(s => s.Amount).ToString("C", new System.Globalization.CultureInfo(Tools.MyCultureInfo));
    }

    private async void OnItemLongPressed(object sender, EventArgs e)
    {
        // «·Õ’Ê· ⁄·Ï «·⁄‰’— «·„Õœœ „‰ «·”Ì«ﬁ
        if (sender is Grid grid && grid.BindingContext is ExpensView selectedItem)
        {
            string action = await DisplayActionSheet(
                "«Œ — «·≈Ã—«¡",
                "≈·€«¡",
                null,
                "Õ–›",
                " ⁄œÌ·"
            );

            switch (action)
            {
                case "Õ–›":
                    //  ‰›Ì– ⁄„·Ì… «·Õ–›
                    if (await DisplayAlert("Õ–›", $"Â·  —Ìœ Õ–› {selectedItem.Title}", "„Ê«›ﬁ", "«·€«¡")) 
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
                case " ⁄œÌ·":
                    //  ‰›Ì– ⁄„·Ì… «· ⁄œÌ·
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
                    // ≈·€«¡
                    break;
            }
        }
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
}

public class ExpensView
{
    public int ID { get; set; }
    public int ParentID { get; set; } // «·„› «Õ «·Œ«—ÃÌ «·„— »ÿ »‹ TreeItem
    public string Title { get; set; } = "";
    public string? Date { get; set; }
    public double Amount { get; set; }
}