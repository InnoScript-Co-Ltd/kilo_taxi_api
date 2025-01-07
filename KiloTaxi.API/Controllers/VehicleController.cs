using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;
[Route("api/v1/[controller]")]
[ApiController]
public class VehicleController:ControllerBase
{
    LoggerHelper _logHelper;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IConfiguration _configuration;
    private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
    private readonly List<string> _allowedMimeTypes = new List<string> { "image/jpeg", "image/png" };
    private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB
    private const string flagDomain = "vehicle";

    public VehicleController(IVehicleRepository vehicleRepository, IConfiguration configuration)
    {
        _logHelper = LoggerHelper.Instance;
        _vehicleRepository = vehicleRepository;
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult<IEnumerable<VehiclePagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
    {
        try
        {
            VehiclePagingDTO vehiclePagingDTO = _vehicleRepository.GetAllVehicle(pageSortParam);
            if (!vehiclePagingDTO.Vehicles.Any())
            {
                return NoContent();
            }
            return Ok(vehiclePagingDTO);

        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<VehicleDTO> Get(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var result = _vehicleRepository.GetVehicleById(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500,"An error occurred while processing your request.");
        }
    }
    // [HttpPost]
    // public async Task<ActionResult<VehicleDTO>> Post(VehicleDTO vehicleDTO)
    // {
    //     try
    //     {
    //         if (!ModelState.IsValid)
    //         {
    //             return BadRequest(ModelState);
    //         }
    //
    //         var fileUploadHelper=new FileUploadHelper(_configuration,_allowedExtensions,_allowedMimeTypes,_maxFileSize);
    //         var filesToProcess = new List<(IFormFile? File, string FilePathProperty)>
    //         {
    //             (vehicleDTO.File_BusinessLicenseImage, nameof(vehicleDTO.BusinessLicenseImage)),
    //             (vehicleDTO.File_VehicleLicenseFront, nameof(vehicleDTO.VehicleLicenseFront)),
    //             (vehicleDTO.File_VehicleLicenseBack, nameof(vehicleDTO.VehicleLicenseBack)),
    //         };
    //         foreach (var (file, filePathProperty) in filesToProcess)
    //         {
    //             if (!fileUploadHelper.ValidateFile(file, true, flagDomain, out var resolvedFilePath, out var errorMessage))
    //             {
    //                 return BadRequest(errorMessage);
    //             }
    //
    //             var fileName = "_" + filePathProperty + resolvedFilePath;
    //             typeof(VehicleDTO).GetProperty(filePathProperty)?.SetValue(vehicleDTO,fileName);
    //
    //         }
    //         var registerVehicle=_vehicleRepository.VehicleRegistration(vehicleDTO);
    //         foreach (var (file, filePathProperty) in filesToProcess)
    //         { 
    //             if (file != null && file.Length > 0)
    //             {
    //                 if (!fileUploadHelper.ValidateFile(file, true, flagDomain, out var resolvedFilePath, out var errorMessage))
    //                 {
    //                     return BadRequest(errorMessage);
    //                 }
    //                 await fileUploadHelper.SaveFileAsync(file, flagDomain,vehicleDTO.Id.ToString()+"_"+filePathProperty, resolvedFilePath);
    //
    //             }
    //         }
    //         return CreatedAtAction(nameof(Get), new { id = registerVehicle.Id }, registerVehicle);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logHelper.LogError(ex);
    //         return StatusCode(500,"An error occurred while processing your request.");
    //     }
    // }
        [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromRoute] int id, VehicleUpdateFormDTO vehicleUpdateFormDTO)
    {
        try
        {
            if (id != vehicleUpdateFormDTO.Id)
            {
                return BadRequest();
            }

            var fileUploadHelper = new FileUploadHelper(_configuration, _allowedExtensions, _allowedMimeTypes, _maxFileSize);
            var filesToProcess = new List<(IFormFile file, string filePathProperty)>
            {
                (vehicleUpdateFormDTO.File_BusinessLicenseImage, nameof(vehicleUpdateFormDTO.BusinessLicenseImage)),
                (vehicleUpdateFormDTO.File_VehicleLicenseFront, nameof(vehicleUpdateFormDTO.VehicleLicenseFront)),
                (vehicleUpdateFormDTO.File_VehicleLicenseBack, nameof(vehicleUpdateFormDTO.VehicleLicenseBack))
            };

            // Validate and update file paths
            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (file != null && file.Length > 0)
                {
                    if (!fileUploadHelper.ValidateFile(file, true, flagDomain, out var resolvedFilePath, out var errorMessage))
                    {
                        return BadRequest(errorMessage);
                    }
                    var fileName = "_" + filePathProperty + resolvedFilePath;
                    typeof(VehicleUpdateFormDTO).GetProperty(filePathProperty)?.SetValue(vehicleUpdateFormDTO, fileName);
                }
            }
            var isUpdated = _vehicleRepository.UpdateVehicle(vehicleUpdateFormDTO);
            if (!isUpdated)
            {
                return NotFound();
            }
            foreach (var (file, filePathProperty) in filesToProcess)
            {
                if (file != null && file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName=vehicleUpdateFormDTO.Id.ToString()+"_"+filePathProperty;
                    await fileUploadHelper.SaveFileAsync(file, flagDomain,fileName, fileExtension);
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
            var deleteEntity = _vehicleRepository.GetVehicleById(id);
            if (deleteEntity == null)
            {
                return NotFound();
            }
            var imagePaths = new List<string>
            {
                deleteEntity.BusinessLicenseImage,
                deleteEntity.VehicleLicenseFront,
                deleteEntity.VehicleLicenseBack
                
            };
            foreach (var imagePath in imagePaths)
            {
                var filePath = Path.Combine(_configuration["MediaFilePath"], flagDomain, imagePath.Replace($"{_configuration["MediaHostUrl"]}{flagDomain}/", "")).Replace('\\', '/');

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
           
            var result = _vehicleRepository.DeleteVehicle(deleteEntity.Id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500,"An error occurred while processing your request.");
        }
    }
}