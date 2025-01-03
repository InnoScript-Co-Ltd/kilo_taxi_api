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
        public ActionResult<OrderExtraDemandPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                OrderExtraDemandPagingDTO orderExtraDemandPagingDTO = _orderExtraDemandRepository.GetAllOrderExtraDemand(
                    pageSortParam
                );
                if (!orderExtraDemandPagingDTO.OrderExtraDemands.Any())
                {
                    return NoContent();
                }
                return Ok(orderExtraDemandPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<OrderExtraDemandController>/5
        [HttpGet("{id}")]
        public ActionResult<OrderExtraDemandDTO> Get(int id)
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
                return Ok(result);
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
        public ActionResult Put(int id, [FromBody] OrderExtraDemandDTO orderExtraDemandDTO)
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
                return Ok();
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE api/<OrderExtraDemandController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
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
