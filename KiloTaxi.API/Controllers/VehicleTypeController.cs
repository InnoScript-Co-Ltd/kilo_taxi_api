using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
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
        public ActionResult<ResponseDTO<VehicleTypePagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto =
                    _vehicleTypeRepository.GetAllVehicleTypes(pageSortParam);
                if (!responseDto.Payload.VehicleTypes.Any())
                {
                    return NoContent();
                }
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<VehicleTypeController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<VehicleTypeInfoDTO>> Get(int id)
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
                
                ResponseDTO<VehicleTypeInfoDTO> responseDto = new ResponseDTO<VehicleTypeInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "vehicle type retrieved successfully.",
                    TimeStamp = DateTime.Now,
                    Payload = result,
                };
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<VehicleTypeController>
        [HttpPost]
        public ActionResult<ResponseDTO<VehicleTypeInfoDTO>> Post([FromBody] VehicleTypeFormDTO vehicleTypeFormDTO)
        {
            try
            {
                if (vehicleTypeFormDTO == null)
                {
                    return BadRequest();
                }

                var createdVehicleType = _vehicleTypeRepository.AddVehicleType(vehicleTypeFormDTO);
                
                var response = new ResponseDTO<VehicleTypeInfoDTO>
                {
                    StatusCode = 201,
                    Message = "vehicle type Register Success.",
                    Payload = createdVehicleType,
                    TimeStamp = DateTime.Now,
                };
                return response;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<VehicleTypeController>/5
        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<VehicleTypeInfoDTO>> Put(int id, [FromBody] VehicleTypeFormDTO vehicleTypeFormDTO)
        {
            try
            {
                if (vehicleTypeFormDTO == null || id != vehicleTypeFormDTO.Id)
                {
                    return BadRequest();
                }

                var result = _vehicleTypeRepository.UpdateVehicleType(vehicleTypeFormDTO);
                if (!result)
                {
                    return NotFound();
                }
                
                ResponseDTO<VehicleTypeInfoDTO> responseDto = new ResponseDTO<VehicleTypeInfoDTO>
                {
                    StatusCode = 200,
                    Message = "vehicle type Updated Successfully.",
                    TimeStamp = DateTime.Now,
                    Payload = null,
                };
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE api/<VehicleTypeController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<VehicleTypeInfoDTO>> Delete(int id)
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

                ResponseDTO<VehicleTypeInfoDTO> responseDto = new ResponseDTO<VehicleTypeInfoDTO>
                {
                    StatusCode = 204,
                    Message = "vehicle type Deleted Successfully.",
                    TimeStamp = DateTime.Now,
                    Payload = null,
                };
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
