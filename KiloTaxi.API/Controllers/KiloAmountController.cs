using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class KiloAmountController : ControllerBase
{
    LoggerHelper _logHelper;
    private readonly IKiloAmountRepository _kiloAmountRepository;

    public KiloAmountController(IKiloAmountRepository kiloAmountRepository)
    {
        _logHelper = LoggerHelper.Instance;
        _kiloAmountRepository = kiloAmountRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<KiloAmountPagingDTO>> GetKiloAmountList(
        [FromQuery] PageSortParam pageSortParam
    )
    {
        try
        {
            KiloAmountPagingDTO kiloAmountPagingDto = _kiloAmountRepository.GetAllKiloAmountList(
                pageSortParam
            );
            if (!kiloAmountPagingDto.KiloAmounts.Any())
            {
                return NoContent();
            }
            return Ok(kiloAmountPagingDto);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<KiloAmountDTO> Get(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest("Invalid KiloAmount ID.");
            }

            var kiloAmount = _kiloAmountRepository.GetKiloAmountById(id);
            if (kiloAmount == null)
            {
                return NotFound();
            }

            return Ok(kiloAmount);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public ActionResult<KiloAmountDTO> Create([FromBody] KiloAmountDTO kiloAmountDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdKiloAmount = _kiloAmountRepository.CreateKiloAmount(kiloAmountDTO);

            return CreatedAtAction(
                nameof(Get),
                new { id = createdKiloAmount.Id },
                createdKiloAmount
            );
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Put([FromRoute] int id, KiloAmountDTO kiloAmountDTO)
    {
        try
        {
            if (id != kiloAmountDTO.Id)
            {
                return BadRequest("KiloAmount ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isUpdated = _kiloAmountRepository.UpdateKiloAmount(kiloAmountDTO);
            if (!isUpdated)
            {
                return NotFound();
            }

            return Ok("KiloAmount updated successfully.");
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var kiloAmount = _kiloAmountRepository.GetKiloAmountById(id);
            if (kiloAmount == null)
            {
                return NotFound();
            }

            var isDeleted = _kiloAmountRepository.DeleteKiloAmount(id);
            if (!isDeleted)
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
