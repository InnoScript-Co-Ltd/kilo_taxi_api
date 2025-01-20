using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter;

public static class KiloAmountConverter
{
    public static KiloAmountDTO ConvertEntityToModel(KiloAmount kiloAmountEntity)
    {
        if (kiloAmountEntity == null)
        {
            LoggerHelper.Instance.LogError(
                new ArgumentNullException(nameof(kiloAmountEntity)),
                "KiloAmount entity is null"
            );
            throw new ArgumentNullException(
                nameof(kiloAmountEntity),
                "Source KiloAmount entity cannot be null"
            );
        }

        return new KiloAmountDTO
        {
            Id = kiloAmountEntity.Id,
            Kilo = kiloAmountEntity.Kilo,
            Amount = kiloAmountEntity.Amount,
        };
    }

    public static void ConvertModelToEntity(
        KiloAmountDTO kiloAmountDTO,
        ref KiloAmount kiloAmountEntity
    )
    {
        try
        {
            if (kiloAmountDTO == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(kiloAmountDTO)),
                    "KiloAmountDTO model is null"
                );
                throw new ArgumentNullException(
                    nameof(kiloAmountDTO),
                    "Source KiloAmountDTO model cannot be null"
                );
            }

            kiloAmountEntity.Id = kiloAmountDTO.Id;
            kiloAmountEntity.Kilo = kiloAmountDTO.Kilo;
            kiloAmountEntity.Amount = kiloAmountDTO.Amount;
        }
        catch (ArgumentException ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                "Argument exception during model-to-entity conversion"
            );
            throw;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                "Unexpected error during model-to-entity conversion"
            );
            throw;
        }
    }
}
