using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class AdminConverter
    {
        public static AdminDTO ConvertEntityToModel(Admin adminEntity)
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

            return new AdminDTO()
            {
                Id = adminEntity.Id,
                Name = adminEntity.Name,
                Phone = adminEntity.Phone,
                Email = adminEntity.Email,
                Role = adminEntity.Role,
                Otp = adminEntity.Otp,
                RefreshToken = adminEntity.RefreshToken,
                RefreshTokenExpiryTime = adminEntity.RefreshTokenExpiryTime,
                EmailVerifiedAt = adminEntity.EmailVerifiedAt,
                PhoneVerifiedAt = adminEntity.PhoneVerifiedAt,
                Password = adminEntity.Password,
                Gender = Enum.Parse<GenderType>(adminEntity.Gender),
                Address = adminEntity.Address,
                Status = Enum.Parse<CustomerStatus>(adminEntity.Status),
            };
        }

        public static void ConvertModelToEntity(AdminDTO adminDTO, ref Admin adminEntity)
        {
            try
            {
                if (adminDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(adminDTO)),
                        "AdminDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(adminDTO),
                        "Source adminDTO cannot be null"
                    );
                }

                adminEntity.Id = adminDTO.Id;
                adminEntity.Name = adminDTO.Name;
                adminEntity.Phone = adminDTO.Phone;
                adminEntity.RefreshToken = adminDTO.RefreshToken;
                adminEntity.RefreshTokenExpiryTime = adminDTO.RefreshTokenExpiryTime;
                adminEntity.Email = adminDTO.Email;
                adminEntity.Role = adminDTO.Role;
                adminEntity.EmailVerifiedAt = adminDTO.EmailVerifiedAt;
                adminEntity.PhoneVerifiedAt = adminDTO.PhoneVerifiedAt;
                adminEntity.Otp = adminDTO.Otp;
                adminEntity.Password = adminDTO.Password;
                adminEntity.Gender = adminDTO.Gender.ToString();
                adminEntity.Address = adminDTO.Address;
                adminEntity.Status = adminDTO.Status.ToString();
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
