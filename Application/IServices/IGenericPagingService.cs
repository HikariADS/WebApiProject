using System.Linq.Expressions;
using WebApiProject.Application.DTOs.Paging;

namespace WebApiProject.Application.IServices
{
    public interface IGenericPagingService<TEntity, TDto>
        where TEntity : class
        where TDto : class, new()
    {
        Task<PageResult<TDto>> PagingAsync(
            IQueryable<TEntity> query,
            PageRequest request,
            params Expression<Func<TEntity, object>>[] searchFields
        );
    }
}
