using System.Linq.Expressions;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.DataAccess.Implementation
{
    public class ScheduleBookingRepository : IScheduleBookingRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public ScheduleBookingRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public ScheduleBookingPagingDTO GetAllScheduleBooking(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext
                    .ScheduleBookings.Include(r => r.Customer)
                    .Include(r => r.Driver)
                    .AsQueryable();

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(ScheduleBooking), "scheduleBooking");
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
                        .MakeGenericMethod(typeof(ScheduleBooking), property.Type);

                    query =
                        (IQueryable<ScheduleBooking>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var scheduleBookings = query
                    .Select(ScheduleBookingConverter.ConvertEntityToModel)
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

                return new ScheduleBookingPagingDTO()
                {
                    Paging = pagingResult,
                    ScheduleBookings = scheduleBookings,
                };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error occurred while fetching all schedule bookings."
                );
                throw;
            }
        }

        public ScheduleBookingDTO AddScheduleBooking(ScheduleBookingDTO scheduleBookingDTO)
        {
            try
            {
                ScheduleBooking scheduleBookingEntity = new ScheduleBooking();
                ScheduleBookingConverter.ConvertModelToEntity(
                    scheduleBookingDTO,
                    ref scheduleBookingEntity
                );

                _dbKiloTaxiContext.Add(scheduleBookingEntity);
                _dbKiloTaxiContext.SaveChanges();

                LoggerHelper.Instance.LogInfo($"Schedule Booking added successfully");

                return scheduleBookingDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding scheule booking.");
                throw;
            }
        }

        public bool UpdateScheduleBooking(ScheduleBookingDTO scheduleBookingDTO)
        {
            try
            {
                var scheduleBookingEntity = _dbKiloTaxiContext.ScheduleBookings.FirstOrDefault(
                    ScheduleBooking => ScheduleBooking.Id == scheduleBookingDTO.Id
                );
                if (scheduleBookingEntity == null)
                {
                    return false;
                }

                ScheduleBookingConverter.ConvertModelToEntity(
                    scheduleBookingDTO,
                    ref scheduleBookingEntity
                );
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating schedule booking with Id: {scheduleBookingDTO.Id}"
                );
                throw;
            }
        }

        public ScheduleBookingDTO getScheduleBookingById(int id)
        {
            try
            {
                // Fetch the ScheduleBooking entity including related entities
                var scheduleBookingEntity = _dbKiloTaxiContext
                    .ScheduleBookings.Include(r => r.Customer)
                    .Include(r => r.Driver)
                    .FirstOrDefault(scheduleBooking => scheduleBooking.Id == id);

                if (scheduleBookingEntity == null)
                {
                    LoggerHelper.Instance.LogError($"Schedule Booking with Id: {id} not found.");
                    return null;
                }

                // Convert ScheduleBooking entity to DTO
                var scheduleBookingDTO = ScheduleBookingConverter.ConvertEntityToModel(
                    scheduleBookingEntity
                );

                // Fetch and populate Orders associated with the ScheduleBooking
                // var orderInfoDTOs = _dbKiloTaxiContext
                //     .Orders.Where(o => o.ScheduleBookingId == scheduleBookingDTO.Id)
                //     .Select(order => OrderConverter.ConvertEntityToModel(order))
                //     .ToList();

                // // Assign associated Orders to the ScheduleBookingDTO
                // scheduleBookingDTO.Orders = orderInfoDTOs;

                return scheduleBookingDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching Schedule Booking with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteScheduleBooking(int id)
        {
            try
            {
                var scheduleBookingEntity = _dbKiloTaxiContext.ScheduleBookings.FirstOrDefault(
                    ScheduleBooking => ScheduleBooking.Id == id
                );
                if (scheduleBookingEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.ScheduleBookings.Remove(scheduleBookingEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting schedule booking with Id: {id}"
                );
                throw;
            }
        }
    }
}
