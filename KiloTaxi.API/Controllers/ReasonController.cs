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
    public class ReasonController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IReasonRepository _reasonRepository;

        public ReasonController(IReasonRepository reasonRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _reasonRepository = reasonRepository;
        }

        // GET: api/<ReasonController>
        [HttpGet]
        public ActionResult<ResponseDTO<ReasonPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto = _reasonRepository.GetAllReason(pageSortParam);
                if (!responseDto.Payload.Reasons.Any())
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

        // GET: api/<ReasonController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<ReasonInfoDTO>> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _reasonRepository.GetReasonById(id);
                if (result == null)
                {
                    return NotFound();
                }
                
                ResponseDTO<ReasonInfoDTO> responseDto = new ResponseDTO<ReasonInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "reason retrieved successfully.",
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

        // POST api/<ReasonController>
        [HttpPost]
        public ActionResult<ResponseDTO<ReasonInfoDTO>> Post([FromBody] ReasonFormDTO reasonFormDTO)
        {
            try
            {
                if (reasonFormDTO == null)
                {
                    return BadRequest();
                }

                var createdReason = _reasonRepository.CreateReason(reasonFormDTO);
                
                var response = new ResponseDTO<ReasonInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "Reason Register Success.",
                    Payload = createdReason,
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

        // PUT api/<ReasonController>/5
        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<ReasonInfoDTO>> Put(int id, [FromBody] ReasonFormDTO reasonFormDTO)
        {
            try
            {
                if (reasonFormDTO == null || id != reasonFormDTO.Id)
                {
                    return BadRequest();
                }

                var result = _reasonRepository.UpdateReason(reasonFormDTO);
                if (!result)
                {
                    return NotFound();
                }
                
                ResponseDTO<ReasonInfoDTO> responseDto = new ResponseDTO<ReasonInfoDTO>
                {
                    StatusCode = 200,
                    Message = "Reason Updated Successfully.",
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

        // DELETE api/<ReasonController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<ReasonInfoDTO>> Delete(int id)
        {
            try
            {
                var reason = _reasonRepository.GetReasonById(id);
                if (reason == null)
                {
                    return NotFound();
                }

                var result = _reasonRepository.DeleteReason(id);
                if (!result)
                {
                    return NotFound();
                }

                ResponseDTO<ReasonInfoDTO> responseDto = new ResponseDTO<ReasonInfoDTO>
                {
                    StatusCode = 200,
                    Message = "Reason Deleted Successfully.",
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
