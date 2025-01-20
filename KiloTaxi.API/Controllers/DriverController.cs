using System.Net.NetworkInformation;
using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class DriverController : ControllerBase
{
    LoggerHelper _logHelper;
    private readonly IDriverRepository _driverRepository;
    private readonly IConfiguration _configuration;
    private readonly DbKiloTaxiContext _dbKiloTaxiContext;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
    private readonly List<string> _allowedMimeTypes = new List<string>
    {
        "image/jpeg",
        "image/png",
    };
    private const long _maxFileSize = 5 * 1024 * 1024;
    private const string flagDomainDriver = "driver";
    private const string flagDomainVehicle = "vehicle";

    public DriverController(
        IDriverRepository driverRepository,
        IConfiguration configuration,
        DbKiloTaxiContext dbContext,
        IVehicleRepository vehicleRepository
    )
    {
        _logHelper = LoggerHelper.Instance;
        _driverRepository = driverRepository;
        _configuration = configuration;
        _dbKiloTaxiContext = dbContext;
        _vehicleRepository = vehicleRepository;
    }

    [HttpGet]
    public ActionResult<ResponseDTO<DriverPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
    {
        try
        {
            var responseDto = _driverRepository.GetAllDrivers(pageSortParam);
            if (!responseDto.Payload.Drivers.Any())
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

    // [HttpPost]
    // [AllowAnonymous]
    // public async Task<ActionResult<DriverDTO>> Post(DriverDTO driverDTO)
    // {
    //     try
    //     {
    //         if (!ModelState.IsValid)
    //         {
    //             return BadRequest(ModelState);
    //         }
    //         var existEmailDriver=_dbKiloTaxiContext.Drivers.FirstOrDefault(driver =>
    //             driver.Email == driverDTO.Email
    //         );
    //         if (existEmailDriver != null)
    //         {
    //             return Conflict();
    //         }
    //
    //         var fileUploadHelper = new FileUploadHelper(
    //             _configuration,
    //             _allowedExtensions,
    //             _allowedMimeTypes,
    //             _maxFileSize
    //         );
    //         var filesToProcess = new List<(IFormFile? File, string FilePathProperty)>
    //         {
    //             (driverDTO.File_NrcImageFront, nameof(driverDTO.NrcImageFront)),
    //             (driverDTO.File_NrcImageBack, nameof(driverDTO.NrcImageBack)),
    //             (driverDTO.File_DriverImageLicenseFront, nameof(driverDTO.DriverImageLicenseFront)),
    //             (driverDTO.File_DriverImageLicenseBack, nameof(driverDTO.DriverImageLicenseBack)),
    //             (driverDTO.File_Profile, nameof(driverDTO.Profile)),
    //         };
    //
    //         foreach (var (file, filePathProperty) in filesToProcess)
    //         {
    //             if (
    //                 !fileUploadHelper.ValidateFile(
    //                     file,
    //                     true,
    //                     flagDomain,
    //                     out var resolvedFilePath,
    //                     out var errorMessage
    //                 )
    //             )
    //             {
    //                 return BadRequest(errorMessage);
    //             }
    //
    //             var fileName = "_" + filePathProperty + resolvedFilePath;
    //             typeof(DriverDTO).GetProperty(filePathProperty)?.SetValue(driverDTO, fileName);
    //         }
    //         var registerDriver = _driverRepository.DriverRegistration(driverDTO);
    //
    //         foreach (var (file, filePathProperty) in filesToProcess)
    //         {
    //             if (file != null && file.Length > 0)
    //             {
    //                 if (
    //                     !fileUploadHelper.ValidateFile(
    //                         file,
    //                         true,
    //                         flagDomain,
    //                         out var resolvedFilePath,
    //                         out var errorMessage
    //                     )
    //                 )
    //                 {
    //                     return BadRequest(errorMessage);
    //                 }
    //                 await fileUploadHelper.SaveFileAsync(
    //                     file,
    //                     flagDomain,
    //                     driverDTO.Id.ToString() + "_" + filePathProperty,
    //                     resolvedFilePath
    //                 );
    //             }
    //         }
    //         return CreatedAtAction(nameof(Get), new { id = registerDriver.Id }, registerDriver);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logHelper.LogError(ex);
    //         return StatusCode(500, "An error occurred while processing your request.");
    //     }
    // }
    [HttpPost("DriverRegister")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDTO<DriverInfoDTO>>> DriverRegister(
        DriverCreateFormDTO driverCreateFormDto
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existPhoneDriver = _dbKiloTaxiContext.Drivers.FirstOrDefault(driver =>
                driver.Phone == driverCreateFormDto.Phone
            );

            if (existPhoneDriver != null)
            {
                return Conflict(new { Message = "Another user already has this phone number." });
            }

            var fileUploadHelper = new FileUploadHelper(
                _configuration,
                _allowedExtensions,
                _allowedMimeTypes,
                _maxFileSize
            );
            var filesToProcess = new List<(IFormFile? File, string FilePathProperty)>
            {
                (
                    driverCreateFormDto.File_DriverImageLicenseFront,
                    nameof(driverCreateFormDto.DriverImageLicenseFront)
                ),
                (
                    driverCreateFormDto.File_DriverImageLicenseBack,
                    nameof(driverCreateFormDto.DriverImageLicenseBack)
                ),
                (driverCreateFormDto.File_Profile, nameof(driverCreateFormDto.Profile)),
            };

            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (
                    !fileUploadHelper.ValidateFile(
                        file,
                        true,
                        flagDomainDriver,
                        out var resolvedFilePath,
                        out var errorMessage
                    )
                )
                {
                    return BadRequest(errorMessage);
                }

                var fileName = "_" + filePathProperty + resolvedFilePath;
                typeof(DriverCreateFormDTO)
                    .GetProperty(filePathProperty)
                    ?.SetValue(driverCreateFormDto, fileName);
            }
            var registerDriver = _driverRepository.DriverRegistration(driverCreateFormDto);

            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (file != null && file.Length > 0)
                {
                    if (
                        !fileUploadHelper.ValidateFile(
                            file,
                            true,
                            flagDomainDriver,
                            out var resolvedFilePath,
                            out var errorMessage
                        )
                    )
                    {
                        return BadRequest(errorMessage);
                    }
                    await fileUploadHelper.SaveFileAsync(
                        file,
                        flagDomainDriver,
                        registerDriver.Id.ToString() + "_" + filePathProperty,
                        resolvedFilePath
                    );
                }
            }
            var filesToProcessVehicle = new List<(IFormFile? File, string FilePathProperty)>
            {
                (
                    driverCreateFormDto.File_BusinessLicenseImage,
                    nameof(driverCreateFormDto.BusinessLicenseImage)
                ),
                (
                    driverCreateFormDto.File_VehicleLicenseFront,
                    nameof(driverCreateFormDto.VehicleLicenseFront)
                ),
                (
                    driverCreateFormDto.File_VehicleLicenseBack,
                    nameof(driverCreateFormDto.VehicleLicenseBack)
                ),
            };
            foreach (var (file, filePathProperty) in filesToProcessVehicle)
            {
                if (
                    !fileUploadHelper.ValidateFile(
                        file,
                        true,
                        flagDomainVehicle,
                        out var resolvedFilePath,
                        out var errorMessage
                    )
                )
                {
                    return BadRequest(errorMessage);
                }

                var fileName = "_" + filePathProperty + resolvedFilePath;
                typeof(DriverCreateFormDTO)
                    .GetProperty(filePathProperty)
                    ?.SetValue(driverCreateFormDto, fileName);
            }
            driverCreateFormDto.DriverId = registerDriver.Id;
            driverCreateFormDto.VehicleTypeId = 1;
            var registerVehicle = _vehicleRepository.VehicleRegistration(driverCreateFormDto);
            registerDriver.VehicleInfo = new List<VehicleInfoDTO> { registerVehicle };
            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (file != null && file.Length > 0)
                {
                    if (
                        !fileUploadHelper.ValidateFile(
                            file,
                            true,
                            flagDomainVehicle,
                            out var resolvedFilePath,
                            out var errorMessage
                        )
                    )
                    {
                        return BadRequest(errorMessage);
                    }
                    await fileUploadHelper.SaveFileAsync(
                        file,
                        flagDomainVehicle,
                        registerVehicle.Id.ToString() + "_" + filePathProperty,
                        resolvedFilePath
                    );
                }
            }
            ResponseDTO<DriverInfoDTO> response = new ResponseDTO<DriverInfoDTO>();
            response.StatusCode = Ok().StatusCode;
            response.Message = "Driver Register Success.";
            response.Payload = registerDriver;
            response.TimeStamp = DateTime.Now;
            return response;
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            throw new Exception("An error occurred while processing your request.");
            // return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(
        [FromRoute] int id,
        DriverUpdateFormDTO driverUpdateFormDto
    )
    {
        try
        {
            if (id != driverUpdateFormDto.Id)
            {
                return BadRequest();
            }

            driverUpdateFormDto.Password = BCrypt.Net.BCrypt.HashPassword(driverUpdateFormDto.Password);
            var fileUploadHelper = new FileUploadHelper(
                _configuration,
                _allowedExtensions,
                _allowedMimeTypes,
                _maxFileSize
            );
            var filesToProcess = new List<(IFormFile file, string filePathProperty)>
            {
                (driverUpdateFormDto.File_Profile, nameof(driverUpdateFormDto.Profile)),
                // (driverFormDto.File_NrcImageFront, nameof(driverFormDto.NrcImageFront)),
                // (driverFormDto.File_NrcImageBack, nameof(driverFormDto.NrcImageBack)),
                (
                    driverUpdateFormDto.File_DriverImageLicenseFront,
                    nameof(driverUpdateFormDto.DriverImageLicenseFront)
                ),
                (
                    driverUpdateFormDto.File_DriverImageLicenseBack,
                    nameof(driverUpdateFormDto.DriverImageLicenseBack)
                ),
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
                            flagDomainDriver,
                            out var resolvedFilePath,
                            out var errorMessage
                        )
                    )
                    {
                        return BadRequest(errorMessage);
                    }
                    var fileName = "_" + filePathProperty + resolvedFilePath;
                    typeof(DriverUpdateFormDTO)
                        .GetProperty(filePathProperty)
                        ?.SetValue(driverUpdateFormDto, fileName);
                }
            }

            // Update the driver in the repository
            var isUpdated = _driverRepository.UpdateDriver(driverUpdateFormDto);
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
                    var fileName = driverUpdateFormDto.Id.ToString() + "_" + filePathProperty;
                    await fileUploadHelper.SaveFileAsync(
                        file,
                        flagDomainDriver,
                        fileName,
                        fileExtension
                    );
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
                            flagDomainDriver,
                            filePath.Replace(
                                $"{_configuration["MediaHostUrl"]}{flagDomainDriver}/",
                                ""
                            )
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

    [HttpGet("GetAllDrivers")]
    public ActionResult<ResponseDTO<DriverPagingDTO>> GetAllDrivers(
        [FromQuery] PageSortParam pageSortParam
    )
    {
        try
        {
            var responseDto = _driverRepository.GetAllDrivers(pageSortParam);

            if (responseDto?.Payload?.Drivers == null || !responseDto.Payload.Drivers.Any())
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

    [HttpGet("GetDriverById/{id}")]
    public ActionResult<ResponseDTO<DriverInfoDTO>> GetDriverById(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest("Invalid driver ID.");
            }

            var result = _driverRepository.GetDriverById(id);
            if (result == null)
            {
                return NotFound();
            }

            ResponseDTO<DriverInfoDTO> responseDto = new ResponseDTO<DriverInfoDTO>
            {
                StatusCode = Ok().StatusCode,
                Message = "Driver retrieved successfully.",
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

    [HttpPut("UpdateDriver/{id}")]
    public async Task<ActionResult<ResponseDTO<DriverInfoDTO>>> UpdateDriver(
        [FromRoute] int id,
        DriverUpdateFormDTO driverUpdateFormDto
    )
    {
        try
        {
            if (id != driverUpdateFormDto.Id)
            {
                return BadRequest("Driver ID mismatch.");
            }

            // Check if the phone and email are unique
            var existPhoneDriver = _dbKiloTaxiContext.Drivers.FirstOrDefault(driver =>
                driver.Phone == driverUpdateFormDto.Phone
            );

            if (existPhoneDriver != null && existPhoneDriver.Id != driverUpdateFormDto.Id)
            {
                return Conflict(new { Message = "Another driver already has this phone number." });
            }

            var fileUploadHelper = new FileUploadHelper(
                _configuration,
                _allowedExtensions,
                _allowedMimeTypes,
                _maxFileSize
            );
            var filesToProcess = new List<(IFormFile file, string filePathProperty)>
            {
                (driverUpdateFormDto.File_Profile, nameof(driverUpdateFormDto.Profile)),
                (
                    driverUpdateFormDto.File_DriverImageLicenseFront,
                    nameof(driverUpdateFormDto.DriverImageLicenseFront)
                ),
                (
                    driverUpdateFormDto.File_DriverImageLicenseBack,
                    nameof(driverUpdateFormDto.DriverImageLicenseBack)
                ),
            };

            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (file != null && file.Length > 0)
                {
                    if (
                        !fileUploadHelper.ValidateFile(
                            file,
                            true,
                            flagDomainDriver,
                            out var resolvedFilePath,
                            out var errorMessage
                        )
                    )
                    {
                        return BadRequest(errorMessage);
                    }
                    var fileName = "_" + filePathProperty + resolvedFilePath;
                    typeof(DriverUpdateFormDTO)
                        .GetProperty(filePathProperty)
                        ?.SetValue(driverUpdateFormDto, fileName);
                }
            }

            driverUpdateFormDto.Password = BCrypt.Net.BCrypt.HashPassword(
                driverUpdateFormDto.Password
            );

            var isUpdated = _driverRepository.UpdateDriver(driverUpdateFormDto);
            if (!isUpdated)
            {
                return NotFound();
            }

            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (file != null && file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = driverUpdateFormDto.Id.ToString() + "_" + filePathProperty;
                    await fileUploadHelper.SaveFileAsync(
                        file,
                        flagDomainDriver,
                        fileName,
                        fileExtension
                    );
                }
            }

            ResponseDTO<DriverInfoDTO> responseDto = new ResponseDTO<DriverInfoDTO>
            {
                StatusCode = 200, // OK status
                Message = "Driver Info Updated Successfully.",
                Payload =
                    null // No payload since we're just updating the driver
                ,
            };

            return Ok(responseDto);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("DeleteDriver/{id}")]
    public ActionResult<ResponseDTO<DriverInfoDTO>> DeleteDriver([FromRoute] int id)
    {
        try
        {
            var deleteEntity = _driverRepository.GetDriverById(id);
            if (deleteEntity == null)
            {
                return NotFound();
            }

            // List of file paths to delete (modify as necessary)
            var filePaths = new List<string?>
            {
                deleteEntity.Profile,
                deleteEntity.DriverImageLicenseFront,
                deleteEntity.DriverImageLicenseBack,
            };

            foreach (var filePath in filePaths)
            {
                if (!filePath.Contains("default.png") && !string.IsNullOrEmpty(filePath))
                {
                    var resolvedFilePath = Path.Combine(
                            _configuration["MediaFilePath"],
                            flagDomainDriver,
                            filePath.Replace(
                                $"{_configuration["MediaHostUrl"]}{flagDomainDriver}/",
                                ""
                            )
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

            // Return a response with a success message
            ResponseDTO<DriverInfoDTO> responseDto = new ResponseDTO<DriverInfoDTO>
            {
                StatusCode = 204, // OK status
                Message = "Driver Info Deleted Successfully.",
                Payload =
                    null // No payload since we are deleting the driver
                ,
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
