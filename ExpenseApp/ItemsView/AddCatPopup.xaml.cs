using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using ExpenseApp.Models;

namespace ExpenseApp.ItemsView;

public partial class AddCatPopup : Popup
{
    TreeItem treeItem;
	public AddCatPopup(TreeItem treeItem)
	{
		InitializeComponent();
        this.treeItem = treeItem;
        if (treeItem.ID != 0)
        {
            lblTitle.Text = "����� ������";
            GetItem();
        }
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            borderContainer.FlowDirection = FlowDirection.RightToLeft;
        else
            borderContainer.FlowDirection = FlowDirection.LeftToRight;
    }

    private void GetItem()
    {
        txtItemTitle.Text = treeItem.Title;

        string? hexColor = treeItem.color; // ����� ����� Hex
        if (string.IsNullOrEmpty(hexColor))
            hexColor = "#FFFFFF"; // ����� ������
                                  // ����� ����� # ��� ���� ������
        hexColor = hexColor.TrimStart('#');

        // ����� ����� ��� ����� �������� ��� ���
        boxColor.Color = Color.FromRgb(
            Convert.ToInt32(hexColor.Substring(0, 2), 16), // ������
            Convert.ToInt32(hexColor.Substring(2, 2), 16), // ������
            Convert.ToInt32(hexColor.Substring(4, 2), 16)  // ������
            );
    }

    private static readonly Random _random = new Random();

    Color GetRandomColor()
    {
        // ����� ��� ������ �������� ��� RGB
        byte red = (byte)_random.Next(256);
        byte green = (byte)_random.Next(256);
        byte blue = (byte)_random.Next(256);

        // ����� Color �������� ����� ���������
        return Color.FromRgb(red, green, blue);
    }
    private bool SetItem()
    {
        if (string.IsNullOrEmpty(txtItemTitle.Text))
        {
            Toast.Make("��� ��� �������� ����", ToastDuration.Short, 20).Show();
            return false;
        }
        else
        {
            treeItem.Title = txtItemTitle.Text;
            treeItem.color = GetRandomColor().ToHex();
            return true;
        }
    }

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        if (SetItem())
            await CloseAsync(treeItem);
    }

   
    private async void btnCancel_Clicked(object sender, EventArgs e)
    {
       await CloseAsync(null);
    }
}