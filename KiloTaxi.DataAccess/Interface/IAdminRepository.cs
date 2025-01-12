using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IAdminRepository
{
    ResponseDTO<AdminPagingDTO> GetAllAdmin(PageSortParam pageSortParam);

    AdminDTO AddAdmin(AdminDTO adminDTO);
    bool UpdateAdmin(AdminDTO adminDTO);
    AdminDTO GetAdminById(int id);
    bool DeleteAdmin(int id);

    Task<AdminDTO> ValidateAdminCredentials(string email, string password);
}
