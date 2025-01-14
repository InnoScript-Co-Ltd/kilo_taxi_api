using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IAdminRepository
{
    ResponseDTO<AdminPagingDTO> GetAllAdmin(PageSortParam pageSortParam);

    AdminInfoDTO AdminRegistration(AdminFormDTO adminDTO);
    bool UpdateAdmin(AdminFormDTO adminDTO);
    AdminInfoDTO GetAdminById(int id);
    bool DeleteAdmin(int id);

    Task<AdminInfoDTO> ValidateAdminCredentials(string email, string password);
}
