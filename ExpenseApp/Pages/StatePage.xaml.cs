using CommunityToolkit.Maui.Views;
using ExpenseApp.Classes;
using ExpenseApp.ItemsView;
using ExpenseApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ExpenseApp.Pages;

public partial class StatePage : ContentPage
{
	DBContext db = new DBContext();
    public ObservableCollection<TreeItem> Items { get; set; } = new ObservableCollection<TreeItem>();

    public StatePage()
	{
		InitializeComponent();
        pkrYear.ItemsSource = Enumerable.Range(2025, 5).Select(year => year.ToString()).ToList();
        pkrYear.SelectedIndex = 0;
        FillData(Convert.ToInt32(pkrYear.SelectedItem));
    }

    private void FillData(int year)
    {

        var data = (from d in db.DetailItems
                    where d.Date.Year == year
                    join t in db.TreeItems on d.ParentID equals t.ID
                    group d by new { d.Date.Year, d.Date.Month } into g
                    select new MonthlySummary
                    {
                        MonthName = "‘Â— " + new DateTime(g.Key.Year, g.Key.Month, 1)
                                             .ToString("MMMM", new CultureInfo(Tools.MyCultureInfo)),
                        TotalAmount = g.Sum(item => item.Amount).ToString("C", new CultureInfo(Tools.MyCultureInfo)),
                        IsExpanded = false,
                        Details = (from groupItem in g
                                   join treeItem in db.TreeItems on groupItem.ParentID equals treeItem.ID
                                   //group treeItems by new { groupItem.ID } into g2
                                   select new catTree
                                   {
                                       ID = groupItem.ID,
                                       Title = treeItem.Title,
                                       Amount = groupItem.Amount
                                   }).ToList()
                    })
             .ToList();
        // —»ÿ «·»Ì«‰«  »⁄‰’— «·⁄—÷
        CollectionItemView.ItemsSource = data;

    }

    private void pkrYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (pkrYear.SelectedItem == null)
            return;
        lblYear.Text = db.DetailItems.Where(b => b.Date.Year == Convert.ToInt32(pkrYear.SelectedItem)).Sum(s => s.Amount).ToString("C", new CultureInfo(Tools.MyCultureInfo));
        FillData(Convert.ToInt32(pkrYear.SelectedItem));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        MonthlySummary item = (MonthlySummary)((Button)sender).BindingContext;
        if (item != null)
        {
            await this.ShowPopupAsync(new ReportPopupPage(item));
        }
    }
}
public class MonthlySummary
{
    public string? MonthName { get; set; }
    public string? TotalAmount { get; set; }
    public bool IsExpanded { get; set; }  // Œ«’Ì… «· Ê”Ì⁄
    public List<catTree>? Details { get; set; } //  ›«’Ì· «·”ÿ—
}
public class catTree
{
    public int ID { get; set; }
    public string? Title { get; set; }
    public double Amount { get; set; }
}