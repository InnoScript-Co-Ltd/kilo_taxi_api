using System.Linq.Expressions;
using System.Net;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.DataAccess.Implementation
{
    public class SmsRepository : ISmsRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public SmsRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public ResponseDTO<SmsPagingDTO> GetAllSms(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext
                    .Sms.Include(s => s.Admin)
                    .Include(s => s.Customer)
                    .Include(s => s.Driver)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(sms =>
                        sms.Title.Contains(pageSortParam.SearchTerm)
                        || sms.Name.Contains(pageSortParam.SearchTerm)
                        || sms.Message.Contains(pageSortParam.SearchTerm)
                    );
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(Sms), "sms");
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
                        .MakeGenericMethod(typeof(Sms), property.Type);

                    query =
                        (IQueryable<Sms>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var sms = query.Select(SmsConverter.ConvertEntityToModel).ToList();

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

                ResponseDTO<SmsPagingDTO> responseDto = new ResponseDTO<SmsPagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "sms retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new SmsPagingDTO { Paging = pagingResult, Sms = sms };
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all sms.");
                throw;
            }
        }

        public SmsInfoDTO CreateSms(SmsFormDTO smsFormDTO)
        {
            try
            {
                Sms smsEntity = new Sms();
                SmsConverter.ConvertModelToEntity(smsFormDTO, ref smsEntity);

                var admin = _dbKiloTaxiContext.Admins.FirstOrDefault(c => c.Id == smsFormDTO.AdminId);
                var customer = _dbKiloTaxiContext.Customers.FirstOrDefault(c =>
                    c.Id == smsFormDTO.CustomerId
                );
                var driver = _dbKiloTaxiContext.Drivers.FirstOrDefault(s =>
                    s.Id == smsFormDTO.DriverId
                );

                _dbKiloTaxiContext.Add(smsEntity);
                _dbKiloTaxiContext.SaveChanges();

                smsFormDTO.Id = smsEntity.Id;
                smsFormDTO.AdminName = admin.Name;
                smsFormDTO.CustomerName = customer.Name;
                smsFormDTO.DriverName = driver.Name;

                LoggerHelper.Instance.LogInfo($"Sms added successfully with Id: {smsEntity.Id}");

                var smsInfoDTO = SmsConverter.ConvertEntityToModel(smsEntity);

                return smsInfoDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding sms.");
                throw;
            }
        }

        public bool UpdateSms(SmsFormDTO smsFormDTO)
        {
            try
            {
                var smsEntity = _dbKiloTaxiContext.Sms.FirstOrDefault(s => s.Id == smsFormDTO.Id);

                if (smsEntity == null)
                {
                    return false;
                }

                SmsConverter.ConvertModelToEntity(smsFormDTO, ref smsEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating sms with Id: {smsFormDTO.Id}"
                );
                throw;
            }
        }

        public SmsInfoDTO GetSmsById(int id)
        {
            try
            {
                var smsEntity = _dbKiloTaxiContext
                    .Sms.Include(r => r.Admin)
                    .Include(r => r.Customer)
                    .Include(r => r.Driver)
                    .FirstOrDefault(sms => sms.Id == id);

                if (smsEntity == null)
                {
                    LoggerHelper.Instance.LogError($"Sms with Id: {id} not found.");
                    return null;
                }

                return SmsConverter.ConvertEntityToModel(smsEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching sms with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteSms(int id)
        {
            try
            {
                var smsEntity = _dbKiloTaxiContext.Sms.FirstOrDefault(s => s.Id == id);
                if (smsEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.Sms.Remove(smsEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting sms with Id: {id}"
                );
                throw;
            }
        }
    }
}
