using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using System;

namespace KiloTaxi.Converter
{
    public static class TopUpTransactionConverter
    {
        public static TopUpTransactionDTO ConvertEntityToModel(TopUpTransaction topUpTransactionEntity)
        {
            if (topUpTransactionEntity == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(topUpTransactionEntity)), "TopUpTransaction entity is null");
                throw new ArgumentNullException(nameof(topUpTransactionEntity), "Source TopUpTransaction entity cannot be null");
            }

            return new TopUpTransactionDTO
            {
                Id = topUpTransactionEntity.Id,
                Amount = topUpTransactionEntity.Amount,
                TransactionScreenShoot = topUpTransactionEntity.TransactionScreenShoot,
                Status = Enum.Parse<TopUpTransactionStatus>(topUpTransactionEntity.Status),
                WalletId = topUpTransactionEntity.WalletId,
                // PaymentChannelId is commented out in the entity; update this if it becomes part of the entity.
            };
        }
        
        public static void ConvertModelToEntity(TopUpTransactionDTO topUpTransactionDTO, ref TopUpTransaction topUpTransactionEntity)
        {
            try
            {
                if (topUpTransactionDTO == null)
                {
                    LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(topUpTransactionDTO)), "TopUpTransactionDTO model is null");
                    throw new ArgumentNullException(nameof(topUpTransactionDTO), "Source TopUpTransactionDTO model cannot be null");
                }

                topUpTransactionEntity.Id = topUpTransactionDTO.Id;
                topUpTransactionEntity.Amount = topUpTransactionDTO.Amount;
                topUpTransactionEntity.TransactionScreenShoot = topUpTransactionDTO.TransactionScreenShoot;
                topUpTransactionEntity.Status = topUpTransactionDTO.Status.ToString();
                topUpTransactionEntity.WalletId = topUpTransactionDTO.WalletId;
                // PaymentChannelId is commented out in the entity; update this if it becomes part of the entity.
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
}
