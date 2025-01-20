using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter;

public static class TravelRateConverter
{
    public static TravelRateInfoDTO ConvertEntityToModel(TravelRate travelRateEntity)
    {
        if (travelRateEntity == null)
        {
            LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(travelRateEntity)), "TravelRate entity is null");
            throw new ArgumentNullException(nameof(travelRateEntity), "Source travelRateEntity cannot be null");
        }

        return new TravelRateInfoDTO()
        {
            Id = travelRateEntity.Id,
            Unit = travelRateEntity.Unit,
            Rate = travelRateEntity.Rate,
            CityId = travelRateEntity.CityId,
            CityName = travelRateEntity.City?.Name ?? null,
            VehicleTypeId = travelRateEntity.VehicleTypeId,
            VehicleTypeName = travelRateEntity.VehicleType?.Name ?? null,
               
        };
    }

    public static void ConvertModelToEntity(TravelRateFormDTO travelRateFormDTO, ref TravelRate travelRateEntity)
    {
        try
        {
            if (travelRateFormDTO == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(travelRateFormDTO)), "travelRateFormDTO is null");
                throw new ArgumentNullException(nameof(travelRateFormDTO), "Source travelRateFormDTO cannot be null");
            }

            travelRateEntity.Id = travelRateFormDTO.Id;
            travelRateEntity.Unit = travelRateFormDTO.Unit;
            travelRateEntity.Rate = travelRateFormDTO.Rate;
            travelRateEntity.CityId = travelRateFormDTO.CityId;
            travelRateEntity.VehicleTypeId = travelRateFormDTO.VehicleTypeId;
              
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