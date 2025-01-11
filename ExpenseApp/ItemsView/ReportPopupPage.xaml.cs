using ExpenseApp.Classes;
using ExpenseApp.Pages;
using System.Globalization;

namespace ExpenseApp.ItemsView;

public partial class ReportPopupPage
{
	MonthlySummary MonthlyItem;
    public ReportPopupPage(MonthlySummary MonthlyItem)
	{
		InitializeComponent();
        if (MonthlyItem != null)
        {
            this.MonthlyItem = MonthlyItem;
            lblAverage.Text = MonthlyItem.Details.Average(a => a.Amount).ToString("C", new CultureInfo(Tools.MyCultureInfo));
            lblMax.Text = MonthlyItem.Details.Max(a => a.Amount).ToString("C", new CultureInfo(Tools.MyCultureInfo));
            lblMin.Text = MonthlyItem.Details.Min(a => a.Amount).ToString("C", new CultureInfo(Tools.MyCultureInfo));
        }
    }
}