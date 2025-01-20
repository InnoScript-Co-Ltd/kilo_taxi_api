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
    public class OrderExtendController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IOrderExtendRepository _orderExtendRepository;

        public OrderExtendController(IOrderExtendRepository orderExtendRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _orderExtendRepository = orderExtendRepository;
        }

        // GET: api/<OrderExtendController>
        [HttpGet]
        public ActionResult<ResponseDTO<OrderExtendPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto = _orderExtendRepository.GetAllOrderExtend(
                    pageSortParam
                );
                if (!responseDto.Payload.OrderExtends.Any())
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

        // GET: api/<OrderExtendController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseDTO<OrderExtendInfoDTO>> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _orderExtendRepository.GetOrderExtendById(id);
                if (result == null)
                {
                    return NotFound();
                }
                
                ResponseDTO<OrderExtendInfoDTO> responseDto = new ResponseDTO<OrderExtendInfoDTO>
                {
                    StatusCode = Ok().StatusCode,
                    Message = "order extend retrieved successfully.",
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

        // POST api/<OrderExtendController>
        [HttpPost]
        public ActionResult<ResponseDTO<OrderExtendInfoDTO>> Post([FromBody] OrderExtendFormDTO orderExtendFormDTO)
        {
            try
            {
                if (orderExtendFormDTO == null)
                {
                    return BadRequest();
                }

                var createdOrderExtend = _orderExtendRepository.CreateOrderExtend(orderExtendFormDTO);
                
                var response = new ResponseDTO<OrderExtendInfoDTO>
                {
                    StatusCode = 201,
                    Message = "order extend Register Success.",
                    TimeStamp = DateTime.Now,
                    Payload = createdOrderExtend,
                };
                return response;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<OrderExtendController>/5
        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<OrderExtendInfoDTO>> Put(int id, [FromBody] OrderExtendFormDTO orderExtendFormDTO)
        {
            try
            {
                if (orderExtendFormDTO == null || id != orderExtendFormDTO.Id)
                {
                    return BadRequest();
                }

                var result = _orderExtendRepository.UpdateOrderExtend(orderExtendFormDTO);
                if (!result)
                {
                    return NotFound();
                }
                
                ResponseDTO<OrderExtendInfoDTO> responseDto = new ResponseDTO<OrderExtendInfoDTO>
                {
                    StatusCode = 200,
                    Message = "order extenc Updated Successfully.",
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

        // DELETE api/<OrderExtendController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<OrderExtendInfoDTO>> Delete(int id)
        {
            try
            {
                var orderExtend = _orderExtendRepository.GetOrderExtendById(id);
                if (orderExtend == null)
                {
                    return NotFound();
                }

                var result = _orderExtendRepository.DeleteOrderExtend(id);
                if (!result)
                {
                    return NotFound();
                }

                ResponseDTO<OrderExtendInfoDTO> responseDto = new ResponseDTO<OrderExtendInfoDTO>
                {
                    StatusCode = 204,
                    Message = "order extend Deleted Successfully.",
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
