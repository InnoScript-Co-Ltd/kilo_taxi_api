using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
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
        public ActionResult<ResponseDTO<PromotionPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto = _promotionRepository.GetAllPromotion(
                    pageSortParam
                );
                if (!responseDto.Payload.Promotions.Any())
                {
                    return NoContent();
                }
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<PromotionController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<PromotionInfoDTO>> Get(int id)
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
                
                ResponseDTO<PromotionInfoDTO> responseDto = new ResponseDTO<PromotionInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "promotion retrieved successfully.",
                    TimeStamp = DateTime.Now,
                    Payload = result,
                };
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<PromotionController>
        [HttpPost]
        public ActionResult<ResponseDTO<PromotionInfoDTO>> Post([FromBody] PromotionFormDTO promotionFormDTO)
        {
            try
            {
                if (promotionFormDTO == null)
                {
                    return BadRequest();
                }

                var createdPromotion = _promotionRepository.AddPromotion(promotionFormDTO);
                
                var response = new ResponseDTO<PromotionInfoDTO>
                {
                    StatusCode = 201,
                    Message = "promotion Register Success.",
                    TimeStamp = DateTime.Now,
                    Payload = createdPromotion,
                };
                return response;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<PromotionController>/5
        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<PromotionInfoDTO>> Put(int id, [FromBody]PromotionFormDTO promotionFormDTO)
        {
            try
            {
                if (promotionFormDTO == null || id != promotionFormDTO.Id)
                {
                    return BadRequest();
                }

                var result = _promotionRepository.UpdatePromotion(promotionFormDTO);
                if (!result)
                {
                    return NotFound();
                }
                
                ResponseDTO<PromotionInfoDTO> responseDto = new ResponseDTO<PromotionInfoDTO>
                {
                    StatusCode = 200,
                    Message = "promotion Updated Successfully.",
                    TimeStamp = DateTime.Now,
                    Payload = null,
                };
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE api/<PromotionController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<PromotionInfoDTO>> Delete(int id)
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

                ResponseDTO<PromotionInfoDTO> responseDto = new ResponseDTO<PromotionInfoDTO>
                {
                    StatusCode = 204,
                    Message = "promotion Deleted Successfully.",
                    TimeStamp = DateTime.Now,
                    Payload = null,
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
