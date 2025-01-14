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
        public ActionResult<ResponseDTO<SmsPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto = _smsRepository.GetAllSms(pageSortParam);
                if (!responseDto.Payload.Sms.Any())
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

        // GET: api/<SmsController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<SmsInfoDTO>> Get(int id)
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
                
                ResponseDTO<SmsInfoDTO> responseDto = new ResponseDTO<SmsInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "sms retrieved successfully.",
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

        // POST api/<SmsController>
        [HttpPost]
        public ActionResult<ResponseDTO<SmsInfoDTO>> Post([FromBody] SmsFormDTO smsFormDTO)
        {
            try
            {
                if (smsFormDTO == null)
                {
                    return BadRequest();
                }

                var createdSms = _smsRepository.CreateSms(smsFormDTO);
                
                var response = new ResponseDTO<SmsInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "sms Register Success.",
                    Payload = createdSms,
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

        // PUT api/<SmsController>/5
        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<SmsInfoDTO>> Put(int id, [FromBody] SmsFormDTO smsFormDTO)
        {
            try
            {
                if (smsFormDTO == null || id != smsFormDTO.Id)
                {
                    return BadRequest();
                }

                var result = _smsRepository.UpdateSms(smsFormDTO);
                if (!result)
                {
                    return NotFound();
                }
                ResponseDTO<SmsInfoDTO> responseDto = new ResponseDTO<SmsInfoDTO>
                {
                    StatusCode = 200,
                    Message = "sms Updated Successfully.",
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

        // DELETE api/<SmsController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<SmsInfoDTO>> Delete(int id)
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

                ResponseDTO<SmsInfoDTO> responseDto = new ResponseDTO<SmsInfoDTO>
                {
                    StatusCode = 200,
                    Message = "sms Deleted Successfully.",
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
