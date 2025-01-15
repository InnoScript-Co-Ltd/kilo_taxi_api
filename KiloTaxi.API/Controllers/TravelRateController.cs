using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
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
        public ActionResult<ResponseDTO<TravelRatePagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto = _travelRateRepository.GetAllTravelRate(pageSortParam);
                if (!responseDto.Payload.TravelRates.Any())
                {
                    return NoContent();
                }
                // Add a custom header
                //Response.Headers.Add("X-Custom-Header", "foo");
                return Ok(responseDto);
       
            }
            catch (Exception ex) {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");

            }
        }
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<TravelRateInfoDTO>> Get(int id)
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
                
                ResponseDTO<TravelRateInfoDTO> responseDto = new ResponseDTO<TravelRateInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "travel rate retrieved successfully.",
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
        
        // POST api/<CityController>
        [HttpPost]
        public ActionResult<ResponseDTO<TravelRateInfoDTO>> Post([FromBody] TravelRateFormDTO travelRateFormDTO)
        {
            try
            {
                if (travelRateFormDTO == null)
                {
                    return BadRequest();
                }
                var createdTravelRate = _travelRateRepository.AddTravelRate(travelRateFormDTO);
                
                var response = new ResponseDTO<TravelRateInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "travel rate Register Success.",
                    Payload = createdTravelRate,
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

        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<TravelRateInfoDTO>> Put(int id, [FromBody] TravelRateFormDTO travelRateFormDTO)
        {
            try
            {
                if(travelRateFormDTO == null || id !=travelRateFormDTO.Id)
                {
                    return BadRequest();
                }
                var result= _travelRateRepository.UpdateTravelRate(travelRateFormDTO);

                if (!result)
                {
                    return NotFound();
                }
                
                ResponseDTO<TravelRateInfoDTO> responseDto = new ResponseDTO<TravelRateInfoDTO>
                {
                    StatusCode = 200,
                    Message = "travel rate Updated Successfully.",
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

        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<TravelRateInfoDTO>> Delete(int id)
        {
            try
            {
                var result = _travelRateRepository.DeleteTravelRate(id);
                if (!result)
                {
                    return NotFound();
                }

                ResponseDTO<TravelRateInfoDTO> responseDto = new ResponseDTO<TravelRateInfoDTO>
                {
                    StatusCode = 200,
                    Message = "travel rate Deleted Successfully.",
                    Payload = null,
                };
                return Ok(responseDto);
            }
            catch(Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
