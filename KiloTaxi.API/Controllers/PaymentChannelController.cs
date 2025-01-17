using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Authorization;
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
    private const string flagDomainPaymentChannel = "payment-channel";

    public PaymentChannelController(
        IPaymentChannelRepository paymentChannelRepository,
        IConfiguration configuration
    )
    {
        _logHelper = LoggerHelper.Instance;
        _paymentChannelRepository = paymentChannelRepository;
        _configuration = configuration;
    }
    [HttpGet]
    public ActionResult<ResponseDTO<PaymentChannelPagingDTO>> GetAllPaymentChannels(
        [FromQuery] PageSortParam pageSortParam
    )
    {
        try
        {
            var responseDto = _paymentChannelRepository.GetAllPaymentChannels(pageSortParam);

            if (
                responseDto?.Payload?.PaymentChannels == null
                || !responseDto.Payload.PaymentChannels.Any()
            )
            {
                return NoContent();
            }

            return Ok(responseDto);
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

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDTO<PaymentChannelInfoDTO>>> PaymentChannelCreate(
        PaymentChannelFormDTO paymentChannelFormDTO
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

            // List of files to process and validate
            var filesToProcess = new List<(IFormFile? File, string FilePathProperty)>
            {
                (paymentChannelFormDTO.File_Icon, nameof(paymentChannelFormDTO.Icon)),
            };

            // Validate and assign file paths
            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (
                    !fileUploadHelper.ValidateFile(
                        file,
                        true,
                        flagDomainPaymentChannel,
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

            // Register the payment channel
            var registerPaymentChannel = _paymentChannelRepository.CreatePaymentChannel(
                paymentChannelFormDTO
            );

            // Save the files to storage
            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (file != null && file.Length > 0)
                {
                    if (
                        !fileUploadHelper.ValidateFile(
                            file,
                            true,
                            flagDomainPaymentChannel,
                            out var resolvedFilePath,
                            out var errorMessage
                        )
                    )
                    {
                        return BadRequest(errorMessage);
                    }
                    await fileUploadHelper.SaveFileAsync(
                        file,
                        flagDomainPaymentChannel,
                        registerPaymentChannel.Id.ToString() + "_" + filePathProperty,
                        resolvedFilePath
                    );
                }
            }

            ResponseDTO<PaymentChannelInfoDTO> response = new ResponseDTO<PaymentChannelInfoDTO>
            {
                StatusCode = 201,
                Message = "Payment Channel Created Successfully.",
                Payload = registerPaymentChannel,
                TimeStamp = DateTime.Now,
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<ResponseDTO<PaymentChannelInfoDTO>> GetPaymentChannelById(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest("Invalid payment channel ID.");
            }

            var result = _paymentChannelRepository.GetPaymentChannelById(id);
            if (result == null)
            {
                return NotFound();
            }

            ResponseDTO<PaymentChannelInfoDTO> responseDto = new ResponseDTO<PaymentChannelInfoDTO>
            {
                StatusCode = Ok().StatusCode,
                Message = "Payment channel retrieved successfully.",
                Payload = result,
            };

            return Ok(responseDto);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    public ActionResult<ResponseDTO<PaymentChannelInfoDTO>> UpdatePaymentChannel(
        [FromRoute] int id,
        PaymentChannelFormDTO paymentChannelFormDTO
    )
    {
        try
        {
            if (id != paymentChannelFormDTO.Id)
            {
                return BadRequest("Payment channel ID mismatch.");
            }

            var isUpdated = _paymentChannelRepository.UpdatePaymentChannel(paymentChannelFormDTO);
            if (!isUpdated)
            {
                return NotFound();
            }

            ResponseDTO<PaymentChannelInfoDTO> responseDto = new ResponseDTO<PaymentChannelInfoDTO>
            {
                StatusCode = 200, // OK status
                Message = "Payment channel updated successfully.",
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
    public ActionResult<ResponseDTO<PaymentChannelInfoDTO>> DeletePaymentChannel([FromRoute] int id)
    {
        try
        {
            var deleteEntity = _paymentChannelRepository.GetPaymentChannelById(id);
            if (deleteEntity == null)
            {
                return NotFound();
            }

            var result = _paymentChannelRepository.DeletePaymentChannel(deleteEntity.Id);
            if (!result)
            {
                return NotFound();
            }
            ResponseDTO<PaymentChannelInfoDTO> responseDto = new ResponseDTO<PaymentChannelInfoDTO>();
            responseDto.StatusCode = 204;
            responseDto.Message = "Payment channel deleted successfully.";
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
