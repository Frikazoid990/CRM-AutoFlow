namespace CRM_AutoFlow.Application.DTOs
{
    public class FunnelReport
    {
        public string id { get; set; }
        
        public string stage { get; set; }

        public int count { get; set; }

        public int percentage { get; set; }

        public string color { get; set; }
    }
}
