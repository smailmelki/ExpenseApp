using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;

namespace ExpenseApp.ItemsView;

public partial class ColorsPopup : Popup
{
    public ColorsPopup()
    {
        InitializeComponent();
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // ����� ������� �������: 20 ����� ������� �������� ��� 20 ����� ���
        var colors = new List<Color>
        {
            // ������� ��������
            Color.FromArgb("#00FFFF"), // Aqua
            Color.FromArgb("#FFE4C4"), // Bisque
            Color.FromArgb("#0000FF"), // Blue
            Color.FromArgb("#8A2BE2"), // BlueViolet
            Color.FromArgb("#A52A2A"), // Brown
            Color.FromArgb("#7FFF00"), // Chartreuse
            Color.FromArgb("#D2691E"), // Chocolate
            Color.FromArgb("#9ACD32"), // YellowGreen
            Color.FromArgb("#FF8C00"), // DarkOrange
            Color.FromArgb("#FF1493"), // DeepPink
            Color.FromArgb("#9400D3"), // DarkViolet
            Color.FromArgb("#FFD700"), // Gold
            Color.FromArgb("#ADFF2F"), // GreenYellow
            Color.FromArgb("#F08080"), // LightCoral
            Color.FromArgb("#FFA5FF"), // PinkLavender
            Color.FromArgb("#FF0000"), // Red
            Color.FromArgb("#C0C0C0"), // Silver
            Color.FromArgb("#FF6347"), // Tomato
            Color.FromArgb("#FFFF00"), // Yellow
            Color.FromArgb("#00FF00"), // Green

            // 20 ����� �������
            Color.FromArgb("#1E90FF"), // DodgerBlue
            Color.FromArgb("#FF69B4"), // HotPink
            Color.FromArgb("#32CD32"), // LimeGreen
            Color.FromArgb("#8B4513"), // SaddleBrown
            Color.FromArgb("#00CED1"), // DarkTurquoise
            Color.FromArgb("#9932CC"), // DarkOrchid
            Color.FromArgb("#B22222"), // FireBrick
            Color.FromArgb("#FF7F50"), // Coral
            Color.FromArgb("#6495ED"), // CornflowerBlue
            Color.FromArgb("#DC143C"), // Crimson
            Color.FromArgb("#2E8B57"), // SeaGreen
            Color.FromArgb("#FF4500"), // OrangeRed
            Color.FromArgb("#DA70D6"), // Orchid
            Color.FromArgb("#7CFC00"), // LawnGreen
            Color.FromArgb("#4169E1"), // RoyalBlue
            Color.FromArgb("#8FBC8F"), // DarkSeaGreen
            Color.FromArgb("#FFB6C1"), // LightPink
            Color.FromArgb("#20B2AA"), // LightSeaGreen
            Color.FromArgb("#87CEFA"), // LightSkyBlue
            Color.FromArgb("#778899"), // LightSlateGray
        };

        // ����� ������� ��� ��� RGB: ���� ����� ����� �����ѡ �� �����ѡ �� ������
        //colors = colors.OrderBy(c => c.Red)
        //               .ThenBy(c => c.Green)
        //               .ThenBy(c => c.Blue)
        //               .ToList();

        // ����� ���� �� ������ ���� ��� �������
        Grid grid = new Grid
        {
            RowSpacing = 8,
            ColumnSpacing = 8,
            Padding = 10,
            BackgroundColor = Colors.Transparent
        };

        // ����� ��� ������� (����� ����� ����� ��� �����)
        int columns = 5;
        int rows = (int)Math.Ceiling((double)colors.Count / columns);

        // ����� ������� ������� ��������
        for (int c = 0; c < columns; c++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        // ����� ������� ������ ��������
        for (int r = 0; r < rows; r++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }

        // ������� Border ���� ������� ��� ��� ����� �� ����� ����� (Margins)
        for (var i = 0; i < colors.Count; i++)
        {
            Color selectedColor = colors[i];

            // ����� ���� Border �� StrokeShape ���� ������ �������
            Border colorBorder = new Border
            {
                BackgroundColor = selectedColor,
                StrokeShape = new RoundRectangle
                {
                    // ��� �� ����� ��������� 50� ��� ����� CornerRadius ��� 25 ���� ����� �������
                    CornerRadius = new CornerRadius(25)
                },
                Stroke = null,
                StrokeThickness = 0,
                Margin = new Thickness(5),
                WidthRequest = 50,
                HeightRequest = 50
            };

            // ����� Gesture ������� ��� ����� ��� �����
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Command = new Command(() => OnColorSelected(selectedColor));
            colorBorder.GestureRecognizers.Add(tapGesture);

            grid.Children.Add(colorBorder);
            Grid.SetColumn(colorBorder, i % columns);
            Grid.SetRow(colorBorder, i / columns);
        }

        // ����� ������ ������ ������� ��������
        Content = grid;
    }

    private void OnColorSelected(Color color)
    {
        CloseAsync(color);
    }
}
