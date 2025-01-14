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
public class PaymentChannelController : ControllerBase
{
    LoggerHelper _logHelper;
    private readonly IPaymentChannelRepository _paymentChannelRepository;

    private readonly IConfiguration _configuration;
    private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
    private readonly List<string> _allowedMimeTypes = new List<string>
    {
        "image/jpeg",
        "image/png",
    };
    private const long _maxFileSize = 5 * 1024 * 1024;
    private const string flagDomain = "payment-channel";

    public PaymentChannelController(
        IPaymentChannelRepository paymentChannelRepository,
        IConfiguration configuration
    )
    {
        _logHelper = LoggerHelper.Instance;
        _paymentChannelRepository = paymentChannelRepository;
        _configuration = configuration;
    }

    // GET: api/<PaymentChannelController>
    [HttpGet]
    public ActionResult<ResponseDTO<PaymentChannelPagingDTO>> Get(
        [FromQuery] PageSortParam pageSortParam
    )
    {
        try
        {
            var responseDto  = _paymentChannelRepository.GetAllPaymentChannels(pageSortParam);
            if (!responseDto.Payload.PaymentChannels.Any())
            {
                return NoContent();
            }
            return responseDto;
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex, "Error occurred while fetching payment channels.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // GET: api/<PaymentChannelController>/5
    [HttpGet("{id}")]
    public ActionResult<ResponseDTO<PaymentChannelInfoDTO>> Get(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest("Invalid Payment Channel ID.");
            }

            var paymentChannel = _paymentChannelRepository.GetPaymentChannelById(id);
            if (paymentChannel == null)
            {
                return NotFound();
            }

            ResponseDTO<PaymentChannelInfoDTO> responseDTO = new ResponseDTO<PaymentChannelInfoDTO>
            {
                StatusCode = Ok().StatusCode,
                Message = "Payment Channel retrieved successfully.",
                Payload = paymentChannel,
            };

            return Ok(responseDTO);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // POST api/<PaymentChannelController>
    [HttpPost]
    public async Task<ActionResult<ResponseDTO<PaymentChannelInfoDTO>>> Post(PaymentChannelFormDTO paymentChannelFormDTO)
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
            var filesToProcess = new List<(IFormFile? File, string FilePathProperty)>
            {
                (paymentChannelFormDTO.File_Icon, nameof(paymentChannelFormDTO.Icon)),
            };

            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (
                    !fileUploadHelper.ValidateFile(
                        file,
                        true,
                        flagDomain,
                        out var resolvedFilePath,
                        out var errorMessage
                    )
                )
                {
                    return BadRequest(errorMessage);
                }

                var fileName = "_" + filePathProperty + resolvedFilePath;
                typeof(PaymentChannelFormDTO)
                    .GetProperty(filePathProperty)
                    ?.SetValue(paymentChannelFormDTO, fileName);
            }

            var createdPaymentChannel = _paymentChannelRepository.CreatePaymentChannel(
                paymentChannelFormDTO
            );

            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (file != null && file.Length > 0)
                {
                    if (
                        !fileUploadHelper.ValidateFile(
                            file,
                            true,
                            flagDomain,
                            out var resolvedFilePath,
                            out var errorMessage
                        )
                    )
                    {
                        return BadRequest(errorMessage);
                    }
                    await fileUploadHelper.SaveFileAsync(
                        file,
                        flagDomain,
                        paymentChannelFormDTO.Id.ToString() + "_" + filePathProperty,
                        resolvedFilePath
                    );
                }
            }
            
            var response = new ResponseDTO<PaymentChannelInfoDTO>
            {
                StatusCode = Ok().StatusCode,
                Message = "Payment Channel Register Success.",
                Payload = createdPaymentChannel,
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

    // PUT api/<PaymentChannelController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseDTO<PaymentChannelInfoDTO>>> Put([FromRoute] int id, PaymentChannelFormDTO paymentChannelFormDTO)
    {
        try
        {
            if (id != paymentChannelFormDTO.Id)
            {
                return BadRequest();
            }

            var fileUploadHelper = new FileUploadHelper(
                _configuration,
                _allowedExtensions,
                _allowedMimeTypes,
                _maxFileSize
            );
            var filesToProcess = new List<(IFormFile file, string filePathProperty)>
            {
                (paymentChannelFormDTO.File_Icon, nameof(paymentChannelFormDTO.Icon)),
            };

            // Validate and update file paths
            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (file != null && file.Length > 0)
                {
                    if (
                        !fileUploadHelper.ValidateFile(
                            file,
                            true,
                            flagDomain,
                            out var resolvedFilePath,
                            out var errorMessage
                        )
                    )
                    {
                        return BadRequest(errorMessage);
                    }
                    var fileName = "_" + filePathProperty + resolvedFilePath;
                    typeof(PaymentChannelFormDTO)
                        .GetProperty(filePathProperty)
                        ?.SetValue(paymentChannelFormDTO, fileName);
                }
            }

            // Update the payment channel in the repository

            var isUpdated = _paymentChannelRepository.UpdatePaymentChannel(paymentChannelFormDTO);
            if (!isUpdated)
            {
                return NotFound();
            }

            // Save the files
            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (file != null && file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = paymentChannelFormDTO.Id.ToString() + "_" + filePathProperty;
                    await fileUploadHelper.SaveFileAsync(file, flagDomain, fileName, fileExtension);
                }
            }

            ResponseDTO<PaymentChannelInfoDTO> responseDto = new ResponseDTO<PaymentChannelInfoDTO>
            {
                StatusCode = 200,
                Message = "Payment Channel updated successfully.",
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

    // DELETE api/<PaymentChannelController>/5
    [HttpDelete("{id}")]
    public ActionResult<ResponseDTO<PaymentChannelInfoDTO>> Delete(int id)
    {
        try
        {
            var paymentChannel = _paymentChannelRepository.GetPaymentChannelById(id);
            if (paymentChannel == null)
            {
                return NotFound();
            }

            var filePaths = new List<string?> { paymentChannel.Icon };
            foreach (var filePath in filePaths)
            {
                if (!filePath.Contains("default.png"))
                {
                    var resolvedFilePath = Path.Combine(
                            _configuration["MediaFilePath"],
                            flagDomain,
                            filePath.Replace($"{_configuration["MediaHostUrl"]}{flagDomain}/", "")
                        )
                        .Replace('\\', '/');
                    if (System.IO.File.Exists(resolvedFilePath))
                    {
                        System.IO.File.Delete(resolvedFilePath);
                    }
                }
            }

            var isDeleted = _paymentChannelRepository.DeletePaymentChannel(id);
            if (!isDeleted)
            {
                return StatusCode(500, "An error occurred while deleting the payment channel.");
            }

            ResponseDTO<PaymentChannelInfoDTO> responseDTO = new ResponseDTO<PaymentChannelInfoDTO>
            {
                StatusCode = 200,
                Message = $"Payment channel deleted successfully.",
                Payload = null,
            };

            return Ok(responseDTO);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
