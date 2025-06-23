namespace CRM_AutoFlow.Application.DTOs
{
    public class ResponseTestDriveDTO
    {
        public Guid Id { get; set; }
        public DateTime PlannedDate { get; set; }
        public string Status { get; set; }


        //Вложенные DTO
        public CarShortInfoDTO Car { get; set; }
        public ClientShortInfoDTO Client { get; set; }
        public EmployeeShortInfoDTO? Employee { get; set; }

    }

    public class CarShortInfoDTO
    {
        public Guid Id { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string ImgPath { get; set; }

    }

    public class EmployeeShortInfoDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
    }

    public class ClientShortInfoDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }

}
