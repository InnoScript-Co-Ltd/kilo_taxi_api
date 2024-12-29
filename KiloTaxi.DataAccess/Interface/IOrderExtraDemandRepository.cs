using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IOrderExtraDemandRepository
{
    OrderExtraDemandPagingDTO GetAllOrderExtraDemand(PageSortParam pageSortParam);
    OrderExtraDemandDTO CreateOrderExtraDemand(OrderExtraDemandDTO orderExtraDemandDTO);
    bool UpdateOrderExtraDemand(OrderExtraDemandDTO orderExtraDemandDTO);
    OrderExtraDemandDTO GetOrderExtraDemandById(int id);
    bool DeleteOrderExtraDemand(int id);
}
