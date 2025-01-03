using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IOrderExtraDemandRepository
{
    OrderExtraDemandPagingDTO GetAllOrderExtraDemand(PageSortParam pageSortParam);
    List<OrderExtraDemandDTO> CreateOrderExtraDemand(List<OrderExtraDemandDTO> orderExtraDemandDTO);
    bool UpdateOrderExtraDemand(OrderExtraDemandDTO orderExtraDemandDTO);
    OrderExtraDemandDTO GetOrderExtraDemandById(int id);
    bool DeleteOrderExtraDemand(int id);
}
