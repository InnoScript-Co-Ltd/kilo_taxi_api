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
        public ActionResult<ResponseDTO<PromotionUsagePagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto =
                    _promotionUsageRepository.GetAllPromotionUsage(pageSortParam);
                if (!responseDto.Payload.promotionUsages.Any())
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

        // GET: api/<PromotionUsageController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<PromotionUsageInfoDTO>> Get(int id)
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
                
                ResponseDTO<PromotionUsageInfoDTO> responseDto = new ResponseDTO<PromotionUsageInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "promotion usage retrieved successfully.",
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
        public ActionResult<ResponseDTO<PromotionUsageInfoDTO>> Post([FromBody] PromotionUsageFormDTO promotionUsageFormDTO)
        {
            try
            {
                if (promotionUsageFormDTO == null)
                {
                    return BadRequest();
                }

                var createdPromotionUsage = _promotionUsageRepository.AddPromotionUsage(
                    promotionUsageFormDTO
                );
                
                var response = new ResponseDTO<PromotionUsageInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "promotion usage Register Success.",
                    TimeStamp = DateTime.Now,
                    Payload = createdPromotionUsage,
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
        public ActionResult<ResponseDTO<PromotionUsageInfoDTO>> Put(int id, [FromBody] PromotionUsageFormDTO promotionUsageFormDTO)
        {
            try
            {
                if (promotionUsageFormDTO == null || id != promotionUsageFormDTO.Id)
                {
                    return BadRequest();
                }

                var result = _promotionUsageRepository.UpdatePromotionUsage(promotionUsageFormDTO);
                if (!result)
                {
                    return NotFound();
                }
                
                ResponseDTO<PromotionUsageInfoDTO> responseDto = new ResponseDTO<PromotionUsageInfoDTO>
                {
                    StatusCode = 200,
                    Message = "promotion usage Updated Successfully.",
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
        public ActionResult<ResponseDTO<PromotionUsageInfoDTO>> Delete(int id)
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

                ResponseDTO<PromotionUsageInfoDTO> responseDto = new ResponseDTO<PromotionUsageInfoDTO>
                {
                    StatusCode = 200,
                    Message = "promotion usage Deleted Successfully.",
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
