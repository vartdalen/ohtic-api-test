using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ohtic.Test.Abstractions.Repositories;
using Ohtic.Test.Data;
using Ohtic.Test.Data.Abstractions.Repositories;
using Ohtic.Test.Data.Repositories;
using Ohtic.Test.OAuth.Api.Refit;
using Ohtic.Test.OAuth.Services;
using Ohtic.Test.OAuth.Services.Abstractions;
using Ohtic.Test.Products.Authorization.Handlers;
using Ohtic.Test.Products.Authorization.Requirements;
using Ohtic.Test.Products.Swagger.Filters;
using Ohtic.Test.Services;
using Ohtic.Test.Services.Abstractions;
using Refit;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Ohtic.Test.Products.Extensions
{
	internal static class IServiceCollectionExtensions
    {
        internal static void ConfigureServices(
            this IServiceCollection services,
            IConfiguration config,
			JsonSerializerOptions jsonOptions
		)
        {
			services.AddSingleton(provider => jsonOptions);
			services.AddSingleton<ITokenService, TokenService>();
			services.AddScoped<IAuthorizationHandler, AdminOrCustomerIdRouteValueMatchAuthorizationHandler>();
			services.AddScoped<IAuthorizationHandler, AdminOrCustomerEmailRouteValueMatchAuthorizationHandler>();
			services.AddScoped<IAuthorizationHandler, AdminOrWeatherForecastIdRouteValueMatchAuthorizationHandler>();
			services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
			services.AddScoped<ICustomerRepository, CustomerRepository>();
			services.AddScoped<IWeatherForecastService, WeatherForecastService>();
			services.AddScoped<ICustomerService, CustomerService>();

			services.AddRefitClient<IGoogleOAuthApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(config["AppSettings:OAuth:Google:TokenBaseUri"]!));
            services.AddRefitClient<IMicrosoftOAuthApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(config["AppSettings:OAuth:Microsoft:TokenBaseUri"]!));
        }

        internal static void ConfigureApi(
            this IServiceCollection services,
			JsonSerializerOptions jsonOptions
		)
        {
            services
				.AddControllers()
                .AddJsonOptions(c =>
                {
                    c.JsonSerializerOptions.Encoder = jsonOptions.Encoder;
					c.JsonSerializerOptions.WriteIndented = jsonOptions.WriteIndented;
				});

            services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ohtic.Test API", Version = "v1" });
                    c.OperationFilter<HttpResponseOperationFilter>();
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT bearer access_token",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOrCustomerIdRouteValueMatch", p => p.AddRequirements(new AdminOrCustomerIdRouteValueMatchRequirement()));
				options.AddPolicy("AdminOrCustomerEmailRouteValueMatch", p => p.AddRequirements(new AdminOrCustomerEmailRouteValueMatchRequirement()));
				options.AddPolicy("AdminOrWeatherForecastIdRouteValueMatch", p => p.AddRequirements(new AdminOrWeatherForecastIdRouteValueMatchRequirement()));
			});
		}

        internal static void ConfigureAuthentication(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddAuthentication("Cookie")
                .AddCookie("Cookie", options =>
                {
                    options.SlidingExpiration = true;
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return Task.CompletedTask;
                    };
                })
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = config["AppSettings:OAuth:Ohtic:Issuer"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = config["AppSettings:OAuth:Ohtic:Issuer"],
                        ValidAudiences = new string[]
                        {
                            config["AppSettings:OAuth:Google:Web:ClientId"]!,
							config["AppSettings:OAuth:Google:Android:ClientId"]!,
							config["AppSettings:OAuth:Microsoft:Web:ClientId"]!,
					        config["AppSettings:OAuth:Microsoft:Android:ClientId"]!,
						},
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Secrets:OAuth:Ohtic:SigningKey"]!))
                    };
                });
        }

        internal static void ConfigureData(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddDbContext<WeatherForecastsDbContext>(options =>
            {
                var connectionString = config.GetConnectionString("DbConnectionString");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WeatherForecastsDbContext>();
            dbContext.Database.Migrate();
        }
    }
}