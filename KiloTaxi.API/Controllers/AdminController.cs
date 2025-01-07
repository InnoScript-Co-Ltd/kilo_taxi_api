using KiloTaxi.API.Services;
using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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


        public AdminController(IAdminRepository adminRepository,DbKiloTaxiContext dbContext,IConfiguration configuration,ApiClientHub apiClientHub)
        {
            _logHelper = LoggerHelper.Instance;
            _adminRepository = adminRepository;
            _dbKiloTaxiContext = dbContext;
            _configuration = configuration;
            _apiClientHub = apiClientHub;
        }

        //GET: api/<AdminController>
        [HttpGet]
        public ActionResult<AdminPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
               
                AdminPagingDTO adminPagingDTO = _adminRepository.GetAllAdmin(pageSortParam);
                if (!adminPagingDTO.Admins.Any())
                {
                    return NoContent();
                }
                // Add a custom header
                //Response.Headers.Add("X-Custom-Header", "foo");
                return Ok(adminPagingDTO);
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
        public  ActionResult<AdminDTO> Post(AdminDTO adminDTO)
        {
            try
            {
                if (adminDTO == null)
                {
                    return BadRequest();
                }
                var existEmailAdmin=_dbKiloTaxiContext.Admins.FirstOrDefault(admin =>
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
    }
}
