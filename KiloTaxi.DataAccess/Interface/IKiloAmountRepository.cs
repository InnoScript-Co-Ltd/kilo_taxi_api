using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IKiloAmountRepository
{
    KiloAmountPagingDTO GetAllKiloAmountList(PageSortParam pageSortParam);

    KiloAmountDTO CreateKiloAmount(KiloAmountDTO kiloAmountDTO);

    bool UpdateKiloAmount(KiloAmountDTO kiloAmountDTO);

    KiloAmountDTO GetKiloAmountById(int id);

    bool DeleteKiloAmount(int id);
}
