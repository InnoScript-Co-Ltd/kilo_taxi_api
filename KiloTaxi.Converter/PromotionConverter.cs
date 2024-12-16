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
                CreatedDate = promotionEntity.CreatedDate,
                Quantity = promotionEntity.Quantity,
                Description = promotionEntity.Description,
                ExpiredDate = promotionEntity.ExpiredDate,
                Unit = promotionEntity.Unit,
                Status = Enum.Parse<PromotionStatus>(promotionEntity.Status),
                ApplicableTo =Enum.Parse<ApplicableTo>(promotionEntity.ApplicableTo),
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
                promotionEntity.CreatedDate = promotionDTO.CreatedDate;
                promotionEntity.ExpiredDate = promotionDTO.ExpiredDate;
                promotionEntity.Quantity = promotionDTO.Quantity;
                promotionEntity.Description = promotionDTO.Description;
                promotionEntity.Unit = promotionDTO.Unit;
                promotionEntity.PromotionType = promotionDTO.PromotionType.ToString();
                promotionEntity.ApplicableTo = promotionDTO.ApplicableTo.ToString();
                promotionEntity.Status = promotionDTO.Status.ToString();
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
