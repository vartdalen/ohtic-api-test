using Microsoft.EntityFrameworkCore;
using Ohtic.Test.Data.Extensions;
using Ohtic.Test.Data.Models.Entities;

namespace Ohtic.Test.Data
{
	public class WeatherForecastsDbContext : DbContext
    {
        public WeatherForecastsDbContext(DbContextOptions<WeatherForecastsDbContext> options)
            : base(options)
        {
        }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
		public DbSet<Customer> Customers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			ConfigureProperties(modelBuilder);
        }

        private static void ConfigureProperties(ModelBuilder modelBuilder)
        {
			modelBuilder.ConfigureGenericProperties<WeatherForecast>();
			modelBuilder.ConfigureGenericProperties<Customer>();

			modelBuilder.Entity<Customer>()
				.HasMany(c => c.WeatherForecasts)
				.WithOne(o => o.Customer)
				.HasForeignKey(o => o.CustomerId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<WeatherForecast>()
                .Property(p => p.Summary)
                .HasMaxLength(255);

			modelBuilder.Entity<Customer>()
				.Property(d => d.Email)
				.HasMaxLength(255);

			modelBuilder.Entity<WeatherForecast>()
                .ToTable("weather_forecasts", t => t.HasCheckConstraint("constraint_temperature_c", "`temperature_c` >= -273"));
        }
    }
}