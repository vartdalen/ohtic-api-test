namespace Ohtic.Test.Services.Abstractions
{
    public interface ICrudService<
        TId,
        TCreateDto,
        TReadDto,
        TUpdateDto
    >
    {
        Task<TReadDto> Create(TCreateDto dto);
        Task<TReadDto> Read(TId id);
        Task Update(TId id, TUpdateDto dto);
        Task Delete(TId id);
    }
}
