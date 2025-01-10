using ExpenseApp.Classes;
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
        // «·ŒÿÊ… 1:  Ã„Ì⁄ «·»Ì«‰«  Õ”» «·”‰… Ê«·‘Â—
        var detailItems = db.DetailItems.Where(d => d.Date.Year == year).ToList();
        var treeItems = db.TreeItems.ToList();

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
                                   select new catTree
                                   {
                                       ID = treeItem.ID,
                                       Title = treeItem.Title,
                                       Amount = groupItem.Amount.ToString("C", new CultureInfo(Tools.MyCultureInfo))
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
    public string? Amount { get; set; }
}