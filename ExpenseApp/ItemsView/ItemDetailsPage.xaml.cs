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
#if WINDOWS
        btnBack.IsVisible = true;
#endif      
        chartVoid();
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            CollectionDetails.FlowDirection = FlowDirection.RightToLeft;
        else
            CollectionDetails.FlowDirection = FlowDirection.LeftToRight;
    }


    void chartVoid()
    {
        //var entries = data.Details.Select(d => new ChartEntry((float)d.Cost)
        //{
        //    Label = d.Title.ToString(),
        //    ValueLabel = d.Cost.ToString(),
        //    // Set color dynamically if needed
        //    Color = SKColor.Parse(d.color)// Or a dynamic color based on data
        //}).ToArray();

        //// Create a chart (e.g., BarChart)
        //var chart = new BarChart()
        //{
        //    Entries = entries,
        //    LabelTextSize = 24, // Example customization
        //    ValueLabelOrientation = Orientation.Horizontal,
        //    LabelOrientation = Orientation.Horizontal
        //};

        //// Assign the chart to the ChartView
        //chartView.Chart = chart;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopAsync();
    }
}