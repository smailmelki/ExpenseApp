using System.Globalization;
using ExpenseApp.Classes;
using ExpenseApp.Pages;
using ExpenseApp.Resources.languag;


namespace ExpenseApp.ItemsView;

public partial class ItemDetailsPage : ContentPage
{
    MonthlySummary data;
    private readonly ChartBareClass? _drawable;

    public ItemDetailsPage(MonthlySummary item)
	{
		InitializeComponent();
        data = item;
        if (data.Details != null && data.Details.Count >= 0)
            _drawable = new ChartBareClass(data.Details);
        BindingContext = new
        {
            GridDrawable = _drawable
        };

        lblMonth.Text = data.MonthName + " " + data.YearName;

        CollectionDetails.ItemsSource = data.SupDetails;
        lblTotal.Text = AppResource.lblStatisTotal + data.TotalAmount;   
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            CollectionDetails.FlowDirection = FlowDirection.RightToLeft;
        else
            CollectionDetails.FlowDirection = FlowDirection.LeftToRight;
    }
}