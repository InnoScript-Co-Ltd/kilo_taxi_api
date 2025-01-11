using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _configuration;
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

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
            IConfiguration configuration,
            DbKiloTaxiContext dbContext
        )
        {
            _logHelper = LoggerHelper.Instance;
            _customerRepository = customerRepository;
            _configuration = configuration;
            _dbKiloTaxiContext = dbContext;
        }

        // GET: api/<CustomerController>
        [HttpGet]
        public ActionResult<ResponseDTO<CustomerPagingDTO>> Get(
            [FromQuery] PageSortParam pageSortParam
        )
        {
            try
            {
                var responseDto = _customerRepository.GetAllCustomer(pageSortParam);
                if (!responseDto.Payload.Customers.Any())
                {
                    responseDto.StatusCode = 204;
                    responseDto.Message = "No content available";
                    return responseDto;
                }
                return responseDto;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                var responseDto = new ResponseDTO<CustomerPagingDTO>
                {
                    StatusCode = 500,
                    Message = "An error occurred while processing your request.",
                };
                return StatusCode(500, responseDto);
            }
        }

        // GET: api/<CustomerController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<CustomerDTO>> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest(
                        new ResponseDTO<CustomerDTO> { StatusCode = 400, Message = "Invalid ID." }
                    );
                }

                var result = _customerRepository.GetCustomerById(id);
                if (result == null)
                {
                    return NotFound(
                        new ResponseDTO<CustomerDTO>
                        {
                            StatusCode = 404,
                            Message = "Customer not found.",
                        }
                    );
                }
                return new ResponseDTO<CustomerDTO>
                {
                    StatusCode = 200,
                    Message = "Customer retrieved successfully.",
                    Payload = result,
                };
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(
                    500,
                    new ResponseDTO<CustomerDTO>
                    {
                        StatusCode = 500,
                        Message = "An error occurred while processing your request.",
                    }
                );
            }
        }

        // POST api/<CustomerController>
        [HttpPost]
        public async Task<ActionResult<ResponseDTO<CustomerInfoDTO>>> Post(
            CustomerFormDTO customerDTO
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new ResponseDTO<CustomerInfoDTO>
                        {
                            StatusCode = 400,
                            Message = "Invalid input data.",
                            Payload = null,
                        }
                    );
                }
                var existEmailCustomer = _dbKiloTaxiContext.Customers.FirstOrDefault(customer =>
                    customer.Email == customerDTO.Email
                );
                if (existEmailCustomer != null)
                {
                    return Conflict(
                        new ResponseDTO<CustomerInfoDTO>
                        {
                            StatusCode = 409,
                            Message = "Email already exists.",
                        }
                    );
                }
                var fileUploadHelper = new FileUploadHelper(
                    _configuration,
                    _allowedExtensions,
                    _allowedMimeTypes,
                    _maxFileSize
                );
                var filesToProcess = new List<(IFormFile? File, string FilePathProperty)>
                {
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
                        return BadRequest(
                            new ResponseDTO<CustomerInfoDTO>
                            {
                                StatusCode = 400,
                                Message = errorMessage,
                            }
                        );
                    }

                    var fileName = "_" + filePathProperty + resolvedFilePath;
                    typeof(CustomerFormDTO)
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
                            return BadRequest(
                                new ResponseDTO<CustomerInfoDTO>
                                {
                                    StatusCode = 400,
                                    Message = errorMessage,
                                }
                            );
                        }
                        await fileUploadHelper.SaveFileAsync(
                            file,
                            flagDomain,
                            customerDTO.Id.ToString() + "_" + filePathProperty,
                            resolvedFilePath
                        );
                    }
                }

                return new ResponseDTO<CustomerInfoDTO>
                {
                    StatusCode = 201,
                    Message = "Customer created successfully",
                    Payload = createCustomer,
                };
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(
                    500,
                    new ResponseDTO<CustomerInfoDTO>
                    {
                        StatusCode = 500,
                        Message = "An error occurred while processing your request.",
                    }
                );
            }
        }

        [HttpPost("CustomerRegister")]
        public async Task<ActionResult<ResponseDTO<OtpInfo>>> CustomerRegister(
            [FromBody] CustomerFormDTO customerFormDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ResponseDTO<OtpInfo> { StatusCode = 400, Message = "Invalid input data." }
                );
            }
            ResponseDTO<OtpInfo> response = await _customerRepository.FindCustomerAndGenerateOtp(
                customerFormDto
            );
            var unVerifiedUser = JsonConvert.SerializeObject(response);
            HttpContext.Session.SetString("UnVerifiedUser" + customerFormDto.Phone, unVerifiedUser);
            response.Payload = null;
            return response;
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDTO<CustomerInfoDTO>>> Put(
            [FromRoute] int id,
            CustomerFormDTO customerDTO
        )
        {
            try
            {
                if (id != customerDTO.Id)
                {
                    return BadRequest(
                        new ResponseDTO<CustomerInfoDTO>
                        {
                            StatusCode = 400,
                            Message = "ID mismatch.",
                        }
                    );
                }
                var existPhoneCustomer = _dbKiloTaxiContext.Customers.FirstOrDefault(customer =>
                    customer.Phone == customerDTO.Phone
                );

                var existEmailCustomer = _dbKiloTaxiContext.Customers.FirstOrDefault(customer =>
                    customer.Email == customerDTO.Email
                );
                if (existPhoneCustomer != null && existPhoneCustomer.Id != customerDTO.Id)
                {
                    return Conflict(
                        new ResponseDTO<CustomerInfoDTO>
                        {
                            StatusCode = 409,
                            Message = "Another user already has this phone number.",
                        }
                    );
                }

                if (existEmailCustomer != null && existEmailCustomer.Id != customerDTO.Id)
                {
                    return Conflict(
                        new ResponseDTO<CustomerInfoDTO>
                        {
                            StatusCode = 409,
                            Message = "Another user already has this email address.",
                        }
                    );
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
                            return BadRequest(
                                new ResponseDTO<CustomerInfoDTO>
                                {
                                    StatusCode = 400,
                                    Message = errorMessage,
                                }
                            );
                        }
                        var fileName = "_" + filePathProperty + resolvedFilePath;
                        typeof(CustomerFormDTO)
                            .GetProperty(filePathProperty)
                            ?.SetValue(customerDTO, fileName);
                    }
                }
                customerDTO.Password = BCrypt.Net.BCrypt.HashPassword(customerDTO.Password);
                // Update the customer in the repository
                var isUpdated = _customerRepository.UpdateCustomer(customerDTO);
                if (!isUpdated)
                {
                    return NotFound(
                        new ResponseDTO<CustomerInfoDTO>
                        {
                            StatusCode = 404,
                            Message = "Customer not found.",
                        }
                    );
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

                return new ResponseDTO<CustomerInfoDTO>
                {
                    StatusCode = 200,
                    Message = "Customer Info Updated Successfully.",
                    TimeStamp = DateTime.Now,
                };
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(
                    500,
                    new ResponseDTO<CustomerInfoDTO>
                    {
                        StatusCode = 500,
                        Message = "An error occurred while processing your request.",
                    }
                );
            }
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<object>> Delete([FromRoute] int id)
        {
            try
            {
                var deleteEntity = _customerRepository.GetCustomerById(id);
                if (deleteEntity == null)
                {
                    return NotFound(
                        new ResponseDTO<object>
                        {
                            StatusCode = 404,
                            Message = "Customer not found.",
                        }
                    );
                }
                var filePaths = new List<string?> { deleteEntity.Profile };
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
                    return NotFound(
                        new ResponseDTO<object>
                        {
                            StatusCode = 404,
                            Message = "Customer deletion failed.",
                        }
                    );
                }
                return new ResponseDTO<object>
                {
                    StatusCode = 204,
                    Message = "Customer deleted successfully.",
                };
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(
                    500,
                    new ResponseDTO<object>
                    {
                        StatusCode = 500,
                        Message = "An error occurred while processing your request.",
                    }
                );
            }
        }
    }
}
