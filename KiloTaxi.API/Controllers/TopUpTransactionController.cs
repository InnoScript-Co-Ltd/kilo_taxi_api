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
    private readonly List<string> _allowedMimeTypes = new List<string>
    {
        "image/jpeg",
        "image/png",
    };
    private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB
    private const string flagDomain = "screenShoot";

    public TopUpTransactionController(
        ITopUpTransactionRepository topUpTransactionRepository,
        IConfiguration configuration
    )
    {
        _logHelper = LoggerHelper.Instance;
        _topUpTransactionRepository = topUpTransactionRepository;
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult<ResponseDTO<TopUpTransactionPagingDTO>> Get(
        [FromQuery] PageSortParam pageSortParam
    )
    {
        try
        {
            var responseDto = _topUpTransactionRepository.GetAllTopUpTransactions(pageSortParam);
            if (!responseDto.Payload.TopUpTransactions.Any())
            {
                return NoContent();
            }
            return responseDto;
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TopUpTransactionInfoDTO>> Post(
        TopUpTransactionFormDTO topUpTransactionFormDTO
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var fileUploadHelper = new FileUploadHelper(
                _configuration,
                _allowedExtensions,
                _allowedMimeTypes,
                _maxFileSize
            );
            if (
                !fileUploadHelper.ValidateFile(
                    topUpTransactionFormDTO.File_TransactionScreenShoot,
                    true,
                    flagDomain,
                    out var resolvedFilePath,
                    out var errorMessage
                )
            )
            {
                return BadRequest(errorMessage);
            }
            topUpTransactionFormDTO.TransactionScreenShoot = resolvedFilePath;

            var createdTransaction = _topUpTransactionRepository.CreateTopUpTransaction(
                topUpTransactionFormDTO
            );

            if (
                topUpTransactionFormDTO.File_TransactionScreenShoot != null
                && topUpTransactionFormDTO.File_TransactionScreenShoot.Length > 0
            )
            {
                string filePath = await fileUploadHelper.SaveFileAsync(
                    topUpTransactionFormDTO.File_TransactionScreenShoot,
                    flagDomain,
                    topUpTransactionFormDTO.Id.ToString(),
                    resolvedFilePath
                );
            }
            return CreatedAtAction(
                nameof(Get),
                new { id = createdTransaction.Id },
                createdTransaction
            );
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("GetAllTopUpTransactions")]
    public ActionResult<ResponseDTO<TopUpTransactionPagingDTO>> GetAllTopUpTransactions(
        [FromQuery] PageSortParam pageSortParam
    )
    {
        try
        {
            // Get the paginated list of top-up transactions
            var responseDto = _topUpTransactionRepository.GetAllTopUpTransactions(pageSortParam);

            // Check if the payload is null or empty and return NoContent if so
            if (
                responseDto?.Payload?.TopUpTransactions == null
                || !responseDto.Payload.TopUpTransactions.Any()
            )
            {
                return NoContent();
            }

            // Return the response DTO with status OK
            return Ok(responseDto);
        }
        catch (Exception ex)
        {
            // Log the exception if an error occurs
            _logHelper.LogError(ex);

            // Return an internal server error with the exception message
            return StatusCode(
                500,
                new
                {
                    Message = "An error occurred while processing your request.",
                    Details = ex.Message,
                }
            );
        }
    }

    [HttpPost("CreateTopUpTransaction")]
    public async Task<ActionResult<ResponseDTO<TopUpTransactionInfoDTO>>> CreateTopUpTransaction(
        TopUpTransactionFormDTO topUpTransactionFormDTO
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fileUploadHelper = new FileUploadHelper(
                _configuration,
                _allowedExtensions,
                _allowedMimeTypes,
                _maxFileSize
            );

            string resolvedFilePath = null; // Declare resolvedFilePath outside the file validation block

            if (
                topUpTransactionFormDTO.File_TransactionScreenShoot != null
                && topUpTransactionFormDTO.File_TransactionScreenShoot.Length > 0
            )
            {
                if (
                    !fileUploadHelper.ValidateFile(
                        topUpTransactionFormDTO.File_TransactionScreenShoot,
                        true,
                        flagDomain,
                        out resolvedFilePath, // Output the resolved file path
                        out var errorMessage
                    )
                )
                {
                    return BadRequest(errorMessage);
                }

                topUpTransactionFormDTO.TransactionScreenShoot = resolvedFilePath;
            }

            var createdTransaction = _topUpTransactionRepository.CreateTopUpTransaction(
                topUpTransactionFormDTO
            );

            if (
                topUpTransactionFormDTO.File_TransactionScreenShoot != null
                && topUpTransactionFormDTO.File_TransactionScreenShoot.Length > 0
            )
            {
                string filePath = await fileUploadHelper.SaveFileAsync(
                    topUpTransactionFormDTO.File_TransactionScreenShoot,
                    flagDomain,
                    topUpTransactionFormDTO.Id.ToString(),
                    resolvedFilePath // Use the resolved file path
                );
            }

            var response = new ResponseDTO<TopUpTransactionInfoDTO>
            {
                StatusCode = (
                    CreatedAtAction(
                        nameof(Get),
                        new { id = createdTransaction.Id },
                        createdTransaction
                    ).StatusCode ?? 200
                ),
                Message = "Top-Up Transaction Created Successfully.",
                Payload = createdTransaction,
                TimeStamp = DateTime.Now,
            };

            return response;
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(
                500,
                new
                {
                    Message = "An error occurred while processing your request.",
                    Details = ex.Message,
                }
            );
        }
    }
}
