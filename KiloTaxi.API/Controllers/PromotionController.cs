using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IPromotionRepository _promotionRepository;

        public PromotionController(IPromotionRepository promotionRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _promotionRepository = promotionRepository;
        }

        // GET: api/<PromotionController>
        [HttpGet]
        public ActionResult<PromotionPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                PromotionPagingDTO promotionPagingDTO = _promotionRepository.GetAllPromotion(
                    pageSortParam
                );
                if (!promotionPagingDTO.Promotions.Any())
                {
                    return NoContent();
                }
                return Ok(promotionPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<PromotionController>/5
        [HttpGet("{id}")]
        public ActionResult<PromotionDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _promotionRepository.GetPromotionById(id);
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
        public ActionResult<PromotionDTO> Post([FromBody] PromotionDTO promotionDTO)
        {
            try
            {
                if (promotionDTO == null)
                {
                    return BadRequest();
                }

                var createdPromotion = _promotionRepository.AddPromotion(promotionDTO);
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
        public ActionResult Put(int id, [FromBody] PromotionDTO promotionDTO)
        {
            try
            {
                if (promotionDTO == null || id != promotionDTO.Id)
                {
                    return BadRequest();
                }

                var result = _promotionRepository.UpdatePromotion(promotionDTO);
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
                var promotion = _promotionRepository.GetPromotionById(id);
                if (promotion == null)
                {
                    return NotFound();
                }

                var result = _promotionRepository.DeletePromotion(id);
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
