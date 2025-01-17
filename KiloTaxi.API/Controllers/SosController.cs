using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
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
    public ActionResult<ResponseDTO<SosPagingDTO>> GetSosList([FromQuery] PageSortParam pageSortParam)
    {
        try
        {
            var responseDto = _sosRepository.GetAllSosList(pageSortParam);
            if (!responseDto.Payload.Sos.Any())
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
    [HttpGet("{id}")]
    public ActionResult<ResponseDTO<SosInfoDTO>> Get(int id)
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

            ResponseDTO<SosInfoDTO> responseDto = new ResponseDTO<SosInfoDTO>
            {
                StatusCode = Ok().StatusCode,
                Message = "sos retrieved successfully.",
                Payload = sos,
            };
            return Ok(responseDto);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    
    [HttpPost]
    public ActionResult<ResponseDTO<SosInfoDTO>> Create([FromBody] SosFormDTO sosFormDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdSos = _sosRepository.CreateSos(sosFormDTO);

            var response = new ResponseDTO<SosInfoDTO>
            {
                StatusCode = Ok().StatusCode,
                Message = "sos Register Success.",
                Payload = createdSos,
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
    
    [HttpPut("{id}")]
    public  ActionResult<ResponseDTO<SosInfoDTO>> Put([FromRoute] int id, SosFormDTO sosFormDTO)
    {
        try
        {
            if (id != sosFormDTO.Id)
            {
                return BadRequest("Sos ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isUpdated = _sosRepository.UpdateSos(sosFormDTO);
            if (!isUpdated)
            {
                return NotFound();
            }

            ResponseDTO<SosInfoDTO> responseDto = new ResponseDTO<SosInfoDTO>
            {
                StatusCode = 200,
                Message = "sos Updated Successfully.",
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
    
    [HttpDelete("{id}")]
    public ActionResult<ResponseDTO<SosInfoDTO>> Delete(int id)
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

            ResponseDTO<SosInfoDTO> responseDto = new ResponseDTO<SosInfoDTO>
            {
                StatusCode = 200,
                Message = "sos Deleted Successfully.",
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