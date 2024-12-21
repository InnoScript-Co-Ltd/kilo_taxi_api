using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly LoggerHelper _logHelper;
    private readonly IWalletRepository _walletRepository;
    private readonly IConfiguration _configuration;

    public WalletController(IWalletRepository walletRepository, IConfiguration configuration)
    {
        _logHelper = LoggerHelper.Instance;
        _walletRepository = walletRepository;
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult<IEnumerable<WalletPagingDTO>> GetAll([FromQuery] PageSortParam pageSortParam)
    {
        try
        {
            WalletPagingDTO walletPagingDTO = _walletRepository.GetAllWallets(pageSortParam);
            if (!walletPagingDTO.Wallets.Any())
            {
                return NoContent();
            }
            return Ok(walletPagingDTO);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<WalletDTO> Get(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest("Invalid Wallet ID.");
            }

            var wallet = _walletRepository.GetWalletById(id);
            if (wallet == null)
            {
                return NotFound();
            }

            return Ok(wallet);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public ActionResult<WalletDTO> Create([FromBody] WalletDTO walletDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdWallet = _walletRepository.CreateWallet(walletDTO);

            return CreatedAtAction(nameof(Get), new { id = createdWallet.Id }, createdWallet);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] WalletDTO walletDTO)
    {
        try
        {
            if (id != walletDTO.Id)
            {
                return BadRequest("Wallet ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isUpdated = _walletRepository.UpdateWallet(walletDTO);
            if (!isUpdated)
            {
                return NotFound();
            }

            return Ok("Wallet updated successfully.");
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
            var wallet = _walletRepository.GetWalletById(id);
            if (wallet == null)
            {
                return NotFound();
            }

            var isDeleted = _walletRepository.DeleteWallet(id);
            if (!isDeleted)
            {
                return StatusCode(500, "An error occurred while deleting the payment channel.");
            }

            return Ok($"Payment channel with ID {id} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
