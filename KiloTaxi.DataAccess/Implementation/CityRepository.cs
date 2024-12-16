using System.Linq.Expressions;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.DataAccess.Implementation;

public class CityRepository : ICityRepository
{
    private readonly DbKiloTaxiContext _DbKiloTaxiContext;
    public CityRepository(DbKiloTaxiContext DbKiloContext)
    {
        _DbKiloTaxiContext = DbKiloContext;
    }
    
        public CityDTO AddCity(CityDTO cityDTO)
        {
            try
            {
                City cityEntity = new City();
                CityConverter.ConvertModelToEntity(cityDTO, ref cityEntity);
                _DbKiloTaxiContext.Add(cityEntity);
                _DbKiloTaxiContext.SaveChanges();
                cityDTO.Id = cityEntity.Id;
                
                LoggerHelper.Instance.LogInfo($"City added successfully with Id: {cityEntity.Id}");

                return cityDTO;

            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding city.");
                throw;
            }
        }

        public CityPagingDTO GetAllCity(PageSortParam pageSortParam)
        {
            try
            {
                // Base query with includes for related data
                var query = _DbKiloTaxiContext.Cities
                    .AsQueryable();

                // Filtering based on search term
                if (!string.IsNullOrWhiteSpace(pageSortParam.SearchTerm))
                {
                    query = query.Where(c => c.Name.Contains(pageSortParam.SearchTerm));
                }
                var totalCount = query.Count();

                // Sorting using Dynamic LINQ
                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(City), "p");
                    var property = Expression.Property(param, pageSortParam.SortField);  // Get the property dynamically
                    var sortExpression = Expression.Lambda(property, param);

                    // Apply sorting using reflection based on SortDirection
                    string sortMethod = pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                    var orderByMethod = typeof(Queryable).GetMethods()
                        .Where(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                        .Single()
                        .MakeGenericMethod(typeof(City), property.Type);

                    query = (IQueryable<City>)orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query.Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                                 .Take(pageSortParam.PageSize);
                }
                // Applying pagination
                var cities = query.Select(c => CityConverter.ConvertEntityToModel(c)).ToList();


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

                // Return the paginated result with cities
                return new CityPagingDTO
                {
                    Paging = pagingResult,
                    Cities = cities,
                };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all cities.");
                throw;
            }
        }



        public bool UpdateCity(CityDTO cityDTO)
        {
            bool result = false;
            try
            {
                City cityEntity = _DbKiloTaxiContext.Cities.FirstOrDefault(x => x.Id == cityDTO.Id);
                if (cityEntity == null)
                {
                    return result;
                }
                CityConverter.ConvertModelToEntity(cityDTO, ref cityEntity);
                _DbKiloTaxiContext.SaveChanges();
                result = true;

            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, $"Error occurred while updating city with Id: {cityDTO.Id}");
                throw;
            }
            return result;

        }

        public bool DeleteCity(int id)
        {
            bool result = false;
            try
            {
                var cityEntity = _DbKiloTaxiContext.Cities.FirstOrDefault(x => x.Id == id);
                if (cityEntity == null)
                {
                    return result;
                }
                _DbKiloTaxiContext.Cities.Remove(cityEntity);
                _DbKiloTaxiContext.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, $"Error occurred while deleting city with Id: {id}");
                throw;
            }
            return result;
        }


        public CityDTO GetCity(int id)
        {
            try
            {
                return CityConverter.ConvertEntityToModel(_DbKiloTaxiContext.Cities.FirstOrDefault(x => x.Id == id));
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, $"Error occurred while fetching country with Id: {id}");
                throw;
            }
        }

       
    }

