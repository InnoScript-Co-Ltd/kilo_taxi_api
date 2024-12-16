using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TransactionLogController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly ITransactionLogRepository _transactionLogRepository;

        public TransactionLogController(ITransactionLogRepository transactionLogRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _transactionLogRepository = transactionLogRepository;
        }

        // GET: api/<TransactionLogController>
        [HttpGet]
        public ActionResult<TransactionLogPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                TransactionLogPagingDTO transactionLogPagingDTO =
                    _transactionLogRepository.GetAllTransactionLog(pageSortParam);
                if (!transactionLogPagingDTO.TransactionLogs.Any())
                {
                    return NoContent();
                }
                return Ok(transactionLogPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<TransactionLogController>/5
        [HttpGet("{id}")]
        public ActionResult<TransactionLogDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _transactionLogRepository.GetTransactionLogByID(id);
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

        // POST api/<TransactionLogController>
        [HttpPost]
        public ActionResult<TransactionLogDTO> Post([FromBody] TransactionLogDTO transactionLogDTO)
        {
            try
            {
                if (transactionLogDTO == null)
                {
                    return BadRequest();
                }

                var createdTransactionLog = _transactionLogRepository.CreateTransactionLog(
                    transactionLogDTO
                );
                return CreatedAtAction(
                    nameof(Get),
                    new { id = createdTransactionLog.Id },
                    createdTransactionLog
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
