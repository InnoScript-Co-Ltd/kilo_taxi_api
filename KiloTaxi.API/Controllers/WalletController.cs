using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
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
    public ActionResult<ResponseDTO<WalletPagingDTO>> GetAll([FromQuery] PageSortParam pageSortParam)
    {
        try
        {
            var responseDto = _walletRepository.GetAllWallets(pageSortParam);
            if (!responseDto.Payload.Wallets.Any())
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
    public ActionResult<ResponseDTO<WalletInfoDTO>> Get(int id)
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
            
            ResponseDTO<WalletInfoDTO> responseDto = new ResponseDTO<WalletInfoDTO>
            {
                StatusCode = Ok().StatusCode,
                Message = "wallet retrieved successfully.",
                Payload = wallet,
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
    public ActionResult<ResponseDTO<WalletInfoDTO>> Create([FromBody] WalletFormDTO walletFormDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdWallet = _walletRepository.CreateWallet(walletFormDTO);
            
            var response = new ResponseDTO<WalletInfoDTO>
            {
                StatusCode = 201,
                Message = "wallet Register Success.",
                Payload = createdWallet,
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
    public ActionResult<ResponseDTO<WalletInfoDTO>> Update(int id, [FromBody] WalletFormDTO walletFormDTO)
    {
        try
        {
            if (id != walletFormDTO.Id)
            {
                return BadRequest("Wallet ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isUpdated = _walletRepository.UpdateWallet(walletFormDTO);
            if (!isUpdated)
            {
                return NotFound();
            }

            ResponseDTO<WalletInfoDTO> responseDto = new ResponseDTO<WalletInfoDTO>
            {
                StatusCode = 200,
                Message = "Wallet Updated Successfully.",
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
    public ActionResult<ResponseDTO<OrderInfoDTO>> Delete(int id)
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

            ResponseDTO<WalletInfoDTO> responseDto = new ResponseDTO<WalletInfoDTO>
            {
                StatusCode = 204,
                Message = "wallet Deleted Successfully.",
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
