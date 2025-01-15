using System;
using System.Collections.Generic;
using System.Linq;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class PromotionConverter
    {
        public static PromotionInfoDTO ConvertEntityToModel(Promotion promotionEntity)
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

            return new PromotionInfoDTO()
            {
                Id = promotionEntity.Id,
                PromoCode = promotionEntity.PromoCode,
                CreatedDate = promotionEntity.CreatedDate,
                Quantity = promotionEntity.Quantity,
                ExpiredDate = promotionEntity.ExpiredDate,
                Unit = promotionEntity.Unit,
                Description = promotionEntity.Description,
                PromotionType = Enum.Parse<PromotionType>(promotionEntity.PromotionType),
                Status = Enum.Parse<PromotionStatus>(promotionEntity.Status),
                ApplicableTo = Enum.Parse<ApplicableTo>(promotionEntity.ApplicableTo),
                CustomerIds = promotionEntity.PromotionUsers?.Select(pu => pu.CustomerId).ToList(),
                CustomerNames = promotionEntity
                    .PromotionUsers?.Select(pu => pu.Customer?.Name)
                    .ToList(),
            };
        }

        public static void ConvertModelToEntity(
            PromotionFormDTO promotionFormDTO,
            ref Promotion promotionEntity
        )
        {
            try
            {
                if (promotionFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(promotionFormDTO)),
                        "PromotionFormDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(promotionFormDTO),
                        "Source promotionFormDTO cannot be null"
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

                promotionEntity.Id = promotionFormDTO.Id;
                promotionEntity.PromoCode = promotionFormDTO.PromoCode;
                promotionEntity.CreatedDate = promotionFormDTO.CreatedDate;
                promotionEntity.ExpiredDate = promotionFormDTO.ExpiredDate;
                promotionEntity.Quantity = promotionFormDTO.Quantity;
                promotionEntity.Description = promotionFormDTO.Description;
                promotionEntity.Unit = promotionFormDTO.Unit;
                promotionEntity.PromotionType = promotionFormDTO.PromotionType.ToString();
                promotionEntity.Status = promotionFormDTO.Status.ToString();
                promotionEntity.ApplicableTo = promotionFormDTO.ApplicableTo.ToString();

                if (promotionFormDTO.CustomerIds != null)
                {
                    promotionEntity.PromotionUsers = promotionFormDTO
                        .CustomerIds.Select(customerId => new PromotionUser
                        {
                            CustomerId = customerId,
                            PromotionId = promotionFormDTO.Id,
                        })
                        .ToList();
                }
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
