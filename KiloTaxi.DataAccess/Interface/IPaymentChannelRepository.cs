using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IPaymentChannelRepository
{
    ResponseDTO<PaymentChannelPagingDTO> GetAllPaymentChannels(PageSortParam pageSortParam);

    PaymentChannelInfoDTO CreatePaymentChannel(PaymentChannelFormDTO paymentChannelFormDTO);

    bool UpdatePaymentChannel(PaymentChannelFormDTO paymentChannelFormDTO);

    PaymentChannelInfoDTO GetPaymentChannelById(int id);
    
    bool DeletePaymentChannel(int id);
}
