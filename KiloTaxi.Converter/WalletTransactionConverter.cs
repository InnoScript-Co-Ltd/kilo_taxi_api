using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using System;

namespace KiloTaxi.Converter
{
    public static class WalletTransactionConverter
    {
        // Convert WalletTransaction entity to DTO
        public static WalletTransactionDTO ConvertEntityToModel(WalletTransaction walletTransactionEntity)
        {
            if (walletTransactionEntity == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(walletTransactionEntity)), "WalletTransaction entity is null");
                throw new ArgumentNullException(nameof(walletTransactionEntity), "Source WalletTransaction entity cannot be null");
            }

            return new WalletTransactionDTO
            {
                Id = walletTransactionEntity.Id,
                Amount = walletTransactionEntity.Amount,
                BalanceBefore = walletTransactionEntity.BalanceBefore,
                BalanceAfter = walletTransactionEntity.BalanceAfter,
                TransactionDate = walletTransactionEntity.TransactionDate,
                ReferenceId = walletTransactionEntity.ReferenceId,
                Details = walletTransactionEntity.Details,
                TransactionType = Enum.Parse<TransactionType>(walletTransactionEntity.TransactionType),
                WalletUserMappingId = walletTransactionEntity.WalletUserMappingId
            };
        }

        // Convert WalletTransactionDTO model to entity
        public static void ConvertModelToEntity(WalletTransactionDTO walletTransactionDTO, ref WalletTransaction walletTransactionEntity)
        {
            try
            {
                if (walletTransactionDTO == null)
                {
                    LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(walletTransactionDTO)), "WalletTransactionDTO model is null");
                    throw new ArgumentNullException(nameof(walletTransactionDTO), "Source WalletTransactionDTO model cannot be null");
                }

                walletTransactionEntity.Id = walletTransactionDTO.Id;
                walletTransactionEntity.Amount = walletTransactionDTO.Amount;
                walletTransactionEntity.BalanceBefore = walletTransactionDTO.BalanceBefore;
                walletTransactionEntity.BalanceAfter = walletTransactionDTO.BalanceAfter;
                walletTransactionEntity.TransactionDate = walletTransactionDTO.TransactionDate;
                walletTransactionEntity.ReferenceId = walletTransactionDTO.ReferenceId;
                walletTransactionEntity.Details = walletTransactionDTO.Details;
                walletTransactionEntity.TransactionType = walletTransactionDTO.TransactionType.ToString();
                walletTransactionEntity.WalletUserMappingId = walletTransactionDTO.WalletUserMappingId;
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
