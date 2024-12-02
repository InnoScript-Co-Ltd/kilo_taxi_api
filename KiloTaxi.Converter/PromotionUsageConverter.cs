using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class PromotionUsageConverter
    {
        public static PromotionUsageDTO ConvertEntityToModel(PromotionUsage promotionUsageEntity)
        {
            if (promotionUsageEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(promotionUsageEntity)),
                    "Promotion Usage entity is null"
                );
                throw new ArgumentNullException(
                    nameof(promotionUsageEntity),
                    "Source promotionUsageEntity cannot be null"
                );
            }

            return new PromotionUsageDTO()
            {
                Id = promotionUsageEntity.Id,
                DiscountApplied = promotionUsageEntity.DiscountApplied,
                WalletTransactionId = promotionUsageEntity.WalletTransactionId,
                PromotionId = promotionUsageEntity.PromotionId,
                CustomerId = promotionUsageEntity.CustomerId,
            };
        }

        public static void ConvertModelToEntity(
            PromotionUsageDTO promotionUsageDTO,
            ref PromotionUsage promotionUsageEntity
        )
        {
            try
            {
                if (promotionUsageDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(promotionUsageDTO)),
                        "promotionUsageDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(promotionUsageDTO),
                        "Source promotionUsageDTO cannot be null"
                    );
                }

                if (promotionUsageEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(promotionUsageEntity)),
                        "Promotion Usage entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(promotionUsageEntity),
                        "Target promotionUsageEntity cannot be null"
                    );
                }

                promotionUsageEntity.Id = promotionUsageDTO.Id;
                promotionUsageEntity.DiscountApplied = promotionUsageDTO.DiscountApplied;
                promotionUsageEntity.WalletTransactionId = promotionUsageDTO.WalletTransactionId;
                promotionUsageEntity.PromotionId = promotionUsageDTO.PromotionId;
                promotionUsageEntity.CustomerId = promotionUsageDTO.CustomerId;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during promotionUsageDTO to Promotion Usage entity conversion"
                );
                throw;
            }
        }
    }
}
