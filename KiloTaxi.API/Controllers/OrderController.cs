using KiloTaxi.API.Services;
using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IOrderRepository _orderRepository;
        private ApiClientHub _apiClientHub;


        public OrderController(IOrderRepository orderRepository,ApiClientHub apiClientHub)
        {
            _logHelper = LoggerHelper.Instance;
            _orderRepository = orderRepository;
            _apiClientHub = apiClientHub;
        }

        //GET: api/<AdminController>
        [HttpGet]
        public ActionResult<IEnumerable<OrderPagingDTO>> GetAll([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                OrderPagingDTO orderPagingDTO = _orderRepository.GetAllOrder(pageSortParam);
                if (!orderPagingDTO.Orders.Any())
                {
                    return NoContent();
                }
                return Ok(orderPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<OrderDTO> Get(int id)
        {
            try
            {
                _logHelper.LogInfo("test info log");
                if (id == 0)
                {
                    return BadRequest();
                }
                var orderDTO = _orderRepository.GetOrderById(id);
                if (orderDTO == null)
                {
                    return NotFound();
                }
                return Ok(orderDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        
        // POST api/<AdminController>
        [HttpPost]
        public ActionResult<OrderDTO> Post([FromBody] OrderDTO orderDTO)
        {
            try
            {
                _apiClientHub.SendMessageAsync($"Get all orders");
                _apiClientHub.SendOrderAsync(orderDTO);

                if (orderDTO == null)
                {
                    return BadRequest();
                }
        
                var createdOrder = _orderRepository.AddOrder(orderDTO);
                return CreatedAtAction(nameof(Get), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] OrderDTO orderDTO)
        {
            try
            {
                if (orderDTO == null)
                {
                    return BadRequest("Request body is missing.");
                }

                if ( id != orderDTO.Id)
                {
                    return BadRequest($"Route ID ({id}) does not match body ID ({orderDTO.Id}).");
                }

                var result = _orderRepository.UpdateOrder(orderDTO);
        
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
        
        // // DELETE api/<AdminController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var order = _orderRepository.GetOrderById(id);
                if (order == null)
                {
                    return NotFound();
                }
        
                var result = _orderRepository.DeleteOrder(id);
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
