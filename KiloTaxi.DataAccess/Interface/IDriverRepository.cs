using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IDriverRepository
{
    ResponseDTO<DriverPagingDTO> GetAllDrivers(PageSortParam pageSortParam);

    DriverInfoDTO DriverRegistration(DriverCreateFormDTO driverCreateFormDto);

    bool UpdateDriver(DriverUpdateFormDTO driverUpdateFormDto);
    DriverInfoDTO GetDriverById(int id);

    bool DeleteDriver(int id);

    Task<DriverInfoDTO> ValidateDriverCredentials(string email, string password);
    List<DriverInfoDTO> SearchNearbyOnlineDriver();

    void UpdateDriverStatus(DriverCreateFormDTO driverCreateFormDto);
}
