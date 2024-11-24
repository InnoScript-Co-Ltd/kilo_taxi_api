using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class ReviewConverter
    {
        public static ReviewDTO ConvertEntityToModel(Review reviewEntity)
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

            return new ReviewDTO()
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

        public static void ConvertModelToEntity(ReviewDTO reviewDTO, ref Review reviewEntity)
        {
            try
            {
                if (reviewDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(reviewDTO)),
                        "ReviewDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(reviewDTO),
                        "Source reviewDTO cannot be null"
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

                reviewEntity.Id = reviewDTO.Id;
                reviewEntity.Rating = reviewDTO.Rating;
                reviewEntity.ReviewContent = reviewDTO.ReviewContent;
                reviewEntity.CustomerId = reviewDTO.CustomerId;
                reviewEntity.DriverId = reviewDTO.DriverId;
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
