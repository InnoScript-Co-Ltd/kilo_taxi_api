using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Implementation;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TopUpTransactionController : ControllerBase
{
    private readonly LoggerHelper _logHelper;
    private readonly ITopUpTransactionRepository _topUpTransactionRepository;
    private readonly IConfiguration _configuration;
    private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
    private readonly List<string> _allowedMimeTypes = new List<string> { "image/jpeg", "image/png" };
    private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB
    private const string flagDomain = "screenShoot";

    public TopUpTransactionController(ITopUpTransactionRepository topUpTransactionRepository, IConfiguration configuration)
    {
        _logHelper = LoggerHelper.Instance;
        _topUpTransactionRepository = topUpTransactionRepository;
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult<ResponseDTO<TopUpTransactionPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
    {
        try
        {
            var responseDTO = _topUpTransactionRepository.GetAllTopUpTransactions(pageSortParam);
            if (!responseDTO.Payload.TopUpTransactions.Any())
            {
                return NoContent();
            }
            return Ok(responseDTO);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }


    [HttpPost]
    public async Task<ActionResult<ResponseDTO<TopUpTransactionInfoDTO>>> Post(TopUpTransactionFormDTO topUpTransactionFormDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var fileUploadHelper = new FileUploadHelper(_configuration, _allowedExtensions, _allowedMimeTypes, _maxFileSize);
            if (!fileUploadHelper.ValidateFile(topUpTransactionFormDTO.File_TransactionScreenShoot, true, flagDomain, out var resolvedFilePath, out var errorMessage))
            {
                return BadRequest(errorMessage);
            }
            topUpTransactionFormDTO.TransactionScreenShoot = resolvedFilePath;

            var createdTransaction = _topUpTransactionRepository.CreateTopUpTransaction(topUpTransactionFormDTO);

            if (topUpTransactionFormDTO.File_TransactionScreenShoot != null && topUpTransactionFormDTO.File_TransactionScreenShoot.Length > 0)
            {
                string filePath = await fileUploadHelper.SaveFileAsync(topUpTransactionFormDTO.File_TransactionScreenShoot, flagDomain, topUpTransactionFormDTO.Id.ToString(), resolvedFilePath);
            }
            
            var response = new ResponseDTO<TopUpTransactionInfoDTO>
            {
                StatusCode = 201,
                Message = "Topup Transaction Register Success.",
                Payload = createdTransaction,
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


}
