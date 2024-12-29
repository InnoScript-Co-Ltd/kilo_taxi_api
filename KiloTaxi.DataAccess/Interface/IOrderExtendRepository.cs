using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IOrderExtendRepository
{
    OrderExtendPagingDTO GetAllOrderExtend(PageSortParam pageSortParam);
    OrderExtendDTO CreateOrderExtend(OrderExtendDTO orderExtendDTO);
    bool UpdateOrderExtend(OrderExtendDTO orderExtendDTO);
    OrderExtendDTO GetOrderExtendById(int id);
    bool DeleteOrderExtend(int id);
}
