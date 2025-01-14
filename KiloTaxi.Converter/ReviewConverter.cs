using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class ReviewConverter
    {
        public static ReviewInfoDTO ConvertEntityToModel(Review reviewEntity)
        {
            if (reviewEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(reviewEntity)),
                    "Review entity is null"
                );
                throw new ArgumentNullException(
                    nameof(reviewEntity),
                    "Source reviewEntity cannot be null"
                );
            }

            return new ReviewInfoDTO()
            {
                Id = reviewEntity.Id,
                Rating = reviewEntity.Rating,
                ReviewContent = reviewEntity.ReviewContent,
                CustomerId = reviewEntity.CustomerId,
                CustomerName = reviewEntity.Customer.Name,
                DriverId = reviewEntity.DriverId,
                DriverName = reviewEntity.Driver.Name,
            };
        }

        public static void ConvertModelToEntity(ReviewFormDTO reviewFormDTO, ref Review reviewEntity)
        {
            try
            {
                if (reviewFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(reviewFormDTO)),
                        "ReviewFormDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(reviewFormDTO),
                        "Source reviewFormDTO cannot be null"
                    );
                }

                if (reviewEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(reviewEntity)),
                        "Review entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(reviewEntity),
                        "Target reviewEntity cannot be null"
                    );
                }

                reviewEntity.Id = reviewFormDTO.Id;
                reviewEntity.Rating = reviewFormDTO.Rating;
                reviewEntity.ReviewContent = reviewFormDTO.ReviewContent;
                reviewEntity.CustomerId = reviewFormDTO.CustomerId;
                reviewEntity.DriverId = reviewFormDTO.DriverId;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during ReviewDTO to Review entity conversion"
                );
                throw;
            }
        }
    }
}
