using ExpenseApp.Pages;

namespace ExpenseApp.Classes
{
    public class ChartBareClass : IDrawable
    {
        List<catTree> data;
        public ChartBareClass(List<catTree> Data)
        {
            this.data = Data;
        }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float width = dirtyRect.Width;//340
            float height = dirtyRect.Height;//180

            float baseY = height - 30;//150

            float p = width / data.Count;
            if (p > 50)
                p = 50;//50
            float margin = (width - p * data.Count) / 2;//45

            float x = p / 2;//25
            double max = data.Max(d => d.Cost);//1200
            float d = (float)max / (height - 50);//8

            // رسم العارضة
            canvas.StrokeSize = p - 20;//30
            foreach (var item in data)
            {
                canvas.StrokeColor = Color.FromArgb(item.color);
                canvas.StrokeLineCap = LineCap.Round;
                canvas.DrawLine(margin + x, baseY - (float)item.Cost / d, margin + x, baseY);

                canvas.FontColor = Application.Current?.UserAppTheme == AppTheme.Dark ? Colors.White : Colors.Black;
                canvas.DrawString(item.Title, margin + x - p / 2,baseY + 20, p, 10, HorizontalAlignment.Center, VerticalAlignment.Center);
                x += p;//175
            }
        }
    }
}