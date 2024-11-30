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

        public OrderController(IOrderRepository orderRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _orderRepository = orderRepository;
        }

        //GET: api/<AdminController>
        // [HttpGet]
        // public ActionResult<AdminPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        // {
        //     try
        //     {
        //         AdminPagingDTO adminPagingDTO = _adminRepository.GetAllAdmin(pageSortParam);
        //         if (!adminPagingDTO.Admins.Any())
        //         {
        //             return NoContent();
        //         }
        //         // Add a custom header
        //         //Response.Headers.Add("X-Custom-Header", "foo");
        //         return Ok(adminPagingDTO);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logHelper.LogError(ex);
        //         return StatusCode(500, "An error occurred while processing your request.");
        //     }
        // }

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
                if (orderDTO == null)
                {
                    return BadRequest();
                }
        
                var createdOrder = _orderRepository.AddOrder(orderDTO);
                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] OrderDTO orderDTO)
        {
            try
            {
                if (orderDTO == null || id != orderDTO.Id)
                {
                    return BadRequest();
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
