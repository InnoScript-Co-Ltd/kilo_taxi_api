using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IOrderExtraDemandRepository
{
    ResponseDTO<OrderExtraDemandPagingDTO> GetAllOrderExtraDemand(PageSortParam pageSortParam);
    List<OrderExtraDemandDTO> CreateOrderExtraDemand(List<OrderExtraDemandDTO> orderExtraDemandDTO);
    bool UpdateOrderExtraDemand(OrderExtraDemandDTO orderExtraDemandDTO);
    OrderExtraDemandInfoDTO GetOrderExtraDemandById(int id);
    bool DeleteOrderExtraDemand(int id);
}
