using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter;

public static class TravelRateConverter
{
    public static TravelRateDTO ConvertEntityToModel(TravelRate travelRateEntity)
    {
        if (travelRateEntity == null)
        {
            LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(travelRateEntity)), "TravelRate entity is null");
            throw new ArgumentNullException(nameof(travelRateEntity), "Source travelRateEntity cannot be null");
        }

        return new TravelRateDTO()
        {
            Id = travelRateEntity.Id,
            Unit = travelRateEntity.Unit,
            Rate = travelRateEntity.Rate,
            CityId = travelRateEntity.CityId,
            CityName = travelRateEntity.City.Name,
            VehicleTypeId = travelRateEntity.VehicleTypeId,
            VehicleTypeName = travelRateEntity.VehicleType.Name,
               
        };
    }

    public static void ConvertModelToEntity(TravelRateDTO travelRateDTO, ref TravelRate travelRateEntity)
    {
        try
        {
            if (travelRateDTO == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(travelRateDTO)), "travelRateDTO is null");
                throw new ArgumentNullException(nameof(travelRateDTO), "Source travelRateDTO cannot be null");
            }

            travelRateEntity.Id = travelRateDTO.Id;
            travelRateEntity.Unit = travelRateDTO.Unit;
            travelRateEntity.Rate = travelRateDTO.Rate;
            travelRateEntity.CityId = travelRateDTO.CityId;
            travelRateEntity.VehicleTypeId = travelRateDTO.VehicleTypeId;
              
        }
        catch (ArgumentException ex)
        {
            LoggerHelper.Instance.LogError(ex, "Argument exception during model-to-entity conversion");
            throw;

        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Unexpected error during model-to-entity conversion");
            throw;

        }
    }
}