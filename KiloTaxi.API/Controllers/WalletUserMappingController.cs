using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class WalletUserMappingController : ControllerBase
{
    private readonly IWalletUserMappingRepository _walletUserMappingRepository;
    private readonly LoggerHelper _logHelper;

    public WalletUserMappingController(IWalletUserMappingRepository walletUserMappingRepository)
    {
        _walletUserMappingRepository = walletUserMappingRepository;
        _logHelper = LoggerHelper.Instance;
    }

    [HttpPost]
    public ActionResult<WalletUserMappingDTO> Create([FromBody] WalletUserMappingDTO walletUserMappingDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdMapping = _walletUserMappingRepository.CreateWalletUserMapping(walletUserMappingDTO);
            return CreatedAtAction(nameof(GetById), new { id = createdMapping.Id }, createdMapping);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while creating WalletUserMapping.");
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] WalletUserMappingDTO walletUserMappingDTO)
    {
        try
        {
            if (id != walletUserMappingDTO.Id)
                return BadRequest("ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isUpdated = _walletUserMappingRepository.UpdateWalletUserMapping(walletUserMappingDTO);
            if (!isUpdated)
                return NotFound();

            return Ok("WalletUserMapping updated successfully.");
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while updating WalletUserMapping.");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<WalletUserMappingDTO> GetById(int id)
    {
        try
        {
            var mapping = _walletUserMappingRepository.GetWalletUserMappingById(id);
            if (mapping == null)
                return NotFound();

            return Ok(mapping);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while fetching WalletUserMapping.");
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<WalletUserMappingDTO>> GetAll()
    {
        try
        {
            var mappings = _walletUserMappingRepository.GetAllWalletUserMappings();
            if (!mappings.Any())
                return NoContent();

            return Ok(mappings);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while fetching all WalletUserMappings.");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var isDeleted = _walletUserMappingRepository.DeleteWalletUserMapping(id);
            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while deleting WalletUserMapping.");
        }
    }
}
