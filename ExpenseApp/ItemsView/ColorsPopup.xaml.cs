
namespace ExpenseApp.ItemsView;

public partial class ColorsPopup
{
	public ColorsPopup()
	{
		InitializeComponent();
		InitializeGrid();
    }

    private void InitializeGrid()
    {
        var colors = new List<Color>
        {
            Color.FromArgb("#00FFFF"), // 01 Aqua
            Color.FromArgb("#FFE4C4"), // 02 Bisque
            Color.FromArgb("#0000FF"), // 03 Blue
            Color.FromArgb("#8A2BE2"), // 04 BlueViolet
            Color.FromArgb("#A52A2A"), // 05 Brown
            Color.FromArgb("#7FFF00"), // 06 Chartreuse
            Color.FromArgb("#D2691E"), // 07 Chocolate
            Color.FromArgb("#9ACD32"), // 08 YellowGreen
            Color.FromArgb("#FF8C00"), // 09 DarkOrange
            Color.FromArgb("#FF1493"), // 10 DeepPink
            Color.FromArgb("#9400D3"), // 11 DarkViolet
            Color.FromArgb("#FFD700"), // 12 Gold
            Color.FromArgb("#ADFF2F"), // 13 GreenYellow
            Color.FromArgb("#F08080"), // 14 LightCoral
            Color.FromArgb("#FFA5FF"), // 15 PinkLavender
            Color.FromArgb("#FF0000"), // 16 Red
            Color.FromArgb("#C0C0C0"), // 17 Silver
            Color.FromArgb("#FF6347"), // 18 Tomato
            Color.FromArgb("#FFFF00"), // 19 Yellow
            Color.FromArgb("#00FF00"), // 20 Green
        };

        Grid grid = new Grid
        {
            RowSpacing = 3,
            ColumnSpacing = 3,
            Padding = 2,
            BackgroundColor = Colors.Transparent
        };

        //  ÕœÌœ ⁄œœ «·√⁄„œ… · Ã‰» ≈÷«›… ⁄œœ “«∆œ „‰ «·√⁄„œ… Ê«·’›Ê›
        int columns = 5;
        int rows = (int)Math.Ceiling((double)colors.Count / columns);

        // ≈‰‘«¡ «·√⁄„œ… Ê«·’›Ê› „”»ﬁ«
        for (int c = 0; c < columns; c++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
        for (int r = 0; r < rows; r++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }

        // ≈÷«›… «·√·Ê«‰ ≈·Ï «·‘»ﬂ…
        for (var i = 0; i < colors.Count - 1; i++)
        {
            var boxView = new BoxView
            {
                Color = colors[i],
               // CornerRadius = 20,
            };

            var tapGesture = new TapGestureRecognizer();
            Color selectedColor = colors[i]; // ≈‰‘«¡ ‰”Œ… „Õ·Ì… · À»Ì  «··Ê‰ «·’ÕÌÕ
            tapGesture.Command = new Command(() => OnColorSelected(selectedColor));
            boxView.GestureRecognizers.Add(tapGesture);

            grid.Children.Add(boxView);
            Grid.SetColumn(boxView, i % columns);
            Grid.SetRow(boxView, i / columns);
        }

        //  ⁄ÌÌ‰ `grid` ﬂ‹ `Content` ≈–« ﬂ«‰ Â–« œ«Œ· `ContentPage`
        Content = grid;
    }

    private void OnColorSelected(Color color)
    {
        CloseAsync(color);
    }
}