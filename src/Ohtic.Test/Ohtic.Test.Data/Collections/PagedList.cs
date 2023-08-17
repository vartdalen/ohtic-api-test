using Microsoft.EntityFrameworkCore;

namespace Ohtic.Test.Data.Collections
{
    public class PagedList<T> : List<T>
    {
        public PagedList() { }

        public PagedList(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            AddRange(items);
        }

        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool IsPreviousPageExistent => PageNumber > 1;
        public bool IsNextPageExistent => PageNumber < TotalPages;

        public static PagedList<T> Empty()
        {
            return new PagedList<T>(new List<T>(), 0, 0, 0);
        }

        public static async Task<PagedList<T>> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var totalCount = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, pageNumber, pageSize, totalCount);
        }
    }
}
