using Fraud.Controllers.Extensions;
using Fraud.Controllers.Middleware;
using Fraud.Core.Interfaces;
using Fraud.DataAccess;
using Fraud.DataAccess.Repositories;
using Fraud.Service;
using Fraud.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using Fraud.Core.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Fraud
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

            #region Serilog Configuration

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                // 1) TXT fayla yazır — hər gün yeni fayl, 30 gün saxlanılır
                .WriteTo.File(
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}{NewLine}")
                // 2) SQL Server-ə yazır — cədvəl avtomatik yaranır
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "ErrorLogs",
                        AutoCreateSqlTable = true,
                        BatchPostingLimit = 1 // xəta baş verən kimi dərhal yazılsın
                    },
                    columnOptions: new ColumnOptions
                    {
                        AdditionalColumns = new System.Collections.ObjectModel.Collection<SqlColumn>
                        {
                            new SqlColumn("SourceLocation", System.Data.SqlDbType.NVarChar, dataLength: 500),
                            new SqlColumn("SqlQuery", System.Data.SqlDbType.NVarChar, dataLength: -1), // MAX
                            new SqlColumn("RequestPath", System.Data.SqlDbType.NVarChar, dataLength: 300),
                            new SqlColumn("StatusCode", System.Data.SqlDbType.Int)
                        }
                    })
                .CreateLogger();

            builder.Host.UseSerilog();

            #endregion

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICardService, CardService>();
            builder.Services.AddScoped<ICardRepository, CardRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddApplicationServices();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();


            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                ));
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<Fraud.Service.Mapping.CommonMappingProfile>();
                cfg.AddProfile<Fraud.Service.Mapping.CardMappingProfile>();
                cfg.AddProfile<Fraud.Service.Mapping.UserMappingProfile>();
            });

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}