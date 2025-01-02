namespace Subscribe_Store_Job.Models
{
    public class Inventory
    {
        public long MaterialPk { get; set; }
        public List<OrderReleasePks> OrderReleasePks { get; set; }
    }
    public class OrderReleasePks
    {
        public long OrderReleasePk { get; set; }
    }
}
