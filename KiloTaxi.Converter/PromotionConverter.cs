using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class PromotionConverter
    {
        public static PromotionDTO ConvertEntityToModel(Promotion promotionEntity)
        {
            if (promotionEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(promotionEntity)),
                    "Promotion entity is null"
                );
                throw new ArgumentNullException(
                    nameof(promotionEntity),
                    "Source promotionEntity cannot be null"
                );
            }

            return new PromotionDTO()
            {
                Id = promotionEntity.Id,
                PromoCode = promotionEntity.PromoCode,
                ExpiredAt = promotionEntity.ExpiredAt,
                Value = promotionEntity.Value,
                Status = Enum.Parse<PromotionStatus>(promotionEntity.Status),
                CustomerId = promotionEntity.CustomerId,
                ApplicableTo = promotionEntity.ApplicableTo,
                CustomerName = promotionEntity.Customer.Name,
            };
        }

        public static void ConvertModelToEntity(
            PromotionDTO promotionDTO,
            ref Promotion promotionEntity
        )
        {
            try
            {
                if (promotionDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(promotionDTO)),
                        "PromotionDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(promotionDTO),
                        "Source promotionDTO cannot be null"
                    );
                }

                if (promotionEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(promotionEntity)),
                        "Promotion entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(promotionEntity),
                        "Target promotionEntity cannot be null"
                    );
                }

                promotionEntity.Id = promotionDTO.Id;
                promotionEntity.PromoCode = promotionDTO.PromoCode;
                promotionEntity.ExpiredAt = promotionDTO.ExpiredAt;
                promotionEntity.Value = promotionDTO.Value;
                promotionEntity.PromotionType = promotionDTO.PromotionType;
                promotionEntity.ApplicableTo = promotionDTO.ApplicableTo;
                promotionEntity.Status = promotionDTO.Status.ToString();
                promotionEntity.CustomerId = promotionDTO.CustomerId;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during PromotionDTO to Promotion entity conversion"
                );
                throw;
            }
        }
    }
}
