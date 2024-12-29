using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
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
        public ActionResult<OrderExtendPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                OrderExtendPagingDTO orderExtendPagingDTO = _orderExtendRepository.GetAllOrderExtend(
                    pageSortParam
                );
                if (!orderExtendPagingDTO.OrderExtends.Any())
                {
                    return NoContent();
                }
                return Ok(orderExtendPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<OrderExtendController>/5
        [HttpGet("{id}")]
        public ActionResult<OrderExtendDTO> Get(int id)
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<OrderExtendController>
        [HttpPost]
        public ActionResult<OrderExtendDTO> Post([FromBody] OrderExtendDTO orderExtendDTO)
        {
            try
            {
                if (orderExtendDTO == null)
                {
                    return BadRequest();
                }

                var createdOrderExtend = _orderExtendRepository.CreateOrderExtend(orderExtendDTO);
                return CreatedAtAction(
                    nameof(Get),
                    new { id = createdOrderExtend.Id },
                    createdOrderExtend
                );
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<OrderExtendController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] OrderExtendDTO orderExtendDTO)
        {
            try
            {
                if (orderExtendDTO == null || id != orderExtendDTO.Id)
                {
                    return BadRequest();
                }

                var result = _orderExtendRepository.UpdateOrderExtend(orderExtendDTO);
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

        // DELETE api/<OrderExtendController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
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
