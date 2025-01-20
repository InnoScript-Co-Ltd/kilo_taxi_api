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
    public class ScheduleBookingController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IScheduleBookingRepository _scheduleBookingRepository;

        public ScheduleBookingController(IScheduleBookingRepository scheduleBookingRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _scheduleBookingRepository = scheduleBookingRepository;
        }

        // GET: api/<ScheduleBookingController>
        [HttpGet]
        public ActionResult<ResponseDTO<ScheduleBookingPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto =
                    _scheduleBookingRepository.GetAllScheduleBooking(pageSortParam);
                if (!responseDto.Payload.ScheduleBookings.Any())
                {
                    return NoContent();
                }
                return responseDto;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<ScheduleBookingController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<ScheduleBookingInfoDTO>> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _scheduleBookingRepository.getScheduleBookingById(id);
                if (result == null)
                {
                    return NotFound();
                }
                
                ResponseDTO<ScheduleBookingInfoDTO> responseDto = new ResponseDTO<ScheduleBookingInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "schedule booking retrieved successfully.",
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

        // POST api/<ScheduleBookingController>
        [HttpPost]
        public ActionResult<ResponseDTO<ScheduleBookingInfoDTO>> Post(
            [FromBody] ScheduleBookingFormDTO scheduleBookingFormDTO
        )
        {
            try
            {
                if (scheduleBookingFormDTO == null)
                {
                    return BadRequest();
                }

                var createdScheduleBooking = _scheduleBookingRepository.AddScheduleBooking(
                    scheduleBookingFormDTO
                );
                
                var response = new ResponseDTO<ScheduleBookingInfoDTO>
                {
                    StatusCode = 201,
                    Message = "schedule booking Register Success.",
                    TimeStamp = DateTime.Now,
                    Payload = createdScheduleBooking,
                };
                return response;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<ScheduleBookingController>/5
        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<ScheduleBookingInfoDTO>> Put(int id, [FromBody] ScheduleBookingFormDTO scheduleBookingFormDTO)
        {
            try
            {
                if (scheduleBookingFormDTO == null || id != scheduleBookingFormDTO.Id)
                {
                    return BadRequest();
                }

                var result = _scheduleBookingRepository.UpdateScheduleBooking(scheduleBookingFormDTO);
                if (!result)
                {
                    return NotFound();
                }
                
                ResponseDTO<ScheduleBookingInfoDTO> responseDto = new ResponseDTO<ScheduleBookingInfoDTO>
                {
                    StatusCode = 200,
                    Message = "schedule booking Updated Successfully.",
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

        // DELETE api/<ScheduleBookingController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<ScheduleBookingInfoDTO>> Delete(int id)
        {
            try
            {
                var scheduleBooking = _scheduleBookingRepository.getScheduleBookingById(id);
                if (scheduleBooking == null)
                {
                    return NotFound();
                }

                var result = _scheduleBookingRepository.DeleteScheduleBooking(id);
                if (!result)
                {
                    return NotFound();
                }

                ResponseDTO<ScheduleBookingInfoDTO> responseDto = new ResponseDTO<ScheduleBookingInfoDTO>
                {
                    StatusCode = 204,
                    Message = "schedule booking Deleted Successfully.",
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
