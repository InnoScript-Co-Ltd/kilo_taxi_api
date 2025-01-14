using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class AdminConverter
    {
        public static AdminInfoDTO ConvertEntityToModel(Admin adminEntity)
        {
            if (adminEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(adminEntity)),
                    "Admin entity is null"
                );
                throw new ArgumentNullException(
                    nameof(adminEntity),
                    "Source adminEntity cannot be null"
                );
            }

            return new AdminInfoDTO
            {
                Id = adminEntity.Id,
                Name = adminEntity.Name,
                Phone = adminEntity.Phone,
                Email = adminEntity.Email,
                Address = adminEntity.Address,
                Role = adminEntity.Role,
                RefreshToken = adminEntity.RefreshToken,
                RefreshTokenExpiryTime = adminEntity.RefreshTokenExpiryTime,
                Otp = adminEntity.Otp,
                EmailVerifiedAt = adminEntity.EmailVerifiedAt,
                PhoneVerifiedAt = adminEntity.PhoneVerifiedAt,
                Gender = string.IsNullOrEmpty(adminEntity.Gender)
                    ? GenderType.Undefined
                    : Enum.Parse<GenderType>(adminEntity.Gender),
                Status = string.IsNullOrEmpty(adminEntity.Status)
                    ? CustomerStatus.Pending
                    : Enum.Parse<CustomerStatus>(adminEntity.Status),
            };
        }

        public static void ConvertModelToEntity(AdminFormDTO adminFormDTO, ref Admin adminEntity)
        {
            try
            {
                if (adminFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(adminFormDTO)),
                        "AdminDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(adminFormDTO),
                        "Source adminFormDTO cannot be null"
                    );
                }

                adminEntity.Id = adminFormDTO.Id;
                adminEntity.Name = adminFormDTO.Name;
                adminEntity.Phone = adminFormDTO.Phone;
                adminEntity.RefreshToken = adminFormDTO.RefreshToken;
                adminEntity.RefreshTokenExpiryTime = adminFormDTO.RefreshTokenExpiryTime;
                adminEntity.Email = adminFormDTO.Email;
                adminEntity.Role = adminFormDTO.Role;
                adminEntity.EmailVerifiedAt = adminFormDTO.EmailVerifiedAt;
                adminEntity.PhoneVerifiedAt = adminFormDTO.PhoneVerifiedAt;
                adminEntity.Otp = adminFormDTO.Otp;
                adminEntity.Password = adminFormDTO.Password;
                adminEntity.Gender = adminFormDTO.Gender.ToString();
                adminEntity.Address = adminFormDTO.Address;
                adminEntity.Status = adminFormDTO.Status.ToString();
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
                    "Error during AdminDTO to Admin entity conversion"
                );
                throw;
            }
        }
    }
}
