using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        LoggerHelper _logHelper;
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _logHelper = LoggerHelper.Instance;
            _customerRepository = customerRepository;
        }

        // GET: api/<CustomerController>
        [HttpGet]
        public ActionResult<CustomerPagingDTO> Get([FromQuery] PageSortParam pageSortParam)
        {
            try
            {
                var customerPagingDTO = _customerRepository.GetAllCustomer(pageSortParam);
                if (customerPagingDTO.Customers.Any() == false)
                {
                    return NoContent();
                }
                return Ok(customerPagingDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/<CustomerController>/5
        [HttpGet("{id}")]
        public ActionResult<CustomerDTO> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var result = _customerRepository.GetCustomerById(id);
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

        // POST api/<CustomerController>
        [HttpPost]
        public ActionResult<CustomerDTO> Post([FromBody] CustomerDTO customerDTO)
        {
            try
            {
                if (customerDTO == null)
                {
                    return BadRequest();
                }

                var createdCustomer = _customerRepository.AddCustomer(customerDTO);
                return CreatedAtAction(
                    nameof(Get),
                    new { id = createdCustomer.Id },
                    createdCustomer
                );
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] CustomerDTO customerDTO)
        {
            try
            {
                if (customerDTO == null || id != customerDTO.Id)
                {
                    return BadRequest();
                }

                var result = _customerRepository.UpdateCustomer(customerDTO);
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

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var customer = _customerRepository.GetCustomerById(id);
                if (customer == null)
                {
                    return NotFound();
                }

                var result = _customerRepository.DeleteCustomer(id);
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
