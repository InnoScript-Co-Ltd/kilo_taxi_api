using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

    [Route("api/v1/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly ICityRepository _cityRepository;

        public CityController(ICityRepository cityRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _cityRepository = cityRepository;
        }


        //GET: api/<CityController>
        [HttpGet]
        public ActionResult<CityPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                CityPagingDTO cityPaginDTO = _cityRepository.GetAllCity(pageSortParam);
                if (!cityPaginDTO.Cities.Any())
                {
                    return NoContent();
                }
                // Add a custom header
                //Response.Headers.Add("X-Custom-Header", "foo");
                return Ok(cityPaginDTO);
       
            }
            catch (Exception ex) {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");

            }
        }
        [HttpGet("{id}")]
        public ActionResult<CityDTO> Get(int id)
        {
            try
            {
                _logHelper.LogInfo("test info log");
                if (id == 0)
                {
                    return BadRequest();
                }
                var result = _cityRepository.GetCity(id);
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
        public ActionResult<CityDTO> Post([FromBody] CityDTO cityDTO)
        {
            try
            {
                if (cityDTO == null)
                {
                    return BadRequest();
                }
                var createdCity = _cityRepository.AddCity(cityDTO);
                return CreatedAtAction(nameof(Get), new { id = createdCity.Id }, createdCity);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");

            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] CityDTO cityDTO)
        {
            try
            {
                if(cityDTO == null || id !=cityDTO.Id)
                {
                    return BadRequest();
                }
                var result= _cityRepository.UpdateCity(cityDTO);

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
        public ActionResult Delete(int id)
        {
            try
            {
                var result = _cityRepository.DeleteCity(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch(Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
