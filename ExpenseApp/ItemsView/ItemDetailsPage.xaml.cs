using ExpenseApp.Pages;


namespace ExpenseApp.ItemsView;

public partial class ItemDetailsPage : ContentPage
{
    MonthlySummary data;
	public ItemDetailsPage(MonthlySummary item)
	{
		InitializeComponent();
        data = item;
        lblMonth.Text = data.MonthName +" "+data.YearName;

        CollectionDetails.ItemsSource = data.SupDetails;
        lblTotal.Text = "ÇáãÌãæÚ : "+ data.TotalAmount;
#if WINDOWS
        btnBack.IsVisible = true;
#endif      
        chartVoid();
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