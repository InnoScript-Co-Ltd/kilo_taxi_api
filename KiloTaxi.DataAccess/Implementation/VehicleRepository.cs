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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KiloTaxi.DataAccess.Implementation;

public class VehicleRepository : IVehicleRepository
{
    private readonly DbKiloTaxiContext _dbKiloTaxiContext;
    private string _mediaHostUrl;

    public VehicleRepository(
        DbKiloTaxiContext dbKiloTaxiContext,
        IOptions<MediaSettings> mediaSettings
    )
    {
        _dbKiloTaxiContext = dbKiloTaxiContext;
        _mediaHostUrl = mediaSettings.Value.MediaHostUrl;
    }

    public VehicleInfoDTO VehicleRegistration(DriverFormDTO vehicleDTO)
    {
        try
        {
            Vehicle vehicleEntity = new Vehicle();
            VehicleConverter.ConvertModelToEntity(vehicleDTO, ref vehicleEntity);
            var driver = _dbKiloTaxiContext.Drivers.FirstOrDefault(driver => driver.Id == vehicleDTO.DriverId);

            _dbKiloTaxiContext.Add(vehicleEntity);
            _dbKiloTaxiContext.SaveChanges();
            vehicleDTO.Id = vehicleEntity.Id;
            var filePaths = new List<(string PropertyName, string FilePath)>
            {
                (nameof(vehicleEntity.BusinessLicenseImage), vehicleEntity.BusinessLicenseImage),
                (nameof(vehicleEntity.VehicleLicenseFront), vehicleEntity.VehicleLicenseFront),
                (nameof(vehicleEntity.VehicleLicenseBack), vehicleEntity.VehicleLicenseBack),
            };

            foreach (var (propertyName, filePath) in filePaths)
            {
                if (!filePath.Contains("default.png"))
                {
                    if (propertyName == nameof(vehicleEntity.BusinessLicenseImage))
                        vehicleEntity.BusinessLicenseImage = $"vehicle/{vehicleDTO.Id}{filePath}";
                    else if (propertyName == nameof(vehicleEntity.VehicleLicenseFront))
                        vehicleEntity.VehicleLicenseFront = $"vehicle/{vehicleDTO.Id}{filePath}";
                    else if (propertyName == nameof(vehicleEntity.VehicleLicenseBack))
                        vehicleEntity.VehicleLicenseBack = $"vehicle/{vehicleDTO.Id}{filePath}";
                }
            }
            _dbKiloTaxiContext.SaveChanges();

            var vehicleInfoDTO = VehicleConverter.ConvertEntityToModel(vehicleEntity, _mediaHostUrl);
            return vehicleInfoDTO;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occured while registering vehicle.");
            throw;
        }
    }

    public bool UpdateVehicle(DriverFormDTO vehicleDTO)
    {
        bool result = false;
        try
        {
            var vehicleEntity = _dbKiloTaxiContext.Vehicles.FirstOrDefault(v =>
                v.Id == vehicleDTO.Id
            );
            if (vehicleEntity == null)
            {
                return result;
            }

            // List of image properties to update
            var imageProperties = new List<(string vehicleDTOProperty, string vehicleEntityFile)>
            {
                (nameof(vehicleDTO.BusinessLicenseImage), vehicleEntity.BusinessLicenseImage),
                (nameof(vehicleDTO.VehicleLicenseFront), vehicleEntity.VehicleLicenseFront),
                (nameof(vehicleDTO.VehicleLicenseBack), vehicleEntity.VehicleLicenseBack),
            };

            // Loop through image properties and update paths if necessary
            foreach (var (vehicleDTOProperty, vehicleEntityFile) in imageProperties)
            {
                var dtoValue = typeof(VehicleDTO)
                    .GetProperty(vehicleDTOProperty)
                    ?.GetValue(vehicleDTO)
                    ?.ToString();
                if (string.IsNullOrEmpty(dtoValue))
                {
                    typeof(VehicleDTO)
                        .GetProperty(vehicleDTOProperty)
                        ?.SetValue(vehicleDTO, vehicleEntityFile);
                }
                else if (dtoValue != vehicleEntityFile)
                {
                    typeof(VehicleDTO)
                        .GetProperty(vehicleDTOProperty)
                        ?.SetValue(vehicleDTO, $"vehicle/{vehicleDTO.Id}{dtoValue}");
                }
            }

            VehicleConverter.ConvertModelToEntity(vehicleDTO, ref vehicleEntity);
            _dbKiloTaxiContext.SaveChanges();
            result = true;
            return result;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                $"Error occurred while updating vehicle with Id: {vehicleDTO.Id}"
            );
            throw;
        }
    }

    public VehicleInfoDTO GetVehicleById(int id)
    {
        try
        {
            return VehicleConverter.ConvertEntityToModel(
                _dbKiloTaxiContext.Vehicles.Include(x => x.Driver).FirstOrDefault(x => x.Id == id),
                _mediaHostUrl
            );
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occured while getting vehicle by id: {id}");
            throw;
        }
    }

    public VehiclePagingDTO GetAllVehicle(PageSortParam pageSortParam)
    {
        try
        {
            var query = _dbKiloTaxiContext.Vehicles.Include(v => v.Driver).AsQueryable();
            if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
            {
                query = query.Where(p =>
                    p.VehicleNo.Contains(pageSortParam.SearchTerm)
                    || p.Model.Contains(pageSortParam.SearchTerm)
                );
            }
            int totalCount = query.Count();
            if (!string.IsNullOrEmpty(pageSortParam.SortField))
            {
                var param = Expression.Parameter(typeof(Vehicle), "p");
                var property = Expression.Property(param, pageSortParam.SortField);
                var sortExpression = Expression.Lambda(property, param);

                string sortMethod =
                    pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                var orderByMethod = typeof(Queryable)
                    .GetMethods()
                    .Single(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(Vehicle), property.Type);
                query =
                    (IQueryable<Vehicle>)(
                        orderByMethod.Invoke(
                            _dbKiloTaxiContext,
                            new object[] { query, sortExpression }
                        ) ?? Enumerable.Empty<Vehicle>().AsQueryable()
                    );
            }

            if (query.Count() > pageSortParam.PageSize)
            {
                query = query
                    .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                    .Take(pageSortParam.PageSize);
            }
            var vehicles = query
                .Select(vehicle => VehicleConverter.ConvertEntityToModel(vehicle, _mediaHostUrl))
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
            return new VehiclePagingDTO() { Paging = pagingResult, Vehicles = vehicles };
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all vehicles.");
            throw;
        }
    }

    public bool DeleteVehicle(int id)
    {
        bool result = false;
        try
        {
            var vehicleEntity = _dbKiloTaxiContext.Vehicles.FirstOrDefault(x => x.Id == id);
            if (vehicleEntity == null)
            {
                return result;
            }
            _dbKiloTaxiContext.Vehicles.Remove(vehicleEntity);
            _dbKiloTaxiContext.SaveChanges();
            result = true;
            return result;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                $"Error occurred while delete vehicle with id: {id}"
            );
            throw;
        }
    }
}
