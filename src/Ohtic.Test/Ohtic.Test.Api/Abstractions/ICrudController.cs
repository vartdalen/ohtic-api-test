using Microsoft.AspNetCore.Mvc;

namespace Ohtic.Test.Api.Abstractions
{
    internal interface ICrudController<
        TCreateDto,
        TUpdateDto
    >
    {
        Task<IActionResult> Post(TCreateDto productDto);
        Task<IActionResult> Get(int id);
        Task<IActionResult> Patch(int id, TUpdateDto dto);
        Task<IActionResult> Delete(int id);
    }
}
