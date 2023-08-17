using Ohtic.Test.Data.Collections;

namespace Ohtic.Test.Abstractions.Repositories
{
	public interface IPagedQueryRepository<T>
    {
        Task<PagedList<T>> Read(
            int pageNumber,
            int pageSize,
            string? q
        );
    }
}
