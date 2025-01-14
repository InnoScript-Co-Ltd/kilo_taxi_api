using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IOrderRepository
{
    ResponseDTO<OrderPagingDTO> GetAllOrder(PageSortParam pageSortParam);

    OrderInfoDTO AddOrder(OrderFormDTO orderFormDTO);
    bool UpdateOrder(OrderFormDTO orderFormDTO);
    OrderInfoDTO GetOrderById(int id);
    bool DeleteOrder(int id);
}
