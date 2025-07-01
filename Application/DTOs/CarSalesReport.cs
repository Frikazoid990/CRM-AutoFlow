namespace CRM_AutoFlow.Application.DTOs
{
    public class CarSalesReport
    {
        public string id { get; set; }

        public string modelBrand { get; set; }

        public int unitsSold { get; set; }

        public decimal avgPrice { get; set; }

        public decimal totalRevenue { get; set; }

    }
}
