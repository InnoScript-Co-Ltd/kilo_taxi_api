using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface ICustomerRepository
{
    CustomerPagingDTO GetAllCustomer(PageSortParam pageSortParam);

    CustomerDTO AddCustomer(CustomerDTO customerDTO);
    bool UpdateCustomer(CustomerDTO customerDTO);
    CustomerDTO GetCustomerById(int id);
    bool DeleteCustomer(int id);
    
    Task<CustomerDTO> ValidateCustomerCredentials(string email, string password);

}
