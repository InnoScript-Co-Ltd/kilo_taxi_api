using System.Linq.Expressions;
using KiloTaxi.Common.ConfigurationSettings;
using KiloTaxi.Common.Enums;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KiloTaxi.DataAccess.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        private string _mediaHostUrl;
        
        // private IAuthenticationService _authenticationService;
        public CustomerRepository(
            DbKiloTaxiContext dbContext,
            IOptions<MediaSettings> mediaSettings
        )
        {
            _dbKiloTaxiContext = dbContext;
            _mediaHostUrl = mediaSettings.Value.MediaHostUrl;
        }

        public CustomerPagingDTO GetAllCustomer(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.Customers.AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(customer =>
                        customer.Name.Contains(pageSortParam.SearchTerm)
                        || customer.Email.Contains(pageSortParam.SearchTerm)
                        || customer.Phone.Contains(pageSortParam.SearchTerm)
                    );
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(Customer), "customer");
                    var property = Expression.Property(param, pageSortParam.SortField);
                    var sortExpression = Expression.Lambda(property, param);

                    string sortMethod =
                        pageSortParam.SortDir == SortDirection.ASC
                            ? "OrderBy"
                            : "OrderByDescending";
                    var orderByMethod = typeof(Queryable)
                        .GetMethods()
                        .Where(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                        .Single()
                        .MakeGenericMethod(typeof(Customer), property.Type);

                    query =
                        (IQueryable<Customer>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var customers = query
                    .Select(customer =>
                        CustomerConverter.ConvertEntityToModel(customer, _mediaHostUrl)
                    )
                    .ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSortParam.PageSize);
                var pagingResult = new PagingResult
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    PreviousPage =
                        pageSortParam.CurrentPage > 1 ? pageSortParam.CurrentPage - 1 : (int?)null,
                    NextPage =
                        pageSortParam.CurrentPage < totalPages
                            ? pageSortParam.CurrentPage + 1
                            : (int?)null,
                    FirstRowOnPage = ((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize) + 1,
                    LastRowOnPage = Math.Min(
                        totalCount,
                        pageSortParam.CurrentPage * pageSortParam.PageSize
                    ),
                };

                return new CustomerPagingDTO { Paging = pagingResult, Customers = customers };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all customers.");
                throw;
            }
        }

        // public CustomerInfoDTO AddCustomer(CustomerFormDTO customerDTO)
        // {
        //     try
        //     {
        //         Customer customerEntity = new Customer();
        //         customerEntity.Status = CustomerStatus.Pending.ToString();
        //         customerEntity.KycStatus = KycStatus.Pending.ToString();
        //         CustomerConverter.ConvertModelToEntity(customerDTO, ref customerEntity);
        //
        //         _dbKiloTaxiContext.Add(customerEntity);
        //         _dbKiloTaxiContext.SaveChanges();
        //
        //         customerDTO.Id = customerEntity.Id;
        //
        //         var filePaths = new List<(string PropertyName, string FilePath)>
        //         {
        //             (nameof(customerEntity.NrcImageFront), customerEntity.NrcImageFront),
        //             (nameof(customerEntity.NrcImageBack), customerEntity.NrcImageBack),
        //             (nameof(customerEntity.Profile), customerEntity.Profile),
        //         };
        //         foreach (var (propertyName, filePath) in filePaths)
        //         {
        //             if (!filePath.Contains("default.png"))
        //             {
        //                 switch (propertyName)
        //                 {
        //                     case nameof(customerEntity.NrcImageFront):
        //                         customerEntity.NrcImageFront =
        //                             $"customer/{customerDTO.Id}{filePath}";
        //                         break;
        //
        //                     case nameof(customerEntity.NrcImageBack):
        //                         customerEntity.NrcImageBack =
        //                             $"customer/{customerDTO.Id}{filePath}";
        //                         break;
        //
        //                     case nameof(customerEntity.Profile):
        //                         customerEntity.Profile = $"customer/{customerDTO.Id}{filePath}";
        //                         break;
        //
        //                     default:
        //                         break;
        //                 }
        //             }
        //         }
        //
        //         _dbKiloTaxiContext.SaveChanges();
        //
        //        var customerInfoDto = CustomerConverter.ConvertEntityToModel(customerEntity, _mediaHostUrl);
        //
        //         LoggerHelper.Instance.LogInfo(
        //             $"Customer added successfully with Id: {customerEntity.Id}"
        //         );
        //
        //         return customerInfoDto;
        //     }
        //     catch (Exception ex)
        //     {
        //         LoggerHelper.Instance.LogError(ex, "Error occurred while adding customer.");
        //         throw;
        //     }
        // }
         public CustomerInfoDTO AddCustomer(CustomerFormDTO customerDTO)
        {
            try
            {
                Customer customerEntity = new Customer();
                customerDTO.Status = CustomerStatus.Pending;
                customerDTO.KycStatus = KycStatus.Pending;
                customerDTO.Gender=GenderType.Undefined;
                CustomerConverter.ConvertModelToEntity(customerDTO, ref customerEntity);

                _dbKiloTaxiContext.Add(customerEntity);
                _dbKiloTaxiContext.SaveChanges();
               var customerInfoDto = CustomerConverter.ConvertEntityToModel(customerEntity, _mediaHostUrl);

                LoggerHelper.Instance.LogInfo(
                    $"Customer added successfully with Id: {customerEntity.Id}"
                );

                return customerInfoDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding customer.");
                throw;
            }
        }

        public bool UpdateCustomer(CustomerFormDTO customerFormDto)
        {
            try
            {
                var customerEntity = _dbKiloTaxiContext.Customers.FirstOrDefault(customer =>
                    customer.Id == customerFormDto.Id
                );
                if (customerEntity == null)
                {
                    return false;
                }

                // List of image properties to update
                var imageProperties = new List<(
                    string customerDTOProperty,
                    string customerEntityFile
                )>
                {
                    // (nameof(customerDTO.NrcImageFront), customerEntity.NrcImageFront),
                    // (nameof(customerDTO.NrcImageBack), customerEntity.NrcImageBack),
                    (nameof(customerFormDto.Profile), customerEntity.Profile),
                };

                // Loop through image properties and update paths if necessary
                foreach (var (customerDTOProperty, customerEntityFile) in imageProperties)
                {
                    var dtoValue = typeof(CustomerFormDTO)
                        .GetProperty(customerDTOProperty)
                        ?.GetValue(customerFormDto)
                        ?.ToString();
                    if (string.IsNullOrEmpty(dtoValue))
                    {
                        typeof(CustomerFormDTO)
                            .GetProperty(customerDTOProperty)
                            ?.SetValue(customerFormDto, customerEntityFile);
                    }
                    else if (dtoValue != customerEntityFile)
                    {
                        typeof(CustomerFormDTO)
                            .GetProperty(customerDTOProperty)
                            ?.SetValue(customerFormDto, $"customer/{customerFormDto.Id}{dtoValue}");
                    }
                }

                CustomerConverter.ConvertModelToEntity(customerFormDto, ref customerEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating customer with Id: {customerFormDto.Id}"
                );
                throw;
            }
        }

        public CustomerInfoDTO GetCustomerById(int id)
        {
            try
            {
                var customerEntity = _dbKiloTaxiContext.Customers.FirstOrDefault(customer =>
                    customer.Id == id
                );
                if (customerEntity == null)
                {
                    LoggerHelper.Instance.LogError($"Customer with Id: {id} not found.");
                    return null;
                }
                var customerDTO = CustomerConverter.ConvertEntityToModel(
                    customerEntity,
                    _mediaHostUrl
                );

                return customerDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching customer with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteCustomer(int id)
        {
            try
            {
                var customerEntity = _dbKiloTaxiContext.Customers.FirstOrDefault(customer =>
                    customer.Id == id
                );
                if (customerEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.Customers.Remove(customerEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting customer with Id: {id}"
                );
                throw;
            }
        }
        public async Task<CustomerInfoDTO> ValidateCustomerCredentials(string EmailOrPhone,string password)
        {

            if (string.IsNullOrEmpty(EmailOrPhone))
            {
                return null; // Or throw an exception depending on your use case
            }

            Customer customerEntity =  _dbKiloTaxiContext.Customers.SingleOrDefault(customer => customer.Phone == EmailOrPhone);
            if (customerEntity != null || ! BCrypt.Net.BCrypt.Verify(password, customerEntity.Password))
            {
                return CustomerConverter.ConvertEntityToModel(customerEntity, _mediaHostUrl);
            }

         
            return null;
        }

        public ResponseDTO<OtpInfo> FindCustomerAndGenerateOtp(CustomerFormDTO customerFormDto)
        {
            var existedCustomer=_dbKiloTaxiContext.Customers.FirstOrDefault(customer => customer.Phone == customerFormDto.Phone);
            OtpInfo otpInfo=new OtpInfo();
            if (existedCustomer != null && existedCustomer.Status == CustomerStatus.Active.ToString())
            {
                
                ResponseDTO<OtpInfo> responseDto = new ResponseDTO<OtpInfo>();
                responseDto.StatusCode=209;        
                responseDto.Message="User Already exist with this phone number "+existedCustomer.Phone;
                otpInfo.UserStatus = existedCustomer.Status;
                responseDto.Payload = otpInfo;
                return responseDto;
            }else if (existedCustomer != null && existedCustomer.Status == CustomerStatus.Pending.ToString())
            {
                ResponseDTO<OtpInfo> responseDto = new ResponseDTO<OtpInfo>();
                responseDto.StatusCode=209;        
                responseDto.Message="User Already exist with this phone number "+existedCustomer.Phone +" but Account status is Pending";
                otpInfo.UserStatus = existedCustomer.Status;
                otpInfo.Email = customerFormDto.Email;
                otpInfo.Password = BCrypt.Net.BCrypt.HashPassword(customerFormDto.Password);
                otpInfo.Role = customerFormDto.Role;
                otpInfo.Phone = customerFormDto.Phone;
                otpInfo.Otp = GenerateOTP();
                otpInfo.OtpExpired = DateTime.Now.AddMinutes(3);
                otpInfo.UserName = customerFormDto.Name;
                otpInfo.RetryCount = 0;
                responseDto.Payload = otpInfo;
                responseDto.TimeStamp =DateTime.Now;
                return responseDto;
            }
            otpInfo.Email = customerFormDto.Email;
            otpInfo.Password = BCrypt.Net.BCrypt.HashPassword(customerFormDto.Password);
            otpInfo.Role = customerFormDto.Role;
            otpInfo.Phone = customerFormDto.Phone;
            otpInfo.Otp = GenerateOTP();
            otpInfo.OtpExpired = DateTime.Now.AddMinutes(3);
            otpInfo.UserName = customerFormDto.Name;
            otpInfo.RetryCount = 0;
            return new ResponseDTO<OtpInfo>
            {
                StatusCode = 200,
                Message = "OTP generated successfully.",
                Payload = otpInfo,
                TimeStamp = DateTime.Now
            };
        }
        public string GenerateOTP()
        {
            Random random = new Random();
            int otp = random.Next(100000, 1000000); // Generates a number between 100000 and 999999
            return otp.ToString();
        }
            
    }
}
