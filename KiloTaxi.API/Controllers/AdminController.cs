using KiloTaxi.API.Services;
using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IAdminRepository _adminRepository;
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;
        private readonly IConfiguration _configuration;
        private ApiClientHub _apiClientHub;

        public AdminController(
            IAdminRepository adminRepository,
            DbKiloTaxiContext dbContext,
            IConfiguration configuration,
            ApiClientHub apiClientHub
        )
        {
            _logHelper = LoggerHelper.Instance;
            _adminRepository = adminRepository;
            _dbKiloTaxiContext = dbContext;
            _configuration = configuration;
            _apiClientHub = apiClientHub;
        }

        //GET: api/<AdminController>
        [HttpGet]
        public ActionResult<ResponseDTO<AdminPagingDTO>> GetAllAdmin(
            [FromQuery] PageSortParam pageSortParam
        )
        {
            try
            {
                var responseDto = _adminRepository.GetAllAdmin(pageSortParam);

                if (responseDto?.Payload?.Admins == null || !responseDto.Payload.Admins.Any())
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
        public async Task<ActionResult<ResponseDTO<AdminInfoDTO>>> AdminRegister(
            AdminFormDTO adminFormDTO
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check for duplicate phone
                var existPhoneAdmin = _dbKiloTaxiContext.Admins.FirstOrDefault(admin =>
                    admin.Phone == adminFormDTO.Phone
                );
                if (existPhoneAdmin != null)
                {
                    return Conflict(
                        new { Message = "Another admin already has this phone number." }
                    );
                }

                // Check for duplicate email
                var existEmailAdmin = _dbKiloTaxiContext.Admins.FirstOrDefault(admin =>
                    admin.Email == adminFormDTO.Email
                );
                if (existEmailAdmin != null)
                {
                    return Conflict(
                        new { Message = "Another admin already has this email address." }
                    );
                }

                // Register the admin
                var registerAdmin = _adminRepository.AdminRegistration(adminFormDTO);

                // Prepare response
                var response = new ResponseDTO<AdminInfoDTO>
                {
                    StatusCode = 201,
                    Message = "Admin Register Success.",
                    Payload = registerAdmin,
                    TimeStamp = DateTime.Now,
                };

                return response;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                throw new Exception("An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<AdminInfoDTO>> GetAdminById(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("Invalid admin ID.");
                }

                // Fetch the admin from the repository
                var result = _adminRepository.GetAdminById(id);
                if (result == null)
                {
                    return NotFound();
                }

                // Create the response DTO and return the result
                ResponseDTO<AdminInfoDTO> responseDto = new ResponseDTO<AdminInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "Admin retrieved successfully.",
                    Payload =
                        result // Assuming 'result' is an AdminDTO
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

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDTO<AdminInfoDTO>>> UpdateAdmin(
            [FromRoute] int id,
            AdminFormDTO adminFormDTO
        )
        {
            try
            {
                if (id != adminFormDTO.Id)
                {
                    return BadRequest("Admin ID mismatch.");
                }

                var isUpdated = _adminRepository.UpdateAdmin(adminFormDTO);
                if (!isUpdated)
                {
                    return NotFound();
                }

                ResponseDTO<AdminInfoDTO> responseDto = new ResponseDTO<AdminInfoDTO>
                {
                    StatusCode = 200, // OK status
                    Message = "Admin Info Updated Successfully.",
                    Payload =
                        null // No payload since we just updated the admin
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

        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<AdminInfoDTO>> DeleteAdmin([FromRoute] int id)
        {
            try
            {
                var deleteEntity = _adminRepository.GetAdminById(id);
                if (deleteEntity == null)
                {
                    return NotFound();
                }

                var result = _adminRepository.DeleteAdmin(deleteEntity.Id);
                if (!result)
                {
                    return NotFound();
                }

                // Return a response with a success message
                ResponseDTO<AdminInfoDTO> responseDto = new ResponseDTO<AdminInfoDTO>
                {
                    StatusCode = 204, // OK status
                    Message = "Admin Info Deleted Successfully.",
                    Payload =
                        null // No payload since we are deleting the admin
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
}
