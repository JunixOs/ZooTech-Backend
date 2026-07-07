using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Common.Time;
using ZooTech.Infrastructure.Common.Services.PdfGenerator;
using ZooTech.Infrastructure.Common.Services.ExcelGenerator;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Gateway.Parametrization.Features;
using ZooTech.Application.Common.Gateway.Parametrization.Rules;
using ZooTech.Application.Common.Gateway.Parametrization.Settings;
using ZooTech.Application.Common.Gateway.Repositories.GanaderiaDb;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Application.Common.Gateway.Tenant;

using ZooTech.Infrastructure.Auditing.MongoDb;
using ZooTech.Infrastructure.Caching;
using ZooTech.Infrastructure.Identity;
using ZooTech.Infrastructure.Parametrization;
using ZooTech.Infrastructure.Parametrization.Features;
using ZooTech.Infrastructure.Parametrization.Rules;
using ZooTech.Infrastructure.Parametrization.Settings;
using ZooTech.Infrastructure.Persistence.Repositories.GanaderiaDb;
using ZooTech.Infrastructure.Persistence.Repositories.MainTenantsDb;
using ZooTech.Infrastructure.Tenant;
using ZooTech.Infrastructure.Context;
using MongoDB.Driver;
using ZooTech.Infrastructure.Caching.ConcurrentCache;

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontró ConnectionStrings:DefaultConnection.");

        services.AddDbContext<GanaderiaDbContext>(options =>
            options.UseSqlServer(connectionString));

        // ============================================
        // Transversal
        // ============================================

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
        services.AddScoped<IOrdeniosComparationPdfGeneratorService, PdfGenerateComparationService>();

        services.AddScoped<IExcelGeneratorService, ExcelGeneratorService>();
        // ============================================
        // Repositories
        // ============================================

        services.AddScoped<ICeloRepository, CeloRepository>();
        services.AddScoped<IOrdenioRepository, OrdenioRepository>();
        services.AddScoped<IVacunoRepository, VacunoRepository>();
        services.AddScoped<ITriajeRepository, TriajeRepository>();
        services.AddScoped<ITipoPesoRepository, TipoPesoRepository>();
        var garnetConnectionString = configuration["Garnet:ConnectionString"]
            ?? throw new InvalidOperationException("Garnet:ConnectionString no configurado");

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(garnetConnectionString));

        services.AddSingleton<GarnetCacheConnection>();
        services.AddScoped<IAppCacheService, GarnetCacheService>();
        services.AddSingleton<
            IConcurrentCache<string, DbContextOptions<GanaderiaDbContext>>,
            ConcurrentCache<string, DbContextOptions<GanaderiaDbContext>>
        >();
        services.AddSingleton<
            IConcurrentCache<string, DbContextOptions<TenantCatalogDb>>,
            ConcurrentCache<string, DbContextOptions<TenantCatalogDb>>
        >();

        services.AddScoped<ITenantStore, TenantStore>();
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
        services.AddScoped<ITenantDatabaseMigrator, TenantDatabaseMigrator>();
        services.AddScoped<ITenantDatabaseCreator , TenantDatabaseCreator>();
        services.AddScoped<ITenantParameterSynchronizer , TenantParameterSynchronizer>();
        services.AddScoped<IGanaderiaDbContextFactory, GanaderiaDbContextFactory>();
        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();

        services.AddSingleton<MongoClient>(_ =>
        {
            var connection = configuration["MongoDb:ConnectionString"];

            return new MongoClient(connection);
        });
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IAppAuditService, MongoDbAudit>();

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRefreshTokenRepository , RefreshTokenRepository>();

        // Unified Tenant Configuration (Phase 1-2)
        services.AddScoped<ITenantConfigurationRepository, TenantConfigurationRepository>();
        services.AddScoped<ITenantConfigurationProvider, TenantConfigurationProvider>();

        // Parametrization / Settings / Features / Rules (deprecated — use ITenantConfigurationProvider)
        services.AddScoped<ISettingsProvider, SettingsProvider>();
        services.AddScoped<IFeatureProvider, FeatureProvider>();
        services.AddScoped<IRuleProvider, RuleProvider>();

        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IRuleRepository, RuleRepository>();

        // Multi-Instance Sync (Phase 4)
        services.AddHostedService<ConfigInvalidationSubscriber>();

        // JWT
        var jwt = configuration.GetSection("Jwt");
        services.Configure<JwtSettings>(
            configuration.GetSection("Jwt")
        );
        services.AddScoped<IJwtService , JwtService>();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt["SecretKey"]!)
                    )
                };
            });

            services.AddAuthorization();
            services.AddScoped<IPasswordHasher , PasswordHasher>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService , CurrentUserService>();

        return services;
    }
}
