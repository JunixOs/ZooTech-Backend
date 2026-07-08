using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Export;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Domain.Common.Interfaces;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Common.Export;
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
using ZooTech.Domain.Module_Fecundacion.Interfaces;
using ZooTech.Infrastructure.Persistence.Modules.Module_Fecundacion.Repositories;
using ZooTech.Infrastructure.Persistence.Repositories;
using ZooTech.Infrastructure.Repositories;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Infrastructure.Reports;
using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        // ============================================
        // Transversal
        // ============================================

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IExcelDocumentGenerator, ExcelDocumentGenerator>();
        services.AddScoped<IPdfDocumentGenerator, PdfDocumentGenerator>();
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
        
        var garnetConnectionString = configuration["Garnet:ConnectionString"];
        if (string.IsNullOrWhiteSpace(garnetConnectionString))
        {
            garnetConnectionString = "localhost,abortConnect=false";
        }
        else if (!garnetConnectionString.Contains("abortConnect="))
        {
            garnetConnectionString = garnetConnectionString.Contains(";") || garnetConnectionString.Contains(",")
                ? garnetConnectionString + ",abortConnect=false"
                : garnetConnectionString + ",abortConnect=false";
        }

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

        services.AddScoped<IEstadoRegistroRepository, EstadoRegistroRepository>();

        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IAnimalReportRepository, AnimalReportRepository>();
        services.AddScoped<IAnimalReportExcelService, AnimalReportExcelService>();
        services.AddScoped<IAnimalReportPdfService, AnimalReportPdfService>();

        // Repositorios Fecundación
        services.AddScoped<IFecundacionRepository, FecundacionRepository>();
        services.AddScoped<IFecundacionEstadoRepository, FecundacionEstadoRepository>();

        // Repositorios Vacuno (Read/Write Models y UoW)
        services.AddScoped<IVacunoListadoReadRepository, VacunoListadoReadRepository>();
        services.AddScoped<IVacunoActivityStatsReadRepository, VacunoActivityStatsReadRepository>();
        services.AddScoped<IVacunoGranjaReadRepository, VacunoGranjaReadRepository>();
        services.AddScoped<IVacunoMutationUnitOfWork, VacunoMutationUnitOfWork>();
        services.AddScoped<IVacunoReferenceReadRepository, VacunoReferenceReadRepository>();
        services.AddScoped<IVacunoResponseReadRepository, VacunoResponseReadRepository>();
        services.AddScoped<IListadoVacunosReporteReadRepository, ListadoVacunosReporteReadRepository>();
        services.AddScoped<IRegistroVacunoReadRepository, RegistroVacunoReadRepository>();

        // Servicios de Exportación y Reportes
        services.AddScoped<ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.IRegistroVacunoExcelReportService, RegistroVacunoExcelReportService>();
        services.AddScoped<ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.IRegistroVacunoPdfReportService, RegistroVacunoPdfReportService>();
        services.AddScoped<ZooTech.Application.Common.Gateway.Services.IArbolGenealogicoExportService, ZooTech.Infrastructure.Reports.Vacunos.ArbolGenealogicoExcelExportService>();

        return services;
    }
}
