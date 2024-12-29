using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IOrderRouteRepository
{
    OrderRoutePagingDTO GetAllOrderRoute(PageSortParam pageSortParam);
    OrderRouteDTO CreateOrderRoute(OrderRouteDTO orderRouteDTO);
    bool UpdateOrderRoute(OrderRouteDTO orderRouteDTO);
    OrderRouteDTO GetOrderRouteById(int id);
    bool DeleteOrderRoute(int id);
}
