using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Application.DTOs.Paging;
using WebApiProject.Application.IServices;

namespace WebApiProject.Application.Services
{
    public class GenericPagingService<TEntity, TDto> 
    : IGenericPagingService<TEntity, TDto>
        where TEntity : class
        where TDto : class, new()
    {
        public async Task<PageResult<TDto>> PagingAsync(
            IQueryable<TEntity> query,
            PageRequest request,
            Expression<Func<TEntity, bool>>? filter = null,
            params Expression<Func<TEntity, object>>[] searchFields
        )
        {
            // ========== FILTER ==========
            if (filter != null)
                query = query.Where(filter);

            // ========== SEARCH ==========
            if (!string.IsNullOrWhiteSpace(request.Search) && searchFields?.Length > 0)
            {
                var term = request.Search.Trim();
                Expression<Func<TEntity, bool>>? combined = null;

                foreach (var field in searchFields)
                {
                    var toStringCall = Expression.Call(field.Body, "ToString", Type.EmptyTypes);
                    var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
                    var containsCall = Expression.Call(toStringCall, containsMethod!, Expression.Constant(term));

                    var lambda = Expression.Lambda<Func<TEntity, bool>>(containsCall, field.Parameters);

                    combined = combined == null
                        ? lambda
                        : CombineOr(combined, lambda);
                }

                if (combined != null)
                    query = query.Where(combined);
            }

            // ========== SORT ==========
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                var param = Expression.Parameter(typeof(TEntity), "x");
                var property = Expression.PropertyOrField(param, request.SortBy);
                var lambda = Expression.Lambda(property, param);

                query = ApplyOrdering(query, lambda, request.SortOrder == "desc");
            }

            // ========== PAGING ==========
            var total = await query.CountAsync();
            var skip = (request.PageNumber - 1) * request.PageSize;

            var data = await query.Skip(skip).Take(request.PageSize).ToListAsync();

            // ========== MAP ENTITY → DTO ==========
            var dtoList = data.Select(entity =>
            {
                var dto = new TDto();
                foreach (var prop in typeof(TDto).GetProperties())
                {
                    var source = typeof(TEntity).GetProperty(prop.Name);
                    if (source != null)
                        prop.SetValue(dto, source.GetValue(entity));
                }
                return dto;
            }).ToList();

            return new PageResult<TDto>
            {
                TotalItems = total,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        // ===== Helper: Combine OR =====
        private Expression<Func<TEntity, bool>> CombineOr(
            Expression<Func<TEntity, bool>> expr1,
            Expression<Func<TEntity, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(TEntity));

            var combined = Expression.OrElse(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter));

            return Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
        }

        // ===== Helper: Apply Sorting =====
        private IQueryable<TEntity> ApplyOrdering(
            IQueryable<TEntity> source,
            LambdaExpression keySelector,
            bool desc)
        {
            string methodName = desc ? "OrderByDescending" : "OrderBy";

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName
                    && m.GetParameters().Length == 2);

            var genericMethod = method.MakeGenericMethod(typeof(TEntity), keySelector.Body.Type);

            return (IQueryable<TEntity>)genericMethod.Invoke(null, new object[] { source, keySelector })!;
        }

        public Task<PageResult<TDto1>> PagingAsync<TEntity1, TDto1>(IQueryable<TEntity1> query, PageRequest request, Func<TEntity1, bool>? filter, Func<TEntity1, object> orderBy, Func<TEntity1, object>? thenBy = default)
            where TEntity1 : class
            where TDto1 : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
