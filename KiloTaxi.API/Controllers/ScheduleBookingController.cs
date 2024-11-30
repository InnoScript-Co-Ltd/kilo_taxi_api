using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
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
        public ActionResult<ScheduleBookingPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                ScheduleBookingPagingDTO scheduleBookingPagingDTO = _scheduleBookingRepository.GetAllScheduleBooking(pageSortParam);
                if (!scheduleBookingPagingDTO.ScheduleBookings.Any())
                {
                    return NoContent();
                }
                return Ok(scheduleBookingPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<ScheduleBookingController>/5
        [HttpGet("{id}")]
        public ActionResult<ScheduleBookingDTO> Get(int id)
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<ScheduleBookingController>
        [HttpPost]
        public ActionResult<ScheduleBookingDTO> Post([FromBody] ScheduleBookingDTO scheduleBookingDTO)
        {
            try
            {
                if (scheduleBookingDTO == null)
                {
                    return BadRequest();
                }

                var createdScheduleBooking = _scheduleBookingRepository.AddScheduleBooking(scheduleBookingDTO);
                return CreatedAtAction(nameof(Get), new { id = createdScheduleBooking.Id }, createdScheduleBooking);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<ScheduleBookingController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] ScheduleBookingDTO scheduleBookingDTO)
        {
            try
            {
                if (scheduleBookingDTO == null || id != scheduleBookingDTO.Id)
                {
                    return BadRequest();
                }

                var result = _scheduleBookingRepository.UpdateScheduleBooking(scheduleBookingDTO);
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

        // DELETE api/<ScheduleBookingController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
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
