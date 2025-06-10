using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM_AutoFlow.Presentation.Controllers
{
    [Route("car")]
    [ApiController]
    public class CarsController : Controller
    {
        private readonly ICarRepository _carRepository;
        public CarsController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }
        [HttpGet("getallcar")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCar()
        {
            var response = await _carRepository.GetAllCars();
            return Ok(response);
        }

        [HttpPost ("add")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,DIRECTOR")]
        public async Task<IActionResult> AddNewCar([FromBody] CarDTO carDto)
        {
            var response = await _carRepository.AddCar(carDto);
            if(!response.Success)
                return BadRequest(new { Error = response.Error} );

            return Ok(new { carId = response.Data});
        }
    }
}
