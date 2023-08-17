using Microsoft.AspNetCore.Mvc;

namespace Ohtic.Test.Api.Abstractions
{
    public interface IPagedController
    {
        Task<IActionResult> Get(
            int? pageNumber,
            int? pageSize,
            string? q
        );
    }
}