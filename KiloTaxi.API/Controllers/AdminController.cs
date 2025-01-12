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
        public ActionResult<ResponseDTO<AdminPagingDTO>> Get(
            [FromQuery] PageSortParam pageSortParam
        )
        {
            try
            {
                var responseDto = _adminRepository.GetAllAdmin(pageSortParam);
                if (!responseDto.Payload.Admins.Any())
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
        public ActionResult<AdminDTO> Get(int id)
        {
            try
            {
                _logHelper.LogInfo("test info log");
                if (id == 0)
                {
                    return BadRequest();
                }
                var result = _adminRepository.GetAdminById(id);
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

        // POST api/<AdminController>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<AdminDTO> Post(AdminDTO adminDTO)
        {
            try
            {
                if (adminDTO == null)
                {
                    return BadRequest();
                }
                var existEmailAdmin = _dbKiloTaxiContext.Admins.FirstOrDefault(admin =>
                    admin.Email == adminDTO.Email
                );
                if (existEmailAdmin != null)
                {
                    return Conflict();
                }

                var createdAdmin = _adminRepository.AddAdmin(adminDTO);
                return CreatedAtAction(nameof(Get), new { id = createdAdmin.Id }, createdAdmin);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] AdminDTO adminDTO)
        {
            try
            {
                if (adminDTO == null || id != adminDTO.Id)
                {
                    return BadRequest();
                }

                var result = _adminRepository.UpdateAdmin(adminDTO);

                if (!result)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE api/<AdminController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var admin = _adminRepository.GetAdminById(id);
                if (admin == null)
                {
                    return NotFound();
                }

                var result = _adminRepository.DeleteAdmin(id);
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

        [HttpGet("GetAllAdmin")]
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

        [HttpGet("GetAdminById/{id}")]
        public ActionResult<ResponseDTO<AdminDTO>> GetAdminById(int id)
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
                ResponseDTO<AdminDTO> responseDto = new ResponseDTO<AdminDTO>
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

        [HttpPut("UpdateAdmin/{id}")]
        public async Task<ActionResult<ResponseDTO<AdminDTO>>> UpdateAdmin(
            [FromRoute] int id,
            AdminDTO adminDTO
        )
        {
            try
            {
                if (id != adminDTO.Id)
                {
                    return BadRequest("Admin ID mismatch.");
                }

                var isUpdated = _adminRepository.UpdateAdmin(adminDTO);
                if (!isUpdated)
                {
                    return NotFound();
                }

                ResponseDTO<AdminDTO> responseDto = new ResponseDTO<AdminDTO>
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

        [HttpDelete("DeleteAdmin/{id}")]
        public ActionResult<ResponseDTO<AdminDTO>> DeleteAdmin([FromRoute] int id)
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
                ResponseDTO<AdminDTO> responseDto = new ResponseDTO<AdminDTO>
                {
                    StatusCode = 200, // OK status
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
