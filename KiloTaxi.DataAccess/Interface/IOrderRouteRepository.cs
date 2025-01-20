using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IOrderRouteRepository
{
    ResponseDTO<OrderRoutePagingDTO> GetAllOrderRoute(PageSortParam pageSortParam);
    OrderRouteInfoDTO CreateOrderRoute(OrderRouteFormDTO orderRouteFormDTO);
    bool UpdateOrderRoute(OrderRouteFormDTO orderRouteFormDTO);
    OrderRouteInfoDTO GetOrderRouteById(int id);
    bool DeleteOrderRoute(int id);
}
