using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter;

    public static class CityConverter
    {
        public static CityDTO ConvertEntityToModel(City cityEntity)
        {
            if (cityEntity == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(cityEntity)), "City entity is null");
                throw new ArgumentNullException(nameof(cityEntity), "Source cityEntity cannot be null");
            }

            return new CityDTO()
            {
                Id = cityEntity.Id,
                Name = cityEntity.Name,
               
            };
        }

        public static void ConvertModelToEntity(CityDTO cityDTO, ref City cityEntity)
        {
            try
            {
                if (cityDTO == null)
                {
                    LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(cityDTO)), "cityDTO is null");
                    throw new ArgumentNullException(nameof(cityDTO), "Source cityDTO cannot be null");
                }

                cityEntity.Id = cityDTO.Id;
                cityEntity.Name = cityDTO.Name;
               
              
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
