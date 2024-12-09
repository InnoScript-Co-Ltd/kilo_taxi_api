using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PaymentChannelController : ControllerBase
{
    private readonly LoggerHelper _logHelper;
    private readonly IPaymentChannelRepository _paymentChannelRepository;

    public PaymentChannelController(IPaymentChannelRepository paymentChannelRepository)
    {
        _logHelper = LoggerHelper.Instance;
        _paymentChannelRepository = paymentChannelRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PaymentChannelPagingDTO>> GetAll([FromQuery] PageSortParam pageSortParam)
    {
        try
        {
            PaymentChannelPagingDTO paymentChannelPagingDTO = _paymentChannelRepository.GetAllPaymentChannels(pageSortParam);
            if (!paymentChannelPagingDTO.PaymentChannels.Any())
            {
                return NoContent(); 
            }
            return Ok(paymentChannelPagingDTO);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex, "Error occurred while fetching payment channels.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }


    [HttpGet("{id}")]
    public ActionResult<PaymentChannelDTO> Get(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest("Invalid Payment Channel ID.");
            }

            var paymentChannel = _paymentChannelRepository.GetPaymentChannelById(id);
            if (paymentChannel == null)
            {
                return NotFound();
            }

            return Ok(paymentChannel);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public ActionResult<PaymentChannelDTO> Create([FromBody] PaymentChannelDTO paymentChannelDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPaymentChannel = _paymentChannelRepository.CreatePaymentChannel(paymentChannelDTO);

            return CreatedAtAction(nameof(Get), new { id = createdPaymentChannel.Id }, createdPaymentChannel);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] PaymentChannelDTO paymentChannelDTO)
    {
        try
        {
            if (id != paymentChannelDTO.Id)
            {
                return BadRequest("Payment Channel ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isUpdated = _paymentChannelRepository.UpdatePaymentChannel(paymentChannelDTO);
            if (!isUpdated)
            {
                return NotFound();
            }

            return Ok("Payment Channel updated successfully.");
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var paymentChannel = _paymentChannelRepository.GetPaymentChannelById(id);
            if (paymentChannel == null)
            {
                return NotFound();
            }

            var isDeleted = _paymentChannelRepository.DeletePaymentChannel(id);
            if (!isDeleted)
            {
                return StatusCode(500, "An error occurred while deleting the payment channel.");
            }

            return Ok($"Payment channel with ID {id} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
