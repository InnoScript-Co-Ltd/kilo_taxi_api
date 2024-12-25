using System.Linq.Expressions;
using KiloTaxi.Common.ConfigurationSettings;
using KiloTaxi.Common.Enums;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.Extensions.Options;

namespace KiloTaxi.DataAccess.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        private string _mediaHostUrl;

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

        public CustomerDTO AddCustomer(CustomerDTO customerDTO)
        {
            try
            {
                Customer customerEntity = new Customer();
                
                // customerEntity.Status = CustomerStatus.Pending.ToString();
                // customerEntity.KycStatus = KycStatus.Pending.ToString();
                CustomerConverter.ConvertModelToEntity(customerDTO, ref customerEntity);

                _dbKiloTaxiContext.Add(customerEntity);
                _dbKiloTaxiContext.SaveChanges();

                customerDTO.Id = customerEntity.Id;

                var filePaths = new List<(string PropertyName, string FilePath)>
                {
                    (nameof(customerEntity.NrcImageFront), customerEntity.NrcImageFront),
                    (nameof(customerEntity.NrcImageBack), customerEntity.NrcImageBack),
                    (nameof(customerEntity.Profile), customerEntity.Profile),
                };
                foreach (var (propertyName, filePath) in filePaths)
                {
                    if (!filePath.Contains("default.png"))
                    {
                        switch (propertyName)
                        {
                            case nameof(customerEntity.NrcImageFront):
                                customerEntity.NrcImageFront =
                                    $"customer/{customerDTO.Id}{filePath}";
                                break;

                            case nameof(customerEntity.NrcImageBack):
                                customerEntity.NrcImageBack =
                                    $"customer/{customerDTO.Id}{filePath}";
                                break;

                            case nameof(customerEntity.Profile):
                                customerEntity.Profile = $"customer/{customerDTO.Id}{filePath}";
                                break;

                            default:
                                break;
                        }
                    }
                }

                _dbKiloTaxiContext.SaveChanges();

                customerDTO = CustomerConverter.ConvertEntityToModel(customerEntity, _mediaHostUrl);

                LoggerHelper.Instance.LogInfo(
                    $"Customer added successfully with Id: {customerEntity.Id}"
                );

                return customerDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding customer.");
                throw;
            }
        }

        public bool UpdateCustomer(CustomerDTO customerDTO)
        {
            try
            {
                var customerEntity = _dbKiloTaxiContext.Customers.FirstOrDefault(customer =>
                    customer.Id == customerDTO.Id
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
                    (nameof(customerDTO.NrcImageFront), customerEntity.NrcImageFront),
                    (nameof(customerDTO.NrcImageBack), customerEntity.NrcImageBack),
                    (nameof(customerDTO.Profile), customerEntity.Profile),
                };

                // Loop through image properties and update paths if necessary
                foreach (var (customerDTOProperty, customerEntityFile) in imageProperties)
                {
                    var dtoValue = typeof(CustomerDTO)
                        .GetProperty(customerDTOProperty)
                        ?.GetValue(customerDTO)
                        ?.ToString();
                    if (string.IsNullOrEmpty(dtoValue))
                    {
                        typeof(CustomerDTO)
                            .GetProperty(customerDTOProperty)
                            ?.SetValue(customerDTO, customerEntityFile);
                    }
                    else if (dtoValue != customerEntityFile)
                    {
                        typeof(CustomerDTO)
                            .GetProperty(customerDTOProperty)
                            ?.SetValue(customerDTO, $"customer/{customerDTO.Id}{dtoValue}");
                    }
                }

                CustomerConverter.ConvertModelToEntity(customerDTO, ref customerEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating customer with Id: {customerDTO.Id}"
                );
                throw;
            }
        }

        public CustomerDTO GetCustomerById(int id)
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
        public async Task<CustomerDTO> ValidateCustomerCredentials(string email, string password)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null; // Or throw an exception depending on your use case
            }

            Customer customerEntity =  _dbKiloTaxiContext.Customers.SingleOrDefault(customer => customer.Email == email);
            if (customerEntity != null || ! BCrypt.Net.BCrypt.Verify(password, customerEntity.Password))
            {
                return CustomerConverter.ConvertEntityToModel(customerEntity, _mediaHostUrl);
            }

            // Convert the entity to a DTO
            return null;
        }
        
    }
}
