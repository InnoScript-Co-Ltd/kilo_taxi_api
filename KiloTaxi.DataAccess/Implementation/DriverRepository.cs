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
using Microsoft.Extensions.Options;

namespace KiloTaxi.DataAccess.Implementation;

public class DriverRepository : IDriverRepository
{
    private readonly DbKiloTaxiContext _dbKiloTaxiContext;
    private string _mediaHostUrl;

    public DriverRepository(
        DbKiloTaxiContext dbKiloTaxiContext,
        IOptions<MediaSettings> mediaSettings
    )
    {
        _dbKiloTaxiContext = dbKiloTaxiContext;
        _mediaHostUrl = mediaSettings.Value.MediaHostUrl;
    }

    public DriverPagingDTO GetAllDrivers(PageSortParam pageSortParam)
    {
        try
        {
            var query = _dbKiloTaxiContext.Drivers.AsQueryable();
            if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(pageSortParam.SearchTerm)
                    || p.Email.Contains(pageSortParam.SearchTerm)
                );
            }

            int totalCount = query.Count();
            if (!string.IsNullOrEmpty(pageSortParam.SortField))
            {
                var param = Expression.Parameter(typeof(Driver), "p");
                var property = Expression.Property(param, pageSortParam.SortField);
                var sortExpression = Expression.Lambda(property, param);

                string sortMethod =
                    pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                var orderByMethod = typeof(Queryable)
                    .GetMethods()
                    .Single(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(Driver), property.Type);
                query =
                    (IQueryable<Driver>)(
                        orderByMethod.Invoke(
                            _dbKiloTaxiContext,
                            new object[] { query, sortExpression }
                        ) ?? Enumerable.Empty<Driver>().AsQueryable()
                    );
            }

            if (query.Count() > pageSortParam.PageSize)
            {
                query = query
                    .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                    .Take(pageSortParam.PageSize);
            }

