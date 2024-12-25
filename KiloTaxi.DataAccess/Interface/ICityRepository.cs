using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface ICityRepository
{
    CityDTO AddCity(CityDTO city);
    bool UpdateCity(CityDTO city);
    CityPagingDTO GetAllCity(PageSortParam pageSortParam);
    bool DeleteCity(int id);
    CityDTO GetCity(int id);
    

}