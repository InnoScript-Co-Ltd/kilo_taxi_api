using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
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
    public ActionResult<TopUpTransactionPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
    {
        try
        {
            var transactions = _topUpTransactionRepository.GetAllTopUpTransactions(pageSortParam);
            if (!transactions.TopUpTransactions.Any())
            {
                return NoContent();
            }
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }


    [HttpPost]
    public async Task<ActionResult<TopUpTransactionDTO>> Post(TopUpTransactionDTO topUpTransactionDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var fileUploadHelper = new FileUploadHelper(_configuration, _allowedExtensions, _allowedMimeTypes, _maxFileSize);
            if (!fileUploadHelper.ValidateFile(topUpTransactionDTO.File_TransactionScreenShoot, true, flagDomain, out var resolvedFilePath, out var errorMessage))
            {
                return BadRequest(errorMessage);
            }
            topUpTransactionDTO.TransactionScreenShoot = resolvedFilePath;

            var createdTransaction = _topUpTransactionRepository.CreateTopUpTransaction(topUpTransactionDTO);
            if (topUpTransactionDTO.File_TransactionScreenShoot != null && topUpTransactionDTO.File_TransactionScreenShoot.Length > 0)
            {
                string filePath = await fileUploadHelper.SaveFileAsync(topUpTransactionDTO.File_TransactionScreenShoot, flagDomain, topUpTransactionDTO.Id.ToString(), resolvedFilePath);
            }

            return CreatedAtAction(nameof(Get), new { id = createdTransaction.Id }, createdTransaction);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    
}
