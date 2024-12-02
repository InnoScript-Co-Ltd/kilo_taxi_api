using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PromotionUsageController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IPromotionUsageRepository _promotionUsageRepository;

        public PromotionUsageController(IPromotionUsageRepository promotionUsageRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _promotionUsageRepository = promotionUsageRepository;
        }

        // GET: api/<PromotionUsageController>
        [HttpGet]
        public ActionResult<PromotionUsagePagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                PromotionUsagePagingDTO promotionUsagePagingDTO =
                    _promotionUsageRepository.GetAllPromotionUsage(pageSortParam);
                if (!promotionUsagePagingDTO.promotionUsages.Any())
                {
                    return NoContent();
                }
                return Ok(promotionUsagePagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<PromotionUsageController>/5
        [HttpGet("{id}")]
        public ActionResult<PromotionUsageDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _promotionUsageRepository.GetPromotionUsageById(id);
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

        // POST api/<PromotionController>
        [HttpPost]
        public ActionResult<PromotionUsageDTO> Post([FromBody] PromotionUsageDTO promotionUsageDTO)
        {
            try
            {
                if (promotionUsageDTO == null)
                {
                    return BadRequest();
                }

                var createdPromotion = _promotionUsageRepository.AddPromotionUsage(
                    promotionUsageDTO
                );
                return CreatedAtAction(
                    nameof(Get),
                    new { id = createdPromotion.Id },
                    createdPromotion
                );
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<PromotionController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] PromotionUsageDTO promotionUsageDTO)
        {
            try
            {
                if (promotionUsageDTO == null || id != promotionUsageDTO.Id)
                {
                    return BadRequest();
                }

                var result = _promotionUsageRepository.UpdatePromotionUsage(promotionUsageDTO);
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

        // DELETE api/<PromotionController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var promotion = _promotionUsageRepository.GetPromotionUsageById(id);
                if (promotion == null)
                {
                    return NotFound();
                }

                var result = _promotionUsageRepository.DeletePromotionUsage(id);
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