            var drivers = query
                .Select(driver => DriverConverter.ConvertEntityToModel(driver, _mediaHostUrl))
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
            return new DriverPagingDTO() { Paging = pagingResult, Drivers = drivers };
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all drivers.");
            throw;
        }
    }

    public DriverInfoDTO GetDriverById(int id)
    {
        try
        {
            var driverDTO = DriverConverter.ConvertEntityToModel(
                _dbKiloTaxiContext.Drivers.FirstOrDefault(x => x.Id == id),
                _mediaHostUrl
            );
            var vehicleDTO = _dbKiloTaxiContext
                .Vehicles.Where(v => v.DriverId == driverDTO.Id)
                .Select(vehicle => VehicleConverter.ConvertEntityToModel(vehicle, _mediaHostUrl))
                .ToList();
            var WalletUserMappingDTO = _dbKiloTaxiContext
                .WalletUserMappings.Where(w =>
                    w.UserId == driverDTO.Id && w.UserType == UserType.Driver.ToString()
                )
                .Select(walletUserMapping =>
                    WalletUserMappingConverter.ConvertEntityToModel(walletUserMapping)
                )
                .ToList();
                 //driverDTO.Vehicle = vehicleDTO;
                //driverDTO.WalletUserMapping = WalletUserMappingDTO;
                driverDTO.VehicleInfo = vehicleDTO;
                
            return driverDTO;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occured while getting driver by id: {id}");
            throw;
        }
    }

    public DriverInfoDTO DriverRegistration(DriverCreateFormDTO driverCreateDto)
    {
        try
        {
            Driver driverEntity = new Driver();
            DateTime createdDate = DateTime.Now;
            driverCreateDto.CreatedDate = createdDate;
            driverCreateDto.Password = BCrypt.Net.BCrypt.HashPassword(driverCreateDto.Password);
            DriverConverter.ConvertModelToEntity(driverCreateDto, ref driverEntity);

            _dbKiloTaxiContext.Add(driverEntity);
            _dbKiloTaxiContext.SaveChanges();
            driverCreateDto.Id = driverEntity.Id;
            var filePaths = new List<(string PropertyName, string FilePath)>
            {
                (nameof(driverEntity.DriverImageLicenseFront), driverEntity.DriverImageLicenseFront),
                (nameof(driverEntity.DriverImageLicenseBack), driverEntity.DriverImageLicenseBack),
                (nameof(driverEntity.Profile), driverEntity.Profile),
            };
            foreach (var (propertyName, filePath) in filePaths)
            {
                if (!filePath.Contains("default.png"))
                {
                     if (propertyName == nameof(driverEntity.DriverImageLicenseFront))
                        driverEntity.DriverImageLicenseFront = $"driver/{driverCreateDto.Id}{filePath}";
                    else if (propertyName == nameof(driverEntity.DriverImageLicenseBack))
                        driverEntity.DriverImageLicenseBack = $"driver/{driverCreateDto.Id}{filePath}";
                    else if (propertyName == nameof(driverEntity.Profile))
                        driverEntity.Profile = $"driver/{driverCreateDto.Id}{filePath}";
                }
            }

            _dbKiloTaxiContext.SaveChanges();

           var driverInfoDTO = DriverConverter.ConvertEntityToModel(driverEntity, _mediaHostUrl);
            return driverInfoDTO;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occured while registering driver.");
            throw;
        }
    }

    public bool UpdateDriver(DriverUpdateFormDTO driverUpdateFormDto)
    {
        bool result = false;
        try
        {
            var driverEntity = _dbKiloTaxiContext.Drivers.FirstOrDefault(d => d.Id == driverUpdateFormDto.Id);
            if (driverEntity == null)
            {
                return result;
            }
            // List of image properties to update
            var imageProperties = new List<(string driverDTOProperty, string driverEntityFile)>
            {
                // (nameof(driverDTO.NrcImageFront), driverEntity.NrcImageFront),
                // (nameof(driverDTO.NrcImageBack), driverEntity.NrcImageBack),
                (nameof(driverUpdateFormDto.DriverImageLicenseFront), driverEntity.DriverImageLicenseFront),
                (nameof(driverUpdateFormDto.DriverImageLicenseBack), driverEntity.DriverImageLicenseBack),
                (nameof(driverUpdateFormDto.Profile), driverEntity.Profile),
            };

            // Loop through image properties and update paths if necessary
            foreach (var (driverDTOProperty, driverEntityFile) in imageProperties)
            {
                var dtoValue = typeof(DriverUpdateFormDTO)
                    .GetProperty(driverDTOProperty)
                    ?.GetValue(driverUpdateFormDto)
                    ?.ToString();
                if (string.IsNullOrEmpty(dtoValue))
                {
                    typeof(DriverUpdateFormDTO)
                        .GetProperty(driverDTOProperty)
                        ?.SetValue(driverUpdateFormDto, driverEntityFile);
                }
                else if (dtoValue != driverEntityFile)
                {
                    typeof(DriverUpdateFormDTO)
                        .GetProperty(driverDTOProperty)
                        ?.SetValue(driverUpdateFormDto, $"driver/{driverUpdateFormDto.Id}{dtoValue}");
                }
            }

            DriverConverter.UpdateConvertModelToEntity(driverUpdateFormDto, ref driverEntity);
            _dbKiloTaxiContext.SaveChanges();
            result = true;
            return result;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                $"Error occurred while updating driver with Id: {driverUpdateFormDto.Id}"
            );
            throw;
        }
    }
    
        public void UpdateDriverStatus(DriverCreateFormDTO driverCreateDto)
    {
        try
        {
            var driverEntity = _dbKiloTaxiContext.Drivers.FirstOrDefault(d => d.Id == driverCreateDto.Id);
            driverEntity.AvabilityStatus = driverCreateDto.AvailableStatus.ToString();
            _dbKiloTaxiContext.SaveChanges();
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                $"Error occurred while updating driver status with Id: {driverCreateDto.Id}"
            );
            throw;
        }
    }

    public bool DeleteDriver(int id)
    {
        bool result = false;
        try
        {
            var driverEntity = _dbKiloTaxiContext.Drivers.FirstOrDefault(x => x.Id == id);
            if (driverEntity == null)
            {
                return result;
            }

            _dbKiloTaxiContext.Drivers.Remove(driverEntity);
            _dbKiloTaxiContext.SaveChanges();
            result = true;
            return result;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while delete driver with id: {id}");
            throw;
        }
    }
    
    public async Task<DriverInfoDTO> ValidateDriverCredentials(string EmailOrPhone, string password)
    {

        if (string.IsNullOrEmpty(EmailOrPhone) || string.IsNullOrEmpty(password))
        {
            return null; // Or throw an exception depending on your use case
        }

        Driver driverEntity =  _dbKiloTaxiContext.Drivers.SingleOrDefault(driver => driver.Phone == EmailOrPhone);
           
        if (driverEntity != null || ! BCrypt.Net.BCrypt.Verify(password, driverEntity.Password))
        {
            return DriverConverter.ConvertEntityToModel(driverEntity, _mediaHostUrl);
        }

        // Convert the entity to a DTO
        return null;
    }

    public List<DriverInfoDTO> SearchNearbyOnlineDriver() {
        var onlineDrivers = _dbKiloTaxiContext.Drivers
        .Where(driver => driver.AvabilityStatus == DriverStatus.Online.ToString())
        .ToList(); 

       
        var onlineDriverDTOs = onlineDrivers
            .Select(driver => DriverConverter.ConvertEntityToModel(driver, _mediaHostUrl))
            .ToList();

        if (!onlineDriverDTOs.Any())
        {
            throw new Exception("No online drivers found.");
        }

        return onlineDriverDTOs;
    }
}
