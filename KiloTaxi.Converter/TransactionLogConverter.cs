using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class TransactionLogConverter
    {
        public static TransactionLogDTO ConvertEntityToModel(TransactionLog transactionLogEntity)
        {
            if (transactionLogEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(transactionLogEntity)),
                    "Transaction Log entity is null"
                );
                throw new ArgumentNullException(
                    nameof(transactionLogEntity),
                    "Source transactionLogEntity cannot be null"
                );
            }

            return new TransactionLogDTO()
            {
                Id = transactionLogEntity.Id,
                TransactionId = transactionLogEntity.TransactionId,
                LogDate = transactionLogEntity.LogDate,
                OperationType = transactionLogEntity.OperationType,
                Details = transactionLogEntity.Details,
                PerformedBy = transactionLogEntity.PerformedBy,
            };
        }

        public static void ConvertModelToEntity(
            TransactionLogDTO transactionLogDTO,
            ref TransactionLog transactionLogEntity
        )
        {
            try
            {
                if (transactionLogDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(transactionLogDTO)),
                        "transactionLogDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(transactionLogDTO),
                        "Source transactionLogDTO cannot be null"
                    );
                }

                if (transactionLogEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(transactionLogEntity)),
                        "Transaction Log entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(transactionLogEntity),
                        "Target transactionLogEntity cannot be null"
                    );
                }

                transactionLogEntity.Id = transactionLogDTO.Id;
                transactionLogEntity.TransactionId = transactionLogDTO.TransactionId;
                transactionLogEntity.LogDate = transactionLogDTO.LogDate;
                transactionLogEntity.OperationType = transactionLogDTO.OperationType;
                transactionLogEntity.Details = transactionLogDTO.Details;
                transactionLogEntity.PerformedBy = transactionLogDTO.PerformedBy;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during TransactionLogDTO to Review entity conversion"
                );
                throw;
            }
        }
    }
}
