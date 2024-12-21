using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WalletUserMappingController : ControllerBase
    {
        private readonly IWalletUserMappingRepository _walletUserMappingRepository;
        private readonly LoggerHelper _logHelper;

        public WalletUserMappingController(IWalletUserMappingRepository walletUserMappingRepository)
        {
            _walletUserMappingRepository = walletUserMappingRepository;
            _logHelper = LoggerHelper.Instance;
        }

        [HttpPost]
        public ActionResult<WalletUserMappingDTO> Create([FromBody] WalletUserMappingDTO walletUserMappingDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdMapping = _walletUserMappingRepository.CreateWalletUserMapping(walletUserMappingDTO);

                return CreatedAtAction(nameof(Create), new { id = createdMapping.Id }, createdMapping);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while creating WalletUserMapping.");
            }
        }

    }
}
