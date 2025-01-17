using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly ISmsRepository _smsRepository;

        public SmsController(ISmsRepository smsRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _smsRepository = smsRepository;
        }

        // GET: api/<SmsController>
        [HttpGet]
        public ActionResult<SmsPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                SmsPagingDTO smsPagingDTO = _smsRepository.GetAllSms(pageSortParam);
                if (!smsPagingDTO.Sms.Any())
                {
                    return NoContent();
                }
                return Ok(smsPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<SmsController>/5
        [HttpGet("{id}")]
        public ActionResult<SmsDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _smsRepository.GetSmsById(id);
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

        // POST api/<SmsController>
        [HttpPost]
        public ActionResult<SmsDTO> Post([FromBody] SmsDTO smsDTO)
        {
            try
            {
                if (smsDTO == null)
                {
                    return BadRequest();
                }

                var createdSms = _smsRepository.CreateSms(smsDTO);
                return CreatedAtAction(nameof(Get), new { id = createdSms.Id }, createdSms);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<SmsController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] SmsDTO smsDTO)
        {
            try
            {
                if (smsDTO == null || id != smsDTO.Id)
                {
                    return BadRequest();
                }

                var result = _smsRepository.UpdateSms(smsDTO);
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

        // DELETE api/<SmsController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<SmsDTO>> Delete(int id)
        {
            try
            {
                var sms = _smsRepository.GetSmsById(id);
                if (sms == null)
                {
                    return NotFound();
                }

                var result = _smsRepository.DeleteSms(id);
                if (!result)
                {
                    return NotFound();
                }
                ResponseDTO<SmsDTO> responseDto = new ResponseDTO<SmsDTO>();
                responseDto.StatusCode = 204;
                responseDto.Message = "Reason deleted";
                responseDto.TimeStamp = DateTime.Now;
                return responseDto;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
