using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Application.Helper;
using System.Collections;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface ICarRepository
    {
        public Task<IEnumerable> GetAllCars();
        public Task<Result<Guid>> AddCar(CarDTO carDto);
    }
}
