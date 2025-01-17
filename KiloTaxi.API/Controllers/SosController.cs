using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class SosController : ControllerBase
{
    LoggerHelper _logHelper;
    private readonly ISosRepository _sosRepository;

    public SosController(ISosRepository sosRepository)
    {
        _logHelper = LoggerHelper.Instance;
        _sosRepository = sosRepository;
    }
    [HttpGet]
    public ActionResult<IEnumerable<SosPagingDTO>> GetSosList([FromQuery] PageSortParam pageSortParam)
    {
        try
        {
            SosPagingDTO sosPagingDto = _sosRepository.GetAllSosList(pageSortParam);
            if (!sosPagingDto.Sos.Any())
            {
                return NoContent();
            }
            return Ok(sosPagingDto);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    [HttpGet("{id}")]
    public ActionResult<SosDTO> Get(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest("Invalid Sos ID.");
            }

            var sos = _sosRepository.GetSosById(id);
            if (sos == null)
            {
                return NotFound();
            }

            return Ok(sos);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    
    [HttpPost]
    public ActionResult<SosDTO> Create([FromBody] SosDTO sosDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdSos = _sosRepository.CreateSos(sosDTO);

            return CreatedAtAction(nameof(Get), new { id = createdSos.Id }, createdSos);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    
    [HttpPut("{id}")]
    public  ActionResult Put([FromRoute] int id, SosDTO sosDTO)
    {
        try
        {
            if (id != sosDTO.Id)
            {
                return BadRequest("Sos ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isUpdated = _sosRepository.UpdateSos(sosDTO);
            if (!isUpdated)
            {
                return NotFound();
            }

            return Ok("Sos updated successfully.");
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    
    [HttpDelete("{id}")]
    public ActionResult<ResponseDTO<SosDTO>> Delete(int id)
    {
        try
        {
            var sos = _sosRepository.GetSosById(id);
            if (sos == null)
            {
                return NotFound();
            }

            var isDeleted = _sosRepository.DeleteSos(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            ResponseDTO<SosDTO> responseDto = new ResponseDTO<SosDTO>();
            responseDto.StatusCode = 204;
            responseDto.Message = "Sos deleted";
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