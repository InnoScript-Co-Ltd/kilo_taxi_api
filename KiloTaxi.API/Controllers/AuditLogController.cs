using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogController(IAuditLogRepository auditLogRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _auditLogRepository = auditLogRepository;
        }

        // GET: api/<AuditLogController>
        [HttpGet]
        public ActionResult<AuditLogPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                AuditLogPagingDTO auditLogPagingDTO =
                    _auditLogRepository.GetAllAuditLog(pageSortParam);
                if (!auditLogPagingDTO.AuditLogs.Any())
                {
                    return NoContent();
                }
                return Ok(auditLogPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<AuditLogController>/1
        [HttpGet("{id}")]
        public ActionResult<AuditLogDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _auditLogRepository.GetAuditLogByID(id);
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

        // POST api/<AuditLogController>
        [HttpPost]
        public ActionResult<AuditLogDTO> Post([FromBody] AuditLogDTO auditLogDTO)
        {
            try
            {
                if (auditLogDTO == null)
                {
                    return BadRequest();
                }

                var createdAuditLog = _auditLogRepository.CreateAuditLog(
                    auditLogDTO
                );
                return CreatedAtAction(
                    nameof(Get),
                    new { id = createdAuditLog.Id },
                    createdAuditLog
                );
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
