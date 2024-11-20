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
                FixAmount = promotionEntity.FixAmount,
                Percentage = promotionEntity.Percentage,
                PromotionStatus = promotionEntity.PromotionStatus,
                CustomerId = promotionEntity.CustomerId,
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
                promotionEntity.FixAmount = promotionDTO.FixAmount;
                promotionEntity.Percentage = promotionDTO.Percentage;
                promotionEntity.PromotionStatus = promotionDTO.PromotionStatus.ToString();
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
