using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using System;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class TopUpTransactionConverter
    {
        public static TopUpTransactionInfoDTO ConvertEntityToModel(TopUpTransaction topUpTransactionEntity)
        {
            if (topUpTransactionEntity == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(topUpTransactionEntity)), "TopUpTransaction entity is null");
                throw new ArgumentNullException(nameof(topUpTransactionEntity), "Source TopUpTransaction entity cannot be null");
            }

            return new TopUpTransactionInfoDTO
            {
                Id = topUpTransactionEntity.Id,
                Amount = topUpTransactionEntity.Amount,
                TransactionScreenShoot = topUpTransactionEntity.TransactionScreenShoot,
                PhoneNumber = topUpTransactionEntity.PhoneNumber,
                DigitalPaymentFromPhoneNumber = topUpTransactionEntity.DigitalPaymentFromPhoneNumber,
                DigitalPaymentToPhoneNumber = topUpTransactionEntity.DigitalPaymentToPhoneNumber,
                Status = Enum.Parse<TopUpTransactionStatus>(topUpTransactionEntity.Status),
                PaymentChannelId = topUpTransactionEntity.PaymentChannelId,
            };
        }
        
        public static void ConvertModelToEntity(TopUpTransactionFormDTO topUpTransactionFormDTO, ref TopUpTransaction topUpTransactionEntity)
        {
            try
            {
                if (topUpTransactionFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(topUpTransactionFormDTO)), "TopUpTransactionFormDTO model is null");
                    throw new ArgumentNullException(nameof(topUpTransactionFormDTO), "Source TopUpTransactionFormDTO model cannot be null");
                }

                topUpTransactionEntity.Id = topUpTransactionFormDTO.Id;
                topUpTransactionEntity.Amount = topUpTransactionFormDTO.Amount;
                topUpTransactionEntity.TransactionScreenShoot = topUpTransactionFormDTO.TransactionScreenShoot;
                topUpTransactionEntity.PhoneNumber = topUpTransactionFormDTO.PhoneNumber;
                topUpTransactionEntity.DigitalPaymentFromPhoneNumber = topUpTransactionFormDTO.DigitalPaymentFromPhoneNumber;
                topUpTransactionEntity.DigitalPaymentToPhoneNumber = topUpTransactionFormDTO.DigitalPaymentToPhoneNumber;
                topUpTransactionEntity.Status = topUpTransactionFormDTO.Status.ToString();
                topUpTransactionEntity.PaymentChannelId = topUpTransactionFormDTO.PaymentChannelId;
                

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
