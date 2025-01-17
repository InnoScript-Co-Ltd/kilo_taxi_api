using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

    [Route("api/v1/[controller]")]
    [ApiController]
    public class TravelRateController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly ITravelRateRepository _travelRateRepository;

        public TravelRateController(ITravelRateRepository travelRateRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _travelRateRepository = travelRateRepository;
        }


        //GET: api/<CityController>
        [HttpGet]
        public ActionResult<TravelRatePagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                TravelRatePagingDTO travelRatePagingDTO = _travelRateRepository.GetAllTravelRate(pageSortParam);
                if (!travelRatePagingDTO.TravelRates.Any())
                {
                    return NoContent();
                }
                // Add a custom header
                //Response.Headers.Add("X-Custom-Header", "foo");
                return Ok(travelRatePagingDTO);
       
            }
            catch (Exception ex) {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");

            }
        }
        [HttpGet("{id}")]
        public ActionResult<TravelRateDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var result = _travelRateRepository.GetTravelRate(id);
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
        
        // POST api/<CityController>
        [HttpPost]
        public ActionResult<TravelRateDTO> Post([FromBody] TravelRateDTO travelRateDTO)
        {
            try
            {
                if (travelRateDTO == null)
                {
                    return BadRequest();
                }
                var createdTravelRate = _travelRateRepository.AddTravelRate(travelRateDTO);
                return CreatedAtAction(nameof(Get), new { id = createdTravelRate.Id }, createdTravelRate);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");

            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] TravelRateDTO travelRateDTO)
        {
            try
            {
                if(travelRateDTO == null || id !=travelRateDTO.Id)
                {
                    return BadRequest();
                }
                var result= _travelRateRepository.UpdateTravelRate(travelRateDTO);

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

        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<TravelRateDTO>> Delete(int id)
        {
            try
            {
                var result = _travelRateRepository.DeleteTravelRate(id);
                if (!result)
                {
                    return NotFound();
                }
                ResponseDTO<TravelRateDTO> responseDto = new ResponseDTO<TravelRateDTO>();
                responseDto.StatusCode = 204;
                responseDto.Message = "TravelRate deleted";
                responseDto.TimeStamp = DateTime.Now;
                return responseDto;
            }
            catch(Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
