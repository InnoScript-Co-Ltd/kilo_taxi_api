using System.Linq.Expressions;
using System.Net;
using KiloTaxi.Common.ConfigurationSettings;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KiloTaxi.DataAccess.Implementation;

public class PaymentChannelRepository : IPaymentChannelRepository
{
    private readonly DbKiloTaxiContext _dbContext;
    private string _mediaHostUrl;

    public PaymentChannelRepository(DbKiloTaxiContext dbContext,IOptions<MediaSettings> mediaSettings)
    {
        _dbContext = dbContext;
        _mediaHostUrl = mediaSettings.Value.MediaHostUrl;

    }

    public PaymentChannelInfoDTO CreatePaymentChannel(PaymentChannelFormDTO paymentChannelFormDTO)
    {
        try
        {
            PaymentChannel paymentChannelEntity = new PaymentChannel();
            PaymentChannelConverter.ConvertModelToEntity(
                paymentChannelFormDTO,
                ref paymentChannelEntity
            );

            _dbContext.PaymentChannels.Add(paymentChannelEntity);
            _dbContext.SaveChanges();

            paymentChannelFormDTO.Id = paymentChannelEntity.Id;

            var filePaths = new List<(string PropertyName, string FilePath)>
            {
                (nameof(paymentChannelEntity.Icon), paymentChannelEntity.Icon),
            };
            foreach (var (propertyName, filePath) in filePaths)
            {
                if (!filePath.Contains("default.png"))
                {
                    switch (propertyName)
                    {
                        case nameof(paymentChannelEntity.Icon):
                            paymentChannelEntity.Icon =
                                $"payment-channel/{paymentChannelFormDTO.Id}{filePath}";
                            break;

                        default:
                            break;
                    }
                }
            }

            _dbContext.SaveChanges();

            var paymentChannelInfoDTO = PaymentChannelConverter.ConvertEntityToModel(
                paymentChannelEntity,
                _mediaHostUrl
            );

            LoggerHelper.Instance.LogInfo(
                $"Payment Channel Channel successfully with Id: {paymentChannelEntity.Id}"
            );

            return paymentChannelInfoDTO;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while creating payment channel.");
            throw;
        }
    }

    public bool UpdatePaymentChannel(PaymentChannelFormDTO paymentChannelFormDTO)
    {
        try
        {
            var paymentChannelEntity = _dbContext.PaymentChannels.FirstOrDefault(pc =>
                pc.Id == paymentChannelFormDTO.Id
            );
            if (paymentChannelEntity == null)
                return false;

            // List of image properties to update
            var imageProperties = new List<(
                string paymentChannelFormDTOProperty,
                string paymentChannelEntityFile
            )>
            {
                (nameof(paymentChannelFormDTO.Icon), paymentChannelEntity.Icon),
            };

            // Loop through image properties and update paths if necessary
            foreach (var (paymentChannelFormDTOProperty, paymentChannelEntityFile) in imageProperties)
            {
                var dtoValue = typeof(PaymentChannelFormDTO)
                    .GetProperty(paymentChannelFormDTOProperty)
                    ?.GetValue(paymentChannelFormDTO)
                    ?.ToString();

                if (string.IsNullOrEmpty(dtoValue))
                {
                    typeof(PaymentChannelFormDTO)
                        .GetProperty(paymentChannelFormDTOProperty)
                        ?.SetValue(paymentChannelFormDTO, paymentChannelEntityFile);
                }
                else if (dtoValue != paymentChannelEntityFile)
                {
                    typeof(PaymentChannelFormDTO)
                        .GetProperty(paymentChannelFormDTOProperty)
                        ?.SetValue(
                            paymentChannelFormDTO,
                            $"payment-channel/{paymentChannelFormDTO.Id}{dtoValue}"
                        );
                }
            }
            PaymentChannelConverter.ConvertModelToEntity(
                paymentChannelFormDTO,
                ref paymentChannelEntity
            );
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while updating payment channel.");
            throw;
        }
    }

    public PaymentChannelInfoDTO GetPaymentChannelById(int id)
    {
        try
        {
            var paymentChannelEntity = _dbContext.PaymentChannels.FirstOrDefault(pc => pc.Id == id);
            if (paymentChannelEntity == null)
            {
                LoggerHelper.Instance.LogError($"Payment Channel with Id: {id} not found.");
                return null;
            }

            var paymentChannelInfoDTO = PaymentChannelConverter.ConvertEntityToModel(
                paymentChannelEntity,
                _mediaHostUrl
            );

            return paymentChannelInfoDTO;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                $"Error occurred while fetching payment channel with Id: {id}"
            );
            throw;
        }
    }

    public ResponseDTO<PaymentChannelPagingDTO> GetAllPaymentChannels(PageSortParam pageSortParam)
    {
        try
        {
            var query = _dbContext.PaymentChannels.AsQueryable();

            if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
            {
                query = query.Where(p =>
                    p.ChannelName.Contains(pageSortParam.SearchTerm)
                    || p.Description.Contains(pageSortParam.SearchTerm)
                );
            }

            int totalCount = query.Count();

            if (!string.IsNullOrEmpty(pageSortParam.SortField))
            {
                var param = Expression.Parameter(typeof(PaymentChannel), "p");
                var property = Expression.Property(param, pageSortParam.SortField);
                var sortExpression = Expression.Lambda(property, param);

                string sortMethod =
                    pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                var orderByMethod = typeof(Queryable)
                    .GetMethods()
                    .Where(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                    .Single()
                    .MakeGenericMethod(typeof(PaymentChannel), property.Type);
                query =
                    (IQueryable<PaymentChannel>)(
                        orderByMethod.Invoke(null, new object[] { query, sortExpression })
                        ?? Enumerable.Empty<PaymentChannel>().AsQueryable()
                    );
            }

            if (query.Count() > pageSortParam.PageSize)
            {
                query = query
                    .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                    .Take(pageSortParam.PageSize);
            }

            var paymentChannels = query
                .Select(channel =>
                    PaymentChannelConverter.ConvertEntityToModel(channel, _mediaHostUrl)
                )
                .ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSortParam.PageSize);
            var pagingResult = new PagingResult
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PreviousPage =
                    pageSortParam.CurrentPage > 1 ? pageSortParam.CurrentPage - 1 : (int?)null,
                NextPage =
                    pageSortParam.CurrentPage < totalPages
                        ? pageSortParam.CurrentPage + 1
                        : (int?)null,
                FirstRowOnPage = ((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize) + 1,
                LastRowOnPage = Math.Min(
                    totalCount,
                    pageSortParam.CurrentPage * pageSortParam.PageSize
                ),
            };
            
            ResponseDTO<PaymentChannelPagingDTO> responseDto = new ResponseDTO<PaymentChannelPagingDTO>();
            responseDto.StatusCode = (int)HttpStatusCode.OK;
            responseDto.Message = "Payment channels retrieved successfully";
            responseDto.TimeStamp = DateTime.Now;
            responseDto.Payload = new PaymentChannelPagingDTO { Paging = pagingResult, PaymentChannels = paymentChannels };
            return responseDto;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                "Error occurred while fetching all payment channels."
            );
            throw;
        }
    }

    public bool DeletePaymentChannel(int id)
    {
        try
        {
            var paymentChannelEntity = _dbContext.PaymentChannels.FirstOrDefault(pc => pc.Id == id);
            if (paymentChannelEntity == null)
                return false;

            _dbContext.PaymentChannels.Remove(paymentChannelEntity);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                $"Error occurred while deleting payment channel with Id: {id}"
            );
            throw;
        }
    }
}
