using System.Linq.Expressions;
using System.Net;
using BCrypt.Net;
using KiloTaxi.Common.Enums;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.DataAccess.Implementation
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public AdminRepository(DbKiloTaxiContext dbKiloTaxiContext)
        {
            _dbKiloTaxiContext = dbKiloTaxiContext;
        }

        public ResponseDTO<AdminPagingDTO> GetAllAdmin(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.Admins.AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(admin =>
                        admin.Name.Contains(pageSortParam.SearchTerm)
                        || admin.Email.Contains(pageSortParam.SearchTerm)
                        || admin.Phone.Contains(pageSortParam.SearchTerm)
                    );
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(Admin), "admin");
                    var property = Expression.Property(param, pageSortParam.SortField);
                    var sortExpression = Expression.Lambda(property, param);

                    var sortMethod =
                        pageSortParam.SortDir == SortDirection.ASC
                            ? "OrderBy"
                            : "OrderByDescending";
                    var orderByMethod = typeof(Queryable)
                        .GetMethods()
                        .Where(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                        .Single()
                        .MakeGenericMethod(typeof(Admin), property.Type);

                    query =
                        (IQueryable<Admin>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var admins = query.Select(AdminConverter.ConvertEntityToModel).ToList();

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

                // return new AdminPagingDTO { Paging = pagingResult, Admins = admins };
                ResponseDTO<AdminPagingDTO> responseDto = new ResponseDTO<AdminPagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "Admins retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new AdminPagingDTO { Paging = pagingResult, Admins = admins };
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all admins.");
                throw;
            }
        }

        // public AdminDTO AddAdmin(AdminDTO adminDTO)
        // {
        //     try
        //     {
        //         Admin adminEntity = new Admin();
        //         adminDTO.Password = BCrypt.Net.BCrypt.HashPassword(adminDTO.Password);

        //         AdminConverter.ConvertModelToEntity(adminDTO, ref adminEntity);

        //         _dbKiloTaxiContext.Add(adminEntity);
        //         _dbKiloTaxiContext.SaveChanges();

        //         adminDTO.Id = adminEntity.Id;

        //         LoggerHelper.Instance.LogInfo(
        //             $"Admin added successfully with Id: {adminEntity.Id}"
        //         );

        //         return adminDTO;
        //     }
        //     catch (Exception ex)
        //     {
        //         LoggerHelper.Instance.LogError(ex, "Error occurred while adding admin.");
        //         throw;
        //     }
        // }

        public bool UpdateAdmin(AdminFormDTO adminFormDTO)
        {
            try
            {
                var adminEntity = _dbKiloTaxiContext.Admins.FirstOrDefault(admin =>
                    admin.Id == adminFormDTO.Id
                );
                adminFormDTO.Password=BCrypt.Net.BCrypt.HashPassword(adminFormDTO.Password);
                if (adminEntity == null)
                {
                    return false;
                }

                AdminConverter.ConvertModelToEntity(adminFormDTO, ref adminEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating admin with Id: {adminFormDTO.Id}"
                );
                throw;
            }
        }

        public AdminInfoDTO GetAdminById(int id)
        {
            try
            {
                var adminEntity = _dbKiloTaxiContext.Admins.FirstOrDefault(admin => admin.Id == id);
                if (adminEntity == null)
                {
                    LoggerHelper.Instance.LogError($"Admin with Id: {id} not found.");
                    return null;
                }

                return AdminConverter.ConvertEntityToModel(adminEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching admin with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteAdmin(int id)
        {
            try
            {
                var adminEntity = _dbKiloTaxiContext.Admins.FirstOrDefault(admin => admin.Id == id);
                if (adminEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.Admins.Remove(adminEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting admin with Id: {id}"
                );
                throw;
            }
        }

        public AdminInfoDTO AdminRegistration(AdminFormDTO adminFormDTO)
        {
            try
            {
                // Initialize Admin entity
                Admin adminEntity = new Admin();

                // Set created date and hash the password
                adminFormDTO.Password = BCrypt.Net.BCrypt.HashPassword(adminFormDTO.Password);

                // Convert DTO to entity
                AdminConverter.ConvertModelToEntity(adminFormDTO, ref adminEntity);

                // Save admin to the database
                _dbKiloTaxiContext.Add(adminEntity);
                _dbKiloTaxiContext.SaveChanges();
                adminFormDTO.Id = adminEntity.Id;

                // Generate AdminInfoDTO
                var adminInfoDTO = AdminConverter.ConvertEntityToModel(adminEntity);
                return adminInfoDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while registering admin.");
                throw;
            }
        }

        public async Task<AdminInfoDTO> ValidateAdminCredentials(
            string EmailOrPhone,
            string password
        )
        {
            if (string.IsNullOrEmpty(EmailOrPhone) || string.IsNullOrEmpty(password))
            {
                return null; // Or throw an exception depending on your use case
            }

            Admin adminEntity = _dbKiloTaxiContext.Admins.SingleOrDefault(admin =>
                admin.Email == EmailOrPhone
            );
            var adminFormDTO = AdminConverter.ConvertEntityToModel(adminEntity);
            if (adminEntity != null || !BCrypt.Net.BCrypt.Verify(password, adminEntity.Password))
            {
                return adminFormDTO;
            }

            // Convert the entity to a DTO
            return null;
        }
    }
}
