using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;

namespace KiloTaxi.Converter
{
    public static class TopUpTransactionConverter
    {
        public static TopUpTransactionInfoDTO ConvertEntityToModel(
            TopUpTransaction topUpTransactionEntity,
            string mediaHostUrl
        )
        {
            if (topUpTransactionEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(topUpTransactionEntity)),
                    "TopUpTransaction entity is null"
                );
                throw new ArgumentNullException(
                    nameof(topUpTransactionEntity),
                    "Source TopUpTransaction entity cannot be null"
                );
            }

            return new TopUpTransactionInfoDTO
            {
                Id = topUpTransactionEntity.Id,
                Amount = topUpTransactionEntity.Amount,
                TransactionScreenShoot =
                    mediaHostUrl + topUpTransactionEntity.TransactionScreenShoot,
                PhoneNumber = topUpTransactionEntity.PhoneNumber,
                DigitalPaymentFromPhoneNumber =
                    topUpTransactionEntity.DigitalPaymentFromPhoneNumber,
                DigitalPaymentToPhoneNumber = topUpTransactionEntity.DigitalPaymentToPhoneNumber,
                Status = Enum.Parse<TopUpTransactionStatus>(topUpTransactionEntity.Status),
                PaymentChannelId = topUpTransactionEntity.PaymentChannelId,
            };
        }

        public static void ConvertModelToEntity(
            TopUpTransactionFormDTO topUpTransactionDTO,
            ref TopUpTransaction topUpTransactionEntity
        )
        {
            try
            {
                if (topUpTransactionDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(topUpTransactionDTO)),
                        "TopUpTransactionDTO model is null"
                    );
                    throw new ArgumentNullException(
                        nameof(topUpTransactionDTO),
                        "Source TopUpTransactionDTO model cannot be null"
                    );
                }

                topUpTransactionEntity.Id = topUpTransactionDTO.Id;
                topUpTransactionEntity.Amount = topUpTransactionDTO.Amount;
                topUpTransactionEntity.TransactionScreenShoot =
                    topUpTransactionDTO.TransactionScreenShoot;
                topUpTransactionEntity.PhoneNumber = topUpTransactionDTO.PhoneNumber;
                topUpTransactionEntity.DigitalPaymentFromPhoneNumber =
                    topUpTransactionDTO.DigitalPaymentFromPhoneNumber;
                topUpTransactionEntity.DigitalPaymentToPhoneNumber =
                    topUpTransactionDTO.DigitalPaymentToPhoneNumber;
                topUpTransactionEntity.Status = topUpTransactionDTO.Status.ToString();
                topUpTransactionEntity.PaymentChannelId = topUpTransactionDTO.PaymentChannelId;
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
}
