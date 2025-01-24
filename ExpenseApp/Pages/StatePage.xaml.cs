using ExpenseApp.Classes;
using ExpenseApp.ItemsView;
using ExpenseApp.Models;
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
        FillPicker();
        FillData(Convert.ToInt32(pkrYear.SelectedItem));
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            FlowDirection = FlowDirection.RightToLeft;
        else
            FlowDirection = FlowDirection.LeftToRight;
    }

    private void FillPicker()
    {
        pkrYear.ItemsSource = Enumerable.Range(2024, 7).Select(year => year.ToString()).ToList();
        int index = pkrYear.ItemsSource.IndexOf(DateTime.Now.Date.Year.ToString());
        if (index == -1)
            index = 0;
        pkrYear.SelectedIndex = index;
    }

    private void FillData(int year)
    {
        var detailItems = db.DetailItems
            .Where(d => d.Date.Year == year)
            .ToList(); // Load data into memory

        var treeItems = db.TreeItems.ToList(); // Load data from the other table

        var data = (from d in detailItems
                    join t in treeItems on d.ParentID equals t.ID
                    group d by new { d.Date.Year, d.Date.Month } into g
                    select new MonthlySummary
                    {
                        MonthName = new DateTime(g.Key.Year, g.Key.Month, 1)
                                   .ToString("MMMM", new CultureInfo(Tools.MyCultureInfo)),
                        YearName = g.Key.Year.ToString(),
                        TotalAmount = g.Sum(item => item.Amount).ToString("C", new CultureInfo(Tools.MyCultureInfo)),
                        IsExpanded = false,
                        SupDetails = (from groupItem in g
                                      join treeItem in treeItems on groupItem.ParentID equals treeItem.ID
                                      select new catTree
                                      {
                                          Title = treeItem.Title,
                                          Amount = groupItem.Amount.ToString("C", new CultureInfo(Tools.MyCultureInfo)),
                                          Note = groupItem.Note,
                                          date = groupItem.Date.ToString("dd MMMM HH:mm" , new CultureInfo(Tools.MyCultureInfo))
                                      }).ToList(),
                        Details = (from item in g
                                   join t in treeItems on item.ParentID equals t.ID
                                   group item by new { item.ParentID, t.Title , t.color } into g2
                                   select new catTree
                                   {
                                       Title = g2.Key.Title,
                                       color = g2.Key.color,
                                       Cost = g2.Sum(s => s.Amount),
                                       Amount = g2.Sum(s => s.Amount).ToString("C", new CultureInfo(Tools.MyCultureInfo)),
                                   }).ToList()
                    }).ToList();

        // Bind data to the view element
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
        await Navigation.PushAsync(new ItemDetailsPage(item));
    }
}
public class MonthlySummary
{
    public string? MonthName { get; set; }
    public string? YearName { get; set; }
    public string? TotalAmount { get; set; }
    public bool IsExpanded { get; set; }  // Œ«’Ì… «· Ê”Ì⁄
    public List<catTree>? Details { get; set; } //  ›«’Ì· «·”ÿ—
    public List<catTree>? SupDetails { get; set; } //  ›«’Ì· «·”ÿ—
}
public class catTree
{
    public string? Title { get; set; }
    public string? Amount { get; set; }
    public double Cost { get; set; }
    public string? color { get; set; }
    public string? Note { get; set; }
    public string? date { get; set; }
}