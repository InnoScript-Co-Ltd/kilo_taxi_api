using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.DataAccess.Interface;

public interface ICustomerRepository
{
    ResponseDTO<CustomerPagingDTO> GetAllCustomer(PageSortParam pageSortParam);

    CustomerInfoDTO AddCustomer(CustomerFormDTO customerFormDTO);
    bool UpdateCustomer(CustomerFormDTO customerFormDTO);
    CustomerInfoDTO GetCustomerById(int id);
    bool DeleteCustomer(int id);
    
    Task<CustomerInfoDTO> ValidateCustomerCredentials(string emailOrPhone,string password);
    
    Task<ResponseDTO<OtpInfo>> FindCustomerAndGenerateOtp(CustomerFormDTO pageSortParam);

}
