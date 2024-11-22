using System.Net.NetworkInformation;
using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class DriverController : ControllerBase
{
    LoggerHelper _logHelper;
    private readonly IDriverRepository _driverRepository;
    private readonly IConfiguration _configuration;
    private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
    private readonly List<string> _allowedMimeTypes = new List<string>
    {
        "image/jpeg",
        "image/png",
    };
    private const long _maxFileSize = 5 * 1024 * 1024;
    private const string flagDomain = "driver";

    public DriverController(IDriverRepository driverRepository, IConfiguration configuration)
    {
        _logHelper = LoggerHelper.Instance;
        _driverRepository = driverRepository;
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult<IEnumerable<DriverPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
    {
        try
        {
            DriverPagingDTO driverPagingDto = _driverRepository.GetAllDrivers(pageSortParam);
            if (!driverPagingDto.Drivers.Any())
            {
                return NoContent();
            }
            return Ok(driverPagingDto);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<DriverDTO> Get(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var result = _driverRepository.GetDriverById(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<DriverDTO>> Post(DriverDTO driverDTO)
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
                (driverDTO.File_NrcImageFront, nameof(driverDTO.NrcImageFront)),
                (driverDTO.File_NrcImageBack, nameof(driverDTO.NrcImageBack)),
                (driverDTO.File_DriverImageLicenseFront, nameof(driverDTO.DriverImageLicenseFront)),
                (driverDTO.File_DriverImageLicenseBack, nameof(driverDTO.DriverImageLicenseBack)),
                (driverDTO.File_Profile, nameof(driverDTO.Profile)),
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
                typeof(DriverDTO).GetProperty(filePathProperty)?.SetValue(driverDTO, fileName);
            }
            var registerDriver = _driverRepository.DriverRegistration(driverDTO);

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
                        driverDTO.Id.ToString() + "_" + filePathProperty,
                        resolvedFilePath
                    );
                }
            }
            return CreatedAtAction(nameof(Get), new { id = registerDriver.Id }, registerDriver);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromRoute] int id, DriverDTO driverDTO)
    {
        try
        {
            if (id != driverDTO.Id)
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
                (driverDTO.File_Profile, nameof(driverDTO.Profile)),
                (driverDTO.File_NrcImageFront, nameof(driverDTO.NrcImageFront)),
                (driverDTO.File_NrcImageBack, nameof(driverDTO.NrcImageBack)),
                (driverDTO.File_DriverImageLicenseFront, nameof(driverDTO.DriverImageLicenseFront)),
                (driverDTO.File_DriverImageLicenseBack, nameof(driverDTO.DriverImageLicenseBack)),
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
                    typeof(DriverDTO).GetProperty(filePathProperty)?.SetValue(driverDTO, fileName);
                }
            }

            // Update the driver in the repository
            var isUpdated = _driverRepository.UpdateDriver(driverDTO);
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
                    var fileName = driverDTO.Id.ToString() + "_" + filePathProperty;
                    await fileUploadHelper.SaveFileAsync(file, flagDomain, fileName, fileExtension);
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete([FromRoute] int id)
    {
        try
        {
            var deleteEntity = _driverRepository.GetDriverById(id);
            if (deleteEntity == null)
            {
                return NotFound();
            }
            var filePaths = new List<string?>
            {
                deleteEntity.NrcImageFront,
                deleteEntity.NrcImageBack,
                deleteEntity.DriverImageLicenseFront,
                deleteEntity.DriverImageLicenseBack,
                deleteEntity.Profile,
            };
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
            var result = _driverRepository.DeleteDriver(deleteEntity.Id);
            if (!result)
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
