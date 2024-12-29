using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExtraDemandController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IExtraDemandRepository _extraDemandRepository;

        public ExtraDemandController(IExtraDemandRepository _extraDemandRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _extraDemandRepository = _extraDemandRepository;
        }

        // GET: api/<ExtraDemandController>
        [HttpGet]
        public ActionResult<ExtraDemandPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                ExtraDemandPagingDTO extraDemandPagingDTO = _extraDemandRepository.GetAllExtraDemand(
                    pageSortParam
                );
                if (!extraDemandPagingDTO.ExtraDemands.Any())
                {
                    return NoContent();
                }
                return Ok(extraDemandPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<ExtraDemandController>/5
        [HttpGet("{id}")]
        public ActionResult<ExtraDemandDTO> Get(int id)
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<ExtraDemandController>
        [HttpPost]
        public ActionResult<ExtraDemandDTO> Post([FromBody] ExtraDemandDTO extraDemandDTO)
        {
            try
            {
                if (extraDemandDTO == null)
                {
                    return BadRequest();
                }

                var createdExtraDemand = _extraDemandRepository.CreateExtraDemand(extraDemandDTO);
                return CreatedAtAction(
                    nameof(Get),
                    new { id = createdExtraDemand.Id },
                    createdExtraDemand
                );
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<ExtraDemandController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] ExtraDemandDTO extraDemandDTO)
        {
            try
            {
                if (extraDemandDTO == null || id != extraDemandDTO.Id)
                {
                    return BadRequest();
                }

                var result = _extraDemandRepository.UpdateExtraDemand(extraDemandDTO);
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

        // DELETE api/<ExtraDemandController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
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
