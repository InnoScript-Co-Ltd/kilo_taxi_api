using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IOrderRepository
{
    OrderPagingDTO GetAllOrders(PageSortParam pageSortParam);

    OrderDTO AddOrder(OrderDTO orderDTO);
    bool UpdateOrder(OrderDTO orderDTO);
    OrderDTO GetOrderById(int id);
    bool DeleteOrder(int id);
}
