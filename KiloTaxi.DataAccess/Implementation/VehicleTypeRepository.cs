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
    public class VehicleTypeRepository : IVehicleTypeRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public VehicleTypeRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public ResponseDTO<VehicleTypePagingDTO> GetAllVehicleTypes(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.VehicleTypes.AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(vt =>
                        vt.Name.Contains(pageSortParam.SearchTerm)
                        || vt.Description.Contains(pageSortParam.SearchTerm)
                    );
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(VehicleType), "vehicleType");
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
                        .MakeGenericMethod(typeof(VehicleType), property.Type);

                    query =
                        (IQueryable<VehicleType>)(
                            orderByMethod.Invoke(null, new object[] { query, sortExpression })
                            ?? Enumerable.Empty<VehicleType>().AsQueryable()
                        );
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var vehicleType = query.Select(VehicleTypeConverter.ConvertEntityToModel).ToList();

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
                
                ResponseDTO<VehicleTypePagingDTO> responseDto = new ResponseDTO<VehicleTypePagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "vehicle types retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new VehicleTypePagingDTO { Paging = pagingResult, VehicleTypes = vehicleType };
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error occurred while fetching all vehicleType."
                );
                throw;
            }
        }

        public VehicleTypeInfoDTO AddVehicleType(VehicleTypeFormDTO vehicleTypeFormDTO)
        {
            try
            {
                VehicleType vehicleTypeEntity = new VehicleType();
                VehicleTypeConverter.ConvertModelToEntity(vehicleTypeFormDTO, ref vehicleTypeEntity);

                _dbKiloTaxiContext.Add(vehicleTypeEntity);
                _dbKiloTaxiContext.SaveChanges();

                vehicleTypeFormDTO.Id = vehicleTypeEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"VehicleType added successfully with Id: {vehicleTypeEntity.Id}"
                );

                var vehicleTypeInfoDTO = VehicleTypeConverter.ConvertEntityToModel(vehicleTypeEntity);
                return vehicleTypeInfoDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding vehicleType.");
                throw;
            }
        }

        public bool UpdateVehicleType(VehicleTypeFormDTO vehicleTypeFormDTO)
        {
            try
            {
                var vehicleTypeEntity = _dbKiloTaxiContext.VehicleTypes.FirstOrDefault(s =>
                    s.Id == vehicleTypeFormDTO.Id
                );

                if (vehicleTypeEntity == null)
                {
                    return false;
                }

                VehicleTypeConverter.ConvertModelToEntity(vehicleTypeFormDTO, ref vehicleTypeEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating vehicleType with Id: {vehicleTypeFormDTO.Id}"
                );
                throw;
            }
        }

        public VehicleTypeInfoDTO GetVehicleTypeById(int id)
        {
            try
            {
                var vehicleTypeEntity = _dbKiloTaxiContext.VehicleTypes.FirstOrDefault(
                    vehicleType => vehicleType.Id == id
                );

                if (vehicleTypeEntity == null)
                {
                    LoggerHelper.Instance.LogError($"VehicleType with Id: {id} not found.");
                    return null;
                }

                return VehicleTypeConverter.ConvertEntityToModel(vehicleTypeEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching vehicleType with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteVehicleType(int id)
        {
            try
            {
                var vehicleTypeEntity = _dbKiloTaxiContext.VehicleTypes.FirstOrDefault(s =>
                    s.Id == id
                );
                if (vehicleTypeEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.VehicleTypes.Remove(vehicleTypeEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting vehicleType with Id: {id}"
                );
                throw;
            }
        }
    }
}
