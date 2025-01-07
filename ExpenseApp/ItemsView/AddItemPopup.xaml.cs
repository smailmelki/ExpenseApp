using CommunityToolkit.Maui.Views;

namespace ExpenseApp.ItemsView;

public partial class AddItemPopup : Popup
{
	public AddItemPopup(int ID = 0)
	{
		InitializeComponent();
        if (ID!=0)
        {
            lblTitle.Text = "ÊÚÏíá ÇáÚäÕÑ";
        }
    }
}