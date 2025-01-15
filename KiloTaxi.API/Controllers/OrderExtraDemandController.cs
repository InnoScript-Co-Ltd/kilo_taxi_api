using Azure;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderExtraDemandController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly IOrderExtraDemandRepository _orderExtraDemandRepository;

        public OrderExtraDemandController(IOrderExtraDemandRepository orderExtraDemandRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _orderExtraDemandRepository = orderExtraDemandRepository;
        }

        // GET: api/<OrderExtraDemandController>
        [HttpGet]
        public ActionResult<ResponseDTO<OrderExtraDemandPagingDTO>> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var responseDto = _orderExtraDemandRepository.GetAllOrderExtraDemand(
                    pageSortParam
                );
                if (!responseDto.Payload.OrderExtraDemands.Any())
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

        // GET: api/<OrderExtraDemandController>/5
        [HttpGet("{id}")]
        public ActionResult<OrderExtraDemandInfoDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _orderExtraDemandRepository.GetOrderExtraDemandById(id);
                if (result == null)
                {
                    return NotFound();
                }
                ResponseDTO<OrderExtraDemandInfoDTO> responseDto = new ResponseDTO<OrderExtraDemandInfoDTO>
                {
                    StatusCode = 200,
                    Message = "order extra demand Deleted Successfully.",
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

        // POST api/<OrderExtraDemandController>
        [HttpPost]
        public ActionResult<ResponseDTO<OrderExtraDemandDTO>> Post([FromBody]List<OrderExtraDemandDTO> orderExtraDemandDTOList)
        {
            try
            {
                if (orderExtraDemandDTOList == null)
                {
                    return BadRequest();
                }

                var createdOrderExtraDemand = _orderExtraDemandRepository.CreateOrderExtraDemand(orderExtraDemandDTOList);
                ResponseDTO<OrderExtraDemandDTO>response=new ResponseDTO<OrderExtraDemandDTO>();
                response.StatusCode = Ok().StatusCode;
                response.Message = "OrderExtraDemand created Success.";
                response.TimeStamp=DateTime.Now;
                response.PayloadList = createdOrderExtraDemand;
                return response;
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<OrderExtraDemandController>/5
        [HttpPut("{id}")]
        public ActionResult<ResponseDTO<OrderExtraDemandDTO>> Put(int id, [FromBody] OrderExtraDemandDTO orderExtraDemandDTO)
        {
            try
            {
                if (orderExtraDemandDTO == null || id != orderExtraDemandDTO.Id)
                {
                    return BadRequest();
                }

                var result = _orderExtraDemandRepository.UpdateOrderExtraDemand(orderExtraDemandDTO);
                if (!result)
                {
                    return NotFound();
                }
                
                ResponseDTO<OrderExtraDemandDTO> responseDto = new ResponseDTO<OrderExtraDemandDTO>
                {
                    StatusCode = 200,
                    Message = "order extra demand Updated Successfully.",
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

        // DELETE api/<OrderExtraDemandController>/5
        [HttpDelete("{id}")]
        public ActionResult<ResponseDTO<OrderExtraDemandInfoDTO>> Delete(int id)
        {
            try
            {
                var orderExtraDemand = _orderExtraDemandRepository.GetOrderExtraDemandById(id);
                if (orderExtraDemand == null)
                {
                    return NotFound();
                }

                var result = _orderExtraDemandRepository.DeleteOrderExtraDemand(id);
                if (!result)
                {
                    return NotFound();
                }

                ResponseDTO<OrderExtraDemandInfoDTO> responseDto = new ResponseDTO<OrderExtraDemandInfoDTO>
                {
                    StatusCode = 200,
                    Message = "order extra demand Info Deleted Successfully.",
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
