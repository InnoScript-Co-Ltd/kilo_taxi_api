using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _configuration;
        private readonly List<string> _allowedExtensions = new List<string>
        {
            ".jpg",
            ".jpeg",
            ".png",
        };
        private readonly List<string> _allowedMimeTypes = new List<string>
        {
            "image/jpeg",
            "image/png",
        };
        private const long _maxFileSize = 5 * 1024 * 1024;
        private const string flagDomain = "customer";

        public CustomerController(
            ICustomerRepository customerRepository,
            IConfiguration configuration
        )
        {
            _logHelper = LoggerHelper.Instance;
            _customerRepository = customerRepository;
            _configuration = configuration;
        }

        // GET: api/<CustomerController>
        [HttpGet]
        public ActionResult<IEnumerable<CustomerPagingDTO>> Get(
            [FromQuery] PageSortParam pageSortParam
        )
        {
            try
            {
                CustomerPagingDTO customerPagingDTO = _customerRepository.GetAllCustomer(
                    pageSortParam
                );
                if (!customerPagingDTO.Customers.Any())
                {
                    return NoContent();
                }
                return Ok(customerPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<CustomerController>/5
        [HttpGet("{id}")]
        public ActionResult<CustomerDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _customerRepository.GetCustomerById(id);
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

        // POST api/<CustomerController>
        [HttpPost]
        public async Task<ActionResult<CustomerDTO>> Post(CustomerDTO customerDTO)
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
                    (customerDTO.File_NrcImageFront, nameof(customerDTO.NrcImageFront)),
                    (customerDTO.File_NrcImageBack, nameof(customerDTO.NrcImageBack)),
                    (customerDTO.File_Profile, nameof(customerDTO.Profile)),
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
                    typeof(CustomerDTO)
                        .GetProperty(filePathProperty)
                        ?.SetValue(customerDTO, fileName);
                }
                var createCustomer = _customerRepository.AddCustomer(customerDTO);

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
                            customerDTO.Id.ToString() + "_" + filePathProperty,
                            resolvedFilePath
                        );
                    }
                }
                return CreatedAtAction(nameof(Get), new { id = createCustomer.Id }, createCustomer);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, CustomerDTO customerDTO)
        {
            try
            {
                if (id != customerDTO.Id)
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
                    (customerDTO.File_Profile, nameof(customerDTO.Profile)),
                    (customerDTO.File_NrcImageFront, nameof(customerDTO.NrcImageFront)),
                    (customerDTO.File_NrcImageBack, nameof(customerDTO.NrcImageBack)),
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
                        typeof(CustomerDTO)
                            .GetProperty(filePathProperty)
                            ?.SetValue(customerDTO, fileName);
                    }
                }

                // Update the customer in the repository
                var isUpdated = _customerRepository.UpdateCustomer(customerDTO);
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
                        var fileName = customerDTO.Id.ToString() + "_" + filePathProperty;
                        await fileUploadHelper.SaveFileAsync(
                            file,
                            flagDomain,
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

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            try
            {
                var deleteEntity = _customerRepository.GetCustomerById(id);
                if (deleteEntity == null)
                {
                    return NotFound();
                }
                var filePaths = new List<string?>
                {
                    deleteEntity.NrcImageFront,
                    deleteEntity.NrcImageBack,
                    deleteEntity.Profile,
                };
                foreach (var filePath in filePaths)
                {
                    if (!filePath.Contains("default.png"))
                    {
                        var resolvedFilePath = Path.Combine(
                                _configuration["MediaFilePath"],
                                flagDomain,
                                filePath.Replace(
                                    $"{_configuration["MediaHostUrl"]}{flagDomain}/",
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
                var result = _customerRepository.DeleteCustomer(deleteEntity.Id);
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
}
