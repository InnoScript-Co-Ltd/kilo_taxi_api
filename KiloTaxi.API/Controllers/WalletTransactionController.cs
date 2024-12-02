using KiloTaxi.API.Helper.FileHelpers;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WalletTransactionController : ControllerBase
    {
        private readonly LoggerHelper _logHelper;
        private readonly IWalletTransactionRepository _walletTransactionRepository;

        public WalletTransactionController(IWalletTransactionRepository walletTransactionRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _walletTransactionRepository = walletTransactionRepository;
        }

        // GET: api/v1/WalletTransaction
        [HttpGet]
        public ActionResult<List<WalletTransactionDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var transactions = _walletTransactionRepository.GetAllWalletTransactions(pageSortParam);
                if (!transactions.Any())
                {
                    return NoContent();
                }
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/v1/WalletTransaction/{id}
        [HttpGet("{id}")]
        public ActionResult<WalletTransactionDTO> Get(int id)
        {
            try
            {
                var transaction = _walletTransactionRepository.GetWalletTransactionById(id);
                if (transaction == null)
                {
                    return NotFound();
                }
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/v1/WalletTransaction
        [HttpPost]
        public ActionResult<WalletTransactionDTO> Post(WalletTransactionDTO walletTransactionDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdTransaction = _walletTransactionRepository.CreateWalletTransaction(walletTransactionDTO);
                return CreatedAtAction(nameof(Get), new { id = createdTransaction.Id }, createdTransaction);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/v1/WalletTransaction/{id}
        [HttpPut("{id}")]
        public ActionResult Put(int id, WalletTransactionDTO walletTransactionDTO)
        {
            try
            {
                if (id != walletTransactionDTO.Id)
                {
                    return BadRequest("Transaction ID mismatch");
                }

                var success = _walletTransactionRepository.UpdateWalletTransaction(walletTransactionDTO);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent(); // Indicates successful update
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/v1/WalletTransaction/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var success = _walletTransactionRepository.DeleteWalletTransaction(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent(); // Indicates successful deletion
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
