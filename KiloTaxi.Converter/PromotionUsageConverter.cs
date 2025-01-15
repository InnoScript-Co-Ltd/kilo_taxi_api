using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class PromotionUsageConverter
    {
        public static PromotionUsageInfoDTO ConvertEntityToModel(PromotionUsage promotionUsageEntity)
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

            return new PromotionUsageInfoDTO()
            {
                Id = promotionUsageEntity.Id,
                DiscountApplied = promotionUsageEntity.DiscountApplied,
                WalletTransactionId = promotionUsageEntity.WalletTransactionId,
                PromotionId = promotionUsageEntity.PromotionId,
                CustomerId = promotionUsageEntity.CustomerId,
            };
        }

        public static void ConvertModelToEntity(
            PromotionUsageFormDTO promotionUsageFormDTO,
            ref PromotionUsage promotionUsageEntity
        )
        {
            try
            {
                if (promotionUsageFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(promotionUsageFormDTO)),
                        "promotionUsageFormDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(promotionUsageFormDTO),
                        "Source promotionUsageFormDTO cannot be null"
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

                promotionUsageEntity.Id = promotionUsageFormDTO.Id;
                promotionUsageEntity.DiscountApplied = promotionUsageFormDTO.DiscountApplied;
                promotionUsageEntity.WalletTransactionId = promotionUsageFormDTO.WalletTransactionId;
                promotionUsageEntity.PromotionId = promotionUsageFormDTO.PromotionId;
                promotionUsageEntity.CustomerId = promotionUsageFormDTO.CustomerId;
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
