using CRM_AutoFlow.Application.DTOs;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface IReportsService
    {
        public Task<List<CarSalesReport>> ReportCar(DateTime DateStart, DateTime DateEnd);

        public Task<List<ManagerPerformanceReport>> ReportManager(DateTime DateStart, DateTime DateEnd);

        public Task<List<FunnelReport>> ReportFunnel(DateTime DateStart, DateTime DateEnd);
    }
}
