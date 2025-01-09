using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class CustomerConverter
    {
        public static CustomerInfoDTO ConvertEntityToModel(Customer customerEntity, string mediaHostUrl)
        {
            if (customerEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(customerEntity)),
                    "Customer entity is null"
                );
                throw new ArgumentNullException(
                    nameof(customerEntity),
                    "Source customerEntity cannot be null"
                );
            }

            return new CustomerInfoDTO()
            {
                Id = customerEntity.Id,
                Name = customerEntity.Name,
                Profile = mediaHostUrl + customerEntity.Profile,
                MobilePrefix = customerEntity.MobilePrefix,
                Role = customerEntity.Role,
                CreatedDate = customerEntity.CreatedDate,
                Phone = customerEntity.Phone,
                Email = customerEntity.Email,
                Gender =  Enum.Parse<GenderType>(customerEntity.Gender),
                Password= customerEntity.Password,
                Address = customerEntity.Address,
                City = customerEntity.City,
                Township = customerEntity.Township,
                Status =    Enum.Parse<CustomerStatus>(customerEntity.Status),
                KycStatus =Enum.Parse<KycStatus>(customerEntity.KycStatus),
            };
        }

        public static void ConvertModelToEntity(
            CustomerFormDTO customerFormDto,
            ref Customer customerEntity
        )
        {
            try
            {
                if (customerFormDto == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(customerFormDto)),
                        "CustomerFormDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(customerFormDto),
                        "Source customerFormDto cannot be null"
                    );
                }

                if (customerEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(customerEntity)),
                        "Customer entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(customerEntity),
                        "Target customerEntity cannot be null"
                    );
                }

                customerEntity.Id = customerFormDto.Id;
                customerEntity.Name = customerFormDto.Name;
                customerEntity.Profile = customerFormDto.Profile;
                customerEntity.MobilePrefix = customerFormDto.MobilePrefix;
                customerEntity.RefreshToken = customerFormDto.RefreshToken;
                customerEntity.CreatedDate=customerFormDto.CreatedDate ?? DateTime.MinValue;
                customerEntity.RefreshTokenExpiryTime = customerFormDto.RefreshTokenExpiryTime;
                customerEntity.Otp = customerFormDto.Otp;
                customerEntity.Phone = customerFormDto.Phone;
                customerEntity.Email = customerFormDto.Email;
                customerEntity.Password = customerFormDto.Password;
                customerEntity.Role = "Customer";
                customerEntity.EmailVerifiedAt = customerFormDto.EmailVerifiedAt;
                customerEntity.PhoneVerifiedAt = customerFormDto.PhoneVerifiedAt;
                customerEntity.Password = customerFormDto.Password;
                customerEntity.Gender = string.IsNullOrEmpty(customerFormDto.Gender.ToString()) 
                    ? GenderType.Undefined.ToString() 
                    : customerFormDto.Gender.ToString();
                customerEntity.Address = customerFormDto.Address;
                customerEntity.State = customerFormDto.State;
                customerEntity.City = customerFormDto.City;
                customerEntity.Township = customerFormDto.Township;
                customerEntity.Status = string.IsNullOrEmpty(customerFormDto.Status.ToString()) 
                    ? CustomerStatus.Pending.ToString() 
                    : customerFormDto.Status.ToString();
                customerEntity.KycStatus = string.IsNullOrEmpty(customerFormDto.KycStatus.ToString()) 
                    ? KycStatus.Pending.ToString() 
                    : customerFormDto.KycStatus.ToString();
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during CustomerDTO to Customer entity conversion"
                );
                throw;
            }
        }
    }
}
