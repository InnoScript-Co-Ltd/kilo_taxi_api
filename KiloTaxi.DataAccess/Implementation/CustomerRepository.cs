using System.Linq.Expressions;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public CustomerRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
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

                var customers = query.Select(CustomerConverter.ConvertEntityToModel).ToList();

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
                CustomerConverter.ConvertModelToEntity(customerDTO, ref customerEntity);

                _dbKiloTaxiContext.Add(customerEntity);
                _dbKiloTaxiContext.SaveChanges();

                customerDTO.Id = customerEntity.Id;

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

                return CustomerConverter.ConvertEntityToModel(customerEntity);
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
    }
}
