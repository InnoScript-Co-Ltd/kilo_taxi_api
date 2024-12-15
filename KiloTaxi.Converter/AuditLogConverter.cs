using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class AuditLogConverter
    {
        public static AuditLogDTO ConvertEntityToModel(AuditLog auditLogEntity)
        {
            if (auditLogEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(auditLogEntity)),
                    "Audit Log entity is null"
                );
                throw new ArgumentNullException(
                    nameof(auditLogEntity),
                    "Source auditLogEntity cannot be null"
                );
            }

            return new AuditLogDTO()
            {
                Id = auditLogEntity.Id,
                TableName = auditLogEntity.TableName,
                RecordId = auditLogEntity.RecordId,
                Operation = auditLogEntity.Operation,
                OldValues = auditLogEntity.OldValues,
                NewValues = auditLogEntity.NewValues,
                ChangedBy = auditLogEntity.ChangedBy,
                ChangedDate = auditLogEntity.ChangedDate,
            };
        }

        public static void ConvertModelToEntity(
            AuditLogDTO auditLogDTO,
            ref AuditLog auditLogEntity
        )
        {
            try
            {
                if (auditLogDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(auditLogDTO)),
                        "auditLogDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(auditLogDTO),
                        "Source auditLogDTO cannot be null"
                    );
                }

                if (auditLogEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(auditLogEntity)),
                        "Audit Log entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(auditLogEntity),
                        "Target auditLogEntity cannot be null"
                    );
                }
                
                auditLogEntity.Id = auditLogDTO.Id;
                auditLogEntity.TableName = auditLogDTO.TableName;
                auditLogEntity.RecordId = auditLogDTO.RecordId;
                auditLogEntity.Operation  = auditLogDTO.Operation;
                auditLogEntity.OldValues = auditLogDTO.OldValues;
                auditLogEntity.NewValues = auditLogDTO.NewValues;
                auditLogEntity.ChangedBy = auditLogDTO.ChangedBy;
                auditLogEntity.ChangedDate = auditLogDTO.ChangedDate;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during AuditLogDTO to Review entity conversion"
                );
                throw;
            }
        }
    }
}
