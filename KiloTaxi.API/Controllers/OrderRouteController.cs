using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderRouteController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IOrderRouteRepository _orderRouteRepository;

        public OrderRouteController(IOrderRouteRepository orderRouteRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _orderRouteRepository = orderRouteRepository;
        }

        // GET: api/<OrderRouteController>
        [HttpGet]
        public ActionResult<OrderRoutePagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                OrderRoutePagingDTO orderRoutePagingDTO = _orderRouteRepository.GetAllOrderRoute(
                    pageSortParam
                );
                if (!orderRoutePagingDTO.OrderRoutes.Any())
                {
                    return NoContent();
                }
                return Ok(orderRoutePagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<OrderRouteController>/5
        [HttpGet("{id}")]
        public ActionResult<OrderRouteDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _orderRouteRepository.GetOrderRouteById(id);
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

        // POST api/<OrderRouteController>
        [HttpPost]
        public ActionResult<OrderRouteDTO> Post([FromBody] OrderRouteDTO orderRouteDTO)
        {
            try
            {
                if (orderRouteDTO == null)
                {
                    return BadRequest();
                }

                var createdOrderRoute = _orderRouteRepository.CreateOrderRoute(orderRouteDTO);
                return CreatedAtAction(
                    nameof(Get),
                    new { id = createdOrderRoute.Id },
                    createdOrderRoute
                );
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<OrderRouteController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] OrderRouteDTO orderRouteDTO)
        {
            try
            {
                if (orderRouteDTO == null || id != orderRouteDTO.Id)
                {
                    return BadRequest();
                }

                var result = _orderRouteRepository.UpdateOrderRoute(orderRouteDTO);
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

        // DELETE api/<OrderRouteController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var orderRoute = _orderRouteRepository.GetOrderRouteById(id);
                if (orderRoute == null)
                {
                    return NotFound();
                }

                var result = _orderRouteRepository.DeleteOrderRoute(id);
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
