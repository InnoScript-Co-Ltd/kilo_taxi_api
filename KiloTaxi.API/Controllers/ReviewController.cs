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
    public class ReviewController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IReviewRepository _reviewRepository;

        public ReviewController(IReviewRepository reviewRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _reviewRepository = reviewRepository;
        }

        // GET: api/<ReviewController>
        [HttpGet]
        public ActionResult<ResponseDTO<ReviewPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDTO = _reviewRepository.GetAllReview(pageSortParam);
                if (!responseDTO.Payload.Reviews.Any())
                {
                    return NoContent();
                }
                return Ok(responseDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<ReviewController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<ReviewInfoDTO>> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _reviewRepository.GetReviewById(id);
                if (result == null)
                {
                    return NotFound();
                }
                ResponseDTO<ReviewInfoDTO> responseDto = new ResponseDTO<ReviewInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "review retrieved successfully.",
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

        // POST api/<ReviewController>
        [HttpPost]
        public ActionResult<ResponseDTO<ReviewInfoDTO>> Post([FromBody] ReviewFormDTO reviewFormDTO)
        {
            try
            {
                if (reviewFormDTO == null)
                {
                    return BadRequest();
                }

                var createdReview = _reviewRepository.AddReview(reviewFormDTO);
                
                var response = new ResponseDTO<ReviewInfoDTO>
                {
                    StatusCode = 201,
                    Message = "review Register Success.",
                    Payload = createdReview,
                    TimeStamp = DateTime.Now,
                };
                return response;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<ReviewController>/5
        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<ReviewInfoDTO>> Put(int id, [FromBody] ReviewFormDTO reviewFormDTO)
        {
            try
            {
                if (reviewFormDTO == null || id != reviewFormDTO.Id)
                {
                    return BadRequest();
                }

                var result = _reviewRepository.UpdateReview(reviewFormDTO);
                if (!result)
                {
                    return NotFound();
                }
                ResponseDTO<ReviewInfoDTO> responseDto = new ResponseDTO<ReviewInfoDTO>
                {
                    StatusCode = 200,
                    Message = "review Updated Successfully.",
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

        // DELETE api/<ReviewController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<ReviewInfoDTO>> Delete(int id)
        {
            try
            {
                var review = _reviewRepository.GetReviewById(id);
                if (review == null)
                {
                    return NotFound();
                }

                var result = _reviewRepository.DeleteReview(id);
                if (!result)
                {
                    return NotFound();
                }

                ResponseDTO<ReviewInfoDTO> responseDto = new ResponseDTO<ReviewInfoDTO>
                {
                    StatusCode = 204,
                    Message = "review Deleted Successfully.",
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
