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
        public ActionResult<ResponseDTO<OrderRoutePagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto = _orderRouteRepository.GetAllOrderRoute(
                    pageSortParam
                );
                if (!responseDto.Payload.OrderRoutes.Any())
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

        // GET: api/<OrderRouteController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<OrderRouteInfoDTO>> Get(int id)
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
                
                ResponseDTO<OrderRouteInfoDTO> responseDto = new ResponseDTO<OrderRouteInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "order route retrieved successfully.",
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

        // POST api/<OrderRouteController>
        [HttpPost]
        public ActionResult<ResponseDTO<OrderRouteInfoDTO>> Post([FromBody] OrderRouteFormDTO orderRouteFormDTO)
        {
            try
            {
                if (orderRouteFormDTO == null)
                {
                    return BadRequest();
                }

                var createdOrderRoute = _orderRouteRepository.CreateOrderRoute(orderRouteFormDTO);
                
                var response = new ResponseDTO<OrderRouteInfoDTO>
                {
                    StatusCode = 201,
                    Message = "order route Register Success.",
                    TimeStamp = DateTime.Now,
                    Payload = createdOrderRoute,
                };
                return response;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<OrderRouteController>/5
        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<OrderRouteInfoDTO>> Put(int id, [FromBody]  OrderRouteFormDTO orderRouteFormDTO)
        {
            try
            {
                if (orderRouteFormDTO == null || id != orderRouteFormDTO.Id)
                {
                    return BadRequest();
                }

                var result = _orderRouteRepository.UpdateOrderRoute(orderRouteFormDTO);
                if (!result)
                {
                    return NotFound();
                }
                
                ResponseDTO<OrderRouteInfoDTO> responseDto = new ResponseDTO<OrderRouteInfoDTO>
                {
                    StatusCode = 200,
                    Message = "order route Updated Successfully.",
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

        // DELETE api/<OrderRouteController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<OrderRouteInfoDTO>> Delete(int id)
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

                ResponseDTO<OrderRouteInfoDTO> responseDto = new ResponseDTO<OrderRouteInfoDTO>
                {
                    StatusCode = 204,
                    Message = "order route Deleted Successfully.",
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
