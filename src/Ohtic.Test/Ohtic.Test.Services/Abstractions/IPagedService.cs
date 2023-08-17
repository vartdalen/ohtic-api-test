using Ohtic.Test.Data.Collections;

namespace Ohtic.Test.Services.Abstractions
{
    public interface IPagedService<TReadDto>
    {
        Task<PagedList<TReadDto>> Read(
            int pageNumber,
            int pageSize,
            string? query = null
        );
    }
}