namespace Ohtic.Test.Data.Extensions
{
    internal static class IQueryableExtensions
    {
        internal static IQueryable<TSource> Conditional<TSource>(
            this IQueryable<TSource> source,
            bool condition,
            Func<IQueryable<TSource>, IQueryable<TSource>> predicate)
        {
            if (condition)
            {
                return predicate(source);
            }
            else
            {
                return source;
            }
        }
    }
}
