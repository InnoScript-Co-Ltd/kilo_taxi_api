﻿using System.Linq.Expressions;
using System.Net;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.DataAccess.Implementation;

public class TravelRateRepository : ITravelRateRepository
{
    
    private readonly DbKiloTaxiContext _DbKiloTaxiContext;
    public TravelRateRepository(DbKiloTaxiContext DbKiloContext)
    {
        _DbKiloTaxiContext = DbKiloContext;
    }
    
    public TravelRateInfoDTO AddTravelRate(TravelRateFormDTO travelRateFormDto)
    {
        try
        {
            TravelRate travelRateEntity = new TravelRate();
            TravelRateConverter.ConvertModelToEntity(travelRateFormDto, ref travelRateEntity);
            _DbKiloTaxiContext.Add(travelRateEntity);
            _DbKiloTaxiContext.SaveChanges();
            travelRateFormDto.Id = travelRateEntity.Id;
                
            LoggerHelper.Instance.LogInfo($"TravelRate added successfully with Id: {travelRateEntity.Id}");

            var travelRateInfoDTO = TravelRateConverter.ConvertEntityToModel(travelRateEntity);
            return travelRateInfoDTO;

        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while adding travelRate.");
            throw;
        }
    }

    public bool UpdateTravelRate(TravelRateFormDTO travelRateFormDto)
    {
        bool result = false;
        try
        {
            TravelRate travelRateEntity = _DbKiloTaxiContext.TravelRates.FirstOrDefault(x => x.Id == travelRateFormDto.Id);
            if (travelRateEntity == null)
            {
                return result;
            }
            TravelRateConverter.ConvertModelToEntity(travelRateFormDto, ref travelRateEntity);
            _DbKiloTaxiContext.SaveChanges();
            result = true;

        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while updating country with Id: {travelRateFormDto.Id}");
            throw;
        }
        return result;
    }

    public ResponseDTO<TravelRatePagingDTO> GetAllTravelRate(PageSortParam pageSortParam)
    {
        try
            {
                // Base query with includes for related data
                var query = _DbKiloTaxiContext.TravelRates
                    .Include(t=>t.VehicleType)
                    .Include(c=>c.City)
                    .AsQueryable();

                // Filtering based on search term
                if (!string.IsNullOrWhiteSpace(pageSortParam.SearchTerm))
                {
                    query = query.Where(c => c.Unit.Contains(pageSortParam.SearchTerm));
                }
                var totalCount = query.Count();

                // Sorting using Dynamic LINQ
                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(TravelRate), "p");
                    var property = Expression.Property(param, pageSortParam.SortField);  // Get the property dynamically
                    var sortExpression = Expression.Lambda(property, param);

                    // Apply sorting using reflection based on SortDirection
                    string sortMethod = pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                    var orderByMethod = typeof(Queryable).GetMethods()
                        .Where(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                        .Single()
                        .MakeGenericMethod(typeof(TravelRate), property.Type);

                    query = (IQueryable<TravelRate>)orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query.Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                                 .Take(pageSortParam.PageSize);
                }
                // Applying pagination
                var travelRates = query.Select(c => TravelRateConverter.ConvertEntityToModel(c)).ToList();


                // Create the paging result
                var pagingResult = new PagingResult
                {
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSortParam.PageSize),
                    PreviousPage = pageSortParam.CurrentPage > 1 ? (int?)pageSortParam.CurrentPage - 1 : null,
                    NextPage = pageSortParam.CurrentPage < (int)Math.Ceiling(totalCount / (double)pageSortParam.PageSize) ? (int?)pageSortParam.CurrentPage + 1 : null,
                    FirstRowOnPage = (pageSortParam.CurrentPage - 1) * pageSortParam.PageSize + 1,
                    LastRowOnPage = Math.Min(pageSortParam.CurrentPage * pageSortParam.PageSize, totalCount)
                };

                ResponseDTO<TravelRatePagingDTO> responseDto = new ResponseDTO<TravelRatePagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "travel rates retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new TravelRatePagingDTO() { Paging = pagingResult, TravelRates = travelRates };
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all travel rates.");
                throw;
            }
    }

    public bool DeleteTravelRate(int id)
    {
        bool result = false;
        try
        {
            var travelRateEntity = _DbKiloTaxiContext.TravelRates.FirstOrDefault(x => x.Id == id);
            if (travelRateEntity == null)
            {
                return result;
            }
            _DbKiloTaxiContext.TravelRates.Remove(travelRateEntity);
            _DbKiloTaxiContext.SaveChanges();
            result = true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while deleting travel rate with Id: {id}");
            throw;
        }
        return result;
    }

    public TravelRateInfoDTO GetTravelRate(int id)
    {
        try
        {
            return TravelRateConverter.ConvertEntityToModel(_DbKiloTaxiContext.TravelRates.Include(t=>t.City).Include(t=>t.VehicleType).FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while fetching travelRate with Id: {id}");
            throw;
        }
    }
}