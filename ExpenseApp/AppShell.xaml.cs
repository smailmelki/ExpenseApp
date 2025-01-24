using System.Globalization;

namespace ExpenseApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            FlowDirection = FlowDirection.RightToLeft;
        else
            FlowDirection = FlowDirection.LeftToRight;
    }
}