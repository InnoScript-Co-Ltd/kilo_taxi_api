using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class CustomerConverter
    {
        public static CustomerDTO ConvertEntityToModel(Customer customerEntity, string mediaHostUrl)
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

            return new CustomerDTO()
            {
                Id = customerEntity.Id,
                Name = customerEntity.Name,
                Profile = mediaHostUrl + customerEntity.Profile,
                MobilePrefix = customerEntity.MobilePrefix,
                Phone = customerEntity.Phone,
                Email = customerEntity.Email,
                Dob = customerEntity.Dob,
                Nrc = customerEntity.Nrc,
                NrcImageFront = mediaHostUrl + customerEntity.NrcImageFront,
                NrcImageBack = mediaHostUrl + customerEntity.NrcImageBack,
                EmailVerifiedAt = customerEntity.EmailVerifiedAt,
                PhoneVerifiedAt = customerEntity.PhoneVerifiedAt,
                Password = customerEntity.Password,
                Gender = string.IsNullOrEmpty(customerEntity.Gender ) ? GenderType.Undefined : Enum.Parse<GenderType>(customerEntity.Gender),
                Address = customerEntity.Address,
                State = customerEntity.State,
                City = customerEntity.City,
                Township = customerEntity.Township,
                Status =   string.IsNullOrEmpty(customerEntity.Status ) ? CustomerStatus.Pending : Enum.Parse<CustomerStatus>(customerEntity.Status),
                KycStatus = string.IsNullOrEmpty(customerEntity.KycStatus) ? KycStatus.Pending : Enum.Parse<KycStatus>(customerEntity.KycStatus),
            };
        }

        public static void ConvertModelToEntity(
            CustomerDTO customerDTO,
            ref Customer customerEntity
        )
        {
            try
            {
                if (customerDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(customerDTO)),
                        "CustomerDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(customerDTO),
                        "Source customerDTO cannot be null"
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

                customerEntity.Id = customerDTO.Id;
                customerEntity.Name = customerDTO.Name;
                customerEntity.Profile = customerDTO.Profile;
                customerEntity.MobilePrefix = customerDTO.MobilePrefix;
                customerEntity.Phone = customerDTO.Phone;
                customerEntity.Email = customerDTO.Email;
                customerEntity.Dob = customerDTO.Dob;
                customerEntity.Nrc = customerDTO.Nrc;
                customerEntity.NrcImageFront = customerDTO.NrcImageFront;
                customerEntity.NrcImageBack = customerDTO.NrcImageBack;
                customerEntity.EmailVerifiedAt = customerDTO.EmailVerifiedAt;
                customerEntity.PhoneVerifiedAt = customerDTO.PhoneVerifiedAt;
                customerEntity.Password = customerDTO.Password;
                customerEntity.Gender = customerDTO.Gender.ToString();
                customerEntity.Address = customerDTO.Address;
                customerEntity.State = customerDTO.State;
                customerEntity.City = customerDTO.City;
                customerEntity.Township = customerDTO.Township;
                customerEntity.Status = customerDTO.Status.ToString();
                customerEntity.KycStatus = customerDTO.KycStatus.ToString();
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
