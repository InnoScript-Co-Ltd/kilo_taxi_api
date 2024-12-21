using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IPaymentChannelRepository
{
    PaymentChannelDTO CreatePaymentChannel(PaymentChannelDTO paymentChannelDTO);

    bool UpdatePaymentChannel(PaymentChannelDTO paymentChannelDTO);

    PaymentChannelDTO GetPaymentChannelById(int id);

    PaymentChannelPagingDTO GetAllPaymentChannels(PageSortParam pageSortParam);

    bool DeletePaymentChannel(int id);
}
