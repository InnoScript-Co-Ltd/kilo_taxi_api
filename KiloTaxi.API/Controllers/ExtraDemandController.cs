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
    public class ExtraDemandController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IExtraDemandRepository _extraDemandRepository;

        public ExtraDemandController(IExtraDemandRepository extraDemandRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _extraDemandRepository = extraDemandRepository;
        }

        // GET: api/<ExtraDemandController>
        [HttpGet]
        public ActionResult<ResponseDTO<ExtraDemandPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto = _extraDemandRepository.GetAllExtraDemand(
                    pageSortParam
                );
                if (!responseDto.Payload.ExtraDemands.Any())
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

        // GET: api/<ExtraDemandController>/5
        [HttpGet("{id}")]
        public ActionResult<ExtraDemandInfoDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _extraDemandRepository.GetExtraDemandById(id);
                if (result == null)
                {
                    return NotFound();
                }
                ResponseDTO<ExtraDemandInfoDTO> responseDto = new ResponseDTO<ExtraDemandInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "extra demands retrieved successfully.",
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

        // POST api/<ExtraDemandController>
        [HttpPost]
        public ActionResult<ResponseDTO<ExtraDemandInfoDTO>> Post([FromBody] ExtraDemandFormDTO extraDemandFormDTO)
        {
            try
            {
                if (extraDemandFormDTO == null)
                {
                    return BadRequest();
                }

                var createdExtraDemand = _extraDemandRepository.CreateExtraDemand(extraDemandFormDTO);
                
                var response = new ResponseDTO<ExtraDemandInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "extra demand Register Success.",
                    TimeStamp = DateTime.Now,
                    Payload = createdExtraDemand,
                };
                return response;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<ExtraDemandController>/5
        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<ExtraDemandInfoDTO>> Put(int id, [FromBody] ExtraDemandFormDTO extraDemandFormDTO)
        {
            try
            {
                if (extraDemandFormDTO == null || id != extraDemandFormDTO.Id)
                {
                    return BadRequest();
                }

                var result = _extraDemandRepository.UpdateExtraDemand(extraDemandFormDTO);
                if (!result)
                {
                    return NotFound();
                }
                
                ResponseDTO<ExtraDemandInfoDTO> responseDto = new ResponseDTO<ExtraDemandInfoDTO>
                {
                    StatusCode = 200,
                    Message = "extra demand Updated Successfully.",
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

        // DELETE api/<ExtraDemandController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<ExtraDemandInfoDTO>> Delete(int id)
        {
            try
            {
                var extraDemand = _extraDemandRepository.GetExtraDemandById(id);
                if (extraDemand == null)
                {
                    return NotFound();
                }

                var result = _extraDemandRepository.DeleteExtraDemand(id);
                if (!result)
                {
                    return NotFound();
                }

                ResponseDTO<ExtraDemandInfoDTO> responseDto = new ResponseDTO<ExtraDemandInfoDTO>
                {
                    StatusCode = 204,
                    Message = "extra demand Deleted Successfully.",
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
