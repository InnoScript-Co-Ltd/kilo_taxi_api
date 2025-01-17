using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IOrderExtendRepository
{
    ResponseDTO<OrderExtendPagingDTO> GetAllOrderExtend(PageSortParam pageSortParam);
    OrderExtendInfoDTO CreateOrderExtend(OrderExtendFormDTO orderExtendFormDTO);
    bool UpdateOrderExtend(OrderExtendFormDTO orderExtendFormDTO);
    OrderExtendInfoDTO GetOrderExtendById(int id);
    bool DeleteOrderExtend(int id);
}
