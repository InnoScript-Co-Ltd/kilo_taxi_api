using KiloTaxi.API.Services;
using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IOrderRepository _orderRepository;
        private ApiClientHub _apiClientHub;
        private readonly IDriverRepository _driverRepository;

        public OrderController(
            IOrderRepository orderRepository,
            ApiClientHub apiClientHub,
            IDriverRepository driverRepository
        )
        {
            _logHelper = LoggerHelper.Instance;
            _orderRepository = orderRepository;
            _apiClientHub = apiClientHub;
            _driverRepository = driverRepository;
        }

        //GET: api/<AdminController>
        [HttpGet]
        public ActionResult<ResponseDTO<OrderPagingDTO>> GetAll(
            [FromQuery] PageSortParam pageSortParam
        )
        {
            try
            {
                var responseDto = _orderRepository.GetAllOrder(pageSortParam);
                if (!responseDto.Payload.Orders.Any())
                {
                    return NoContent();
                }
                return responseDto;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<OrderInfoDTO> Get(int id)
        {
            try
            {
                _logHelper.LogInfo("test info log");
                if (id == 0)
                {
                    return BadRequest();
                }
                var orderInfoDTO = _orderRepository.GetOrderById(id);
                if (orderInfoDTO == null)
                {
                    return NotFound();
                }
                return Ok(orderInfoDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<AdminController>
        // [HttpPost]
        // public ActionResult<OrderDTO> Post([FromBody] OrderDTO orderDTO)
        // {
        //     try
        //     {
        //         _apiClientHub.SendMessageAsync($"Get all orders");

        //         if (orderDTO == null)
        //         {
        //             return BadRequest();
        //         }

        //         var createdOrder = _orderRepository.AddOrder(orderDTO);
        //         _apiClientHub.SendOrderAsync(createdOrder, _driverRepository);
        //         return CreatedAtAction(nameof(Get), new { id = createdOrder.Id }, createdOrder);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logHelper.LogError(ex);
        //         return StatusCode(500, "An error occurred while processing your request.");
        //     }
        // }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] OrderFormDTO orderFormDTO)
        {
            try
            {
                if (orderFormDTO == null)
                {
                    return BadRequest("Request body is missing.");
                }

                if (id != orderFormDTO.Id)
                {
                    return BadRequest(
                        $"Route ID ({id}) does not match body ID ({orderFormDTO.Id})."
                    );
                }

                var result = _orderRepository.UpdateOrder(orderFormDTO);

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

        [HttpGet("GetAllOrders")]
        public ActionResult<ResponseDTO<OrderPagingDTO>> GetAllOrders(
            [FromQuery] PageSortParam pageSortParam
        )
        {
            try
            {
                var responseDto = _orderRepository.GetAllOrder(pageSortParam);

                if (responseDto?.Payload?.Orders == null || !responseDto.Payload.Orders.Any())
                {
                    return NoContent();
                }

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(
                    500,
                    new
                    {
                        Message = "An error occurred while processing your request.",
                        Details = ex.Message,
                    }
                );
            }
        }

        [HttpPost("CreateOrder")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDTO<OrderInfoDTO>>> CreateOrder(
            OrderFormDTO orderFormDTO
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Register the admin
                var createdOrder = _orderRepository.AddOrder(orderFormDTO);

                // Prepare response
                var response = new ResponseDTO<OrderInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "Order Register Success.",
                    Payload = createdOrder,
                    TimeStamp = DateTime.Now,
                };

                return response;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                throw new Exception("An error occurred while processing your request.");
            }
        }

        // Get order by ID
        [HttpGet("GetOrderById/{id}")]
        public ActionResult<ResponseDTO<OrderInfoDTO>> GetOrderById(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("Invalid order ID.");
                }

                var result = _orderRepository.GetOrderById(id);
                if (result == null)
                {
                    return NotFound();
                }

                ResponseDTO<OrderInfoDTO> responseDto = new ResponseDTO<OrderInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "Order retrieved successfully.",
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

        // Update order
        [HttpPut("UpdateOrder/{id}")]
        public async Task<ActionResult<ResponseDTO<OrderInfoDTO>>> UpdateOrder(
            [FromRoute] int id,
            OrderFormDTO orderFormDTO
        )
        {
            try
            {
                if (id != orderFormDTO.Id)
                {
                    return BadRequest("Order ID mismatch.");
                }

                // Check if the order exists
                var existingOrder = _orderRepository.GetOrderById(id);
                if (existingOrder == null)
                {
                    return NotFound();
                }

                ResponseDTO<OrderInfoDTO> responseDto = new ResponseDTO<OrderInfoDTO>
                {
                    StatusCode = 200,
                    Message = "Order Info Updated Successfully.",
                    Payload =
                        null // No payload since we're just updating the order
                    ,
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Delete order
        [HttpGet("DeleteOrder/{id}")]
        public ActionResult<ResponseDTO<OrderInfoDTO>> DeleteOrder([FromRoute] int id)
        {
            try
            {
                var deleteEntity = _orderRepository.GetOrderById(id);
                if (deleteEntity == null)
                {
                    return NotFound();
                }

                ResponseDTO<OrderInfoDTO> responseDto = new ResponseDTO<OrderInfoDTO>
                {
                    StatusCode = 200,
                    Message = "Order Info Deleted Successfully.",
                    Payload =
                        null // No payload since we are deleting the order
                    ,
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
