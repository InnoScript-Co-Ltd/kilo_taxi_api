using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
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
        public ActionResult<ReasonPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                ReasonPagingDTO reasonPagingDTO = _reasonRepository.GetAllReason(pageSortParam);
                if (!reasonPagingDTO.Reasons.Any())
                {
                    return NoContent();
                }
                return Ok(reasonPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<ReasonController>/5
        [HttpGet("{id}")]
        public ActionResult<ReasonDTO> Get(int id)
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<ReasonController>
        [HttpPost]
        public ActionResult<ReasonDTO> Post([FromBody] ReasonDTO reasonDTO)
        {
            try
            {
                if (reasonDTO == null)
                {
                    return BadRequest();
                }

                var createdReason = _reasonRepository.CreateReason(reasonDTO);
                return CreatedAtAction(nameof(Get), new { id = createdReason.Id }, createdReason);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<ReasonController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] ReasonDTO reasonDTO)
        {
            try
            {
                if (reasonDTO == null || id != reasonDTO.Id)
                {
                    return BadRequest();
                }

                var result = _reasonRepository.UpdateReason(reasonDTO);
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

        // DELETE api/<ReasonController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<ReasonDTO>> Delete(int id)
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
                ResponseDTO<ReasonDTO> responseDto = new ResponseDTO<ReasonDTO>();
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
