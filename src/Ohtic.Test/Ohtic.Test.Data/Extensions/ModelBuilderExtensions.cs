using Microsoft.EntityFrameworkCore;
using Ohtic.Test.Data.Abstractions;

namespace Ohtic.Test.Data.Extensions
{
	internal static class ModelBuilderExtensions
    {
        internal static void ConfigureGenericProperties<T>(this ModelBuilder modelBuilder)
            where T : class, IIdentifiable, IAuditable
        {
            modelBuilder.Entity<T>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<T>()
                .Property(e => e.CreatedAt)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<T>()
                .Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}
