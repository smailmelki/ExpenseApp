namespace ExpenseApp.Models
{
    public class TreeItem
    {
        public int ID { get; set; }
        public string Title { get; set; } = "";
        public string? color { get; set; }
        public List<DetailItem>? Details { get; set; }
        public double Total
        {
            get
            {
                return Details?.Sum(detail => detail.Amount) ?? 0;
            }
        }
    }
}
