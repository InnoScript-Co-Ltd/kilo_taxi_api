using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
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
        public ActionResult<ReviewPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                ReviewPagingDTO reviewPagingDTO = _reviewRepository.GetAllReview(pageSortParam);
                if (!reviewPagingDTO.Reviews.Any())
                {
                    return NoContent();
                }
                return Ok(reviewPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<ReviewController>/5
        [HttpGet("{id}")]
        public ActionResult<ReviewDTO> Get(int id)
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<ReviewController>
        [HttpPost]
        public ActionResult<ReviewDTO> Post([FromBody] ReviewDTO reviewDTO)
        {
            try
            {
                if (reviewDTO == null)
                {
                    return BadRequest();
                }

                var createdReview = _reviewRepository.AddReview(reviewDTO);
                return CreatedAtAction(nameof(Get), new { id = createdReview.Id }, createdReview);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<ReviewController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] ReviewDTO reviewDTO)
        {
            try
            {
                if (reviewDTO == null || id != reviewDTO.Id)
                {
                    return BadRequest();
                }

                var result = _reviewRepository.UpdateReview(reviewDTO);
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

        // DELETE api/<ReviewController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
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
