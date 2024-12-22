using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
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
    public ActionResult<IEnumerable<PaymentChannelPagingDTO>> Get(
        [FromQuery] PageSortParam pageSortParam
    )
    {
        try
        {
            PaymentChannelPagingDTO paymentChannelPagingDTO =
                _paymentChannelRepository.GetAllPaymentChannels(pageSortParam);
            if (!paymentChannelPagingDTO.PaymentChannels.Any())
            {
                return NoContent();
            }
            return Ok(paymentChannelPagingDTO);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex, "Error occurred while fetching payment channels.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // GET: api/<PaymentChannelController>/5
    [HttpGet("{id}")]
    public ActionResult<PaymentChannelDTO> Get(int id)
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

            return Ok(paymentChannel);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // POST api/<PaymentChannelController>
    [HttpPost]
    public async Task<ActionResult<PaymentChannelDTO>> Post(PaymentChannelDTO paymentChannelDTO)
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
                (paymentChannelDTO.File_Icon, nameof(paymentChannelDTO.Icon)),
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
                typeof(PaymentChannelDTO)
                    .GetProperty(filePathProperty)
                    ?.SetValue(paymentChannelDTO, fileName);
            }

            var createdPaymentChannel = _paymentChannelRepository.CreatePaymentChannel(
                paymentChannelDTO
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
                        paymentChannelDTO.Id.ToString() + "_" + filePathProperty,
                        resolvedFilePath
                    );
                }
            }

            return CreatedAtAction(
                nameof(Get),
                new { id = createdPaymentChannel.Id },
                createdPaymentChannel
            );
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // PUT api/<PaymentChannelController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromRoute] int id, PaymentChannelDTO paymentChannelDTO)
    {
        try
        {
            if (id != paymentChannelDTO.Id)
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
                (paymentChannelDTO.File_Icon, nameof(paymentChannelDTO.Icon)),
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
                    typeof(PaymentChannelDTO)
                        .GetProperty(filePathProperty)
                        ?.SetValue(paymentChannelDTO, fileName);
                }
            }

            // Update the payment channel in the repository

            var isUpdated = _paymentChannelRepository.UpdatePaymentChannel(paymentChannelDTO);
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
                    var fileName = paymentChannelDTO.Id.ToString() + "_" + filePathProperty;
                    await fileUploadHelper.SaveFileAsync(file, flagDomain, fileName, fileExtension);
                }
            }

            return Ok("Payment Channel updated successfully.");
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // DELETE api/<PaymentChannelController>/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
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

            return Ok($"Payment channel with ID {id} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
