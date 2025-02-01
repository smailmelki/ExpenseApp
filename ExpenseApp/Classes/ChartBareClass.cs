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
            float width = dirtyRect.Width;
            float height = dirtyRect.Height;

            float beamYEnd = height - 10;

            float p = width / data.Count;
            if (p > 50)
                p = 50;
            float margin = (width - p * data.Count) / 2;

            float x = p / 2;
            double max = data.Max(d => d.Cost);
            float d = (float)max / (height - 20);

            // رسم العارضة
            canvas.StrokeSize = p - 20;
            foreach (var item in data)
            {
                canvas.StrokeColor = Color.FromArgb(item.color);
                canvas.DrawLine(margin + x, beamYEnd - (float)item.Cost / d, margin + x, beamYEnd);
                canvas.DrawString(item.Title, margin + x - p / 2, beamYEnd + 10, p, 10, HorizontalAlignment.Center, VerticalAlignment.Center);
                x += p;
            }
        }
    }
}