using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VehicleTypeController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IVehicleTypeRepository _vehicleTypeRepository;

        public VehicleTypeController(IVehicleTypeRepository vehicleTypeRepositoryRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _vehicleTypeRepository = vehicleTypeRepositoryRepository;
        }

        // GET: api/<VehicleTypeController>
        [HttpGet]
        public ActionResult<VehicleTypePagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                VehicleTypePagingDTO vehicleTypePagingDTO =
                    _vehicleTypeRepository.GetAllVehicleTypes(pageSortParam);
                if (!vehicleTypePagingDTO.VehicleTypes.Any())
                {
                    return NoContent();
                }
                return Ok(vehicleTypePagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<VehicleTypeController>/5
        [HttpGet("{id}")]
        public ActionResult<VehicleTypeDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _vehicleTypeRepository.GetVehicleTypeById(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<VehicleTypeController>
        [HttpPost]
        public ActionResult<VehicleTypeDTO> Post([FromBody] VehicleTypeDTO vehicleTypeDTO)
        {
            try
            {
                if (vehicleTypeDTO == null)
                {
                    return BadRequest();
                }

                var createdVehicleType = _vehicleTypeRepository.AddVehicleType(vehicleTypeDTO);
                return CreatedAtAction(
                    nameof(Get),
                    new { id = createdVehicleType.Id },
                    createdVehicleType
                );
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<VehicleTypeController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] VehicleTypeDTO vehicleTypeDTO)
        {
            try
            {
                if (vehicleTypeDTO == null || id != vehicleTypeDTO.Id)
                {
                    return BadRequest();
                }

                var result = _vehicleTypeRepository.UpdateVehicleType(vehicleTypeDTO);
                if (!result)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE api/<VehicleTypeController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var vehicleType = _vehicleTypeRepository.GetVehicleTypeById(id);
                if (vehicleType == null)
                {
                    return NotFound();
                }

                var result = _vehicleTypeRepository.DeleteVehicleType(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
