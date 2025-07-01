namespace CRM_AutoFlow.Application.DTOs
{
    public class ManagerPerformanceReport
    {
        public string id {  get; set; }

        public string managerFullName { get; set; }

        public int testDrives { get; set; }

        public int dealsRegistered { get; set; }

        public int dealsCompleted { get; set; }

        public string conversionRate { get; set; }

        public string revenue {  get; set; }

    }
}
