using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IAdminRepository
{
    AdminPagingDTO GetAllAdmin(PageSortParam pageSortParam);

    AdminDTO AddAdmin(AdminDTO adminDTO);
    bool UpdateAdmin(AdminDTO adminDTO);
    AdminDTO GetAdminById(int id);
    bool DeleteAdmin(int id);
    
    Task<AdminDTO> ValidateAdminCredentials(string email, string password);
}
