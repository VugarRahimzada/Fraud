using AutoMapper;
using FluentValidation;
using Fraud.Core.Interfaces;
using Fraud.DataAccess;
using Fraud.DataAccess.Interceptors;
using Fraud.DataAccess.Repositories;
using Fraud.Service;
using Fraud.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fraud.Controllers.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, string connectionString)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<SqlCommandInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(sp.GetRequiredService<SqlCommandInterceptor>());
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICardRepository, CardRepository>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICardService, CardService>();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<Fraud.Service.Mapping.CardMappingProfile>();
            });

            services.AddValidatorsFromAssembly(typeof(Fraud.Service.Mapping.CardMappingProfile).Assembly);

            return services;
        }
    }
}