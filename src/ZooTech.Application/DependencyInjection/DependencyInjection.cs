using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;
using Microsoft.Extensions.DependencyInjection;
// Removed Animals references
using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin;
using ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser;
using ZooTech.Application.Modules.Module_Celo.Validators;
using ZooTech.Application.Modules.Module_ProduccionLeche.Validators;
using ZooTech.Application.Modules.Module_Sanidad.Validators;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.Validators;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.DeleteVacuno;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ExportarArbolGenealogico;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.GetArbolGenealogico;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoCatalogs;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.GetVacunoCatalogs;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ListarVacunos;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.UpdateVacuno;
using ZooTech.Application.Modules.Module_Fecundacion.Validators;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;


namespace ZooTech.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // ============================================
        // Use Cases - Module_Sanidad
        // ============================================
        services.AddScoped<ICommandQueryValidator<CreateTriajeCommand>, CreateTriajeValidator>();

        services.AddScoped<ICommandQueryValidator<DeleteTriajeCommand>, DeleteTriajeValidator>();

        services.AddScoped<ICommandQueryValidator<UpdateTriajeCommand>, UpdateTriajeValidator>();

        // ============================================
        // Use Cases - Module_ProduccionLeche
        // ============================================
        services.AddScoped<ICommandQueryValidator<CreateOrdenioCommand>, CreateOrdenioValidator>();
        
        services.AddScoped<ICommandQueryValidator<DeleteOrdenioCommand>, DeleteOrdenioValidator>();

        services.AddScoped<ICommandQueryValidator<UpdateOrdenioCommand>, UpdateOrdenioValidator>();

        // ============================================
        // FluentValidation â€” all assemblies
        // ============================================
        // services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        // ============================================
        // Use Cases - Module_Celo
        // ============================================
        services.AddScoped<ICommandQueryValidator<CreateCeloCommand>, CreateCeloValidator>();

        services.AddScoped<ICommandQueryValidator<DeleteCeloCommand>, DeleteCeloValidator>();

        services.AddScoped<ICommandQueryValidator<UpdateCeloCommand>, UpdateCeloValidator>();

        services.AddScoped<ICommandQueryValidator<UpdateFecundacionEstadoCommand>, UpdateFecundacionEstadoValidator>();

        services.AddScoped<UpdateFecundacionEstadoValidator>();
        services.AddScoped<FecundacionEstadoTransitionValidator>();

        // Removed Animals Use Cases

        // ============================================
        // Use Cases - Module_Fecundacion
        // ============================================
        services.AddScoped<ICommandQueryValidator<CreateFecundacionCommand>, CreateFecundacionValidator>();

        services.AddScoped<ICommandQueryValidator<DeleteFecundacionCommand>, DeleteFecundacionValidator>();
                
        services.AddScoped<ICommandQueryValidator<ListarFecundacionQuery>, ListarFecundacionQueryValidator>();
        
        services.AddScoped<ICommandQueryValidator<UpdateFecundacionCommand>, UpdateFecundacionValidator>();

        // ============================================
        // Use Cases - Module_Vacuno
        // ============================================
        services.AddScoped<ICreateVacunoInputPort, CreateVacunoInteractor>();
        services.AddScoped<ICreateVacunoBehaviorPipelineFactory, CreateVacunoBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<CreateVacunoCommand>, CreateVacunoValidator>();

        services.AddScoped<IDeleteVacunoInputPort, DeleteVacunoInteractor>();
        services.AddScoped<IDeleteVacunoBehaviorPipelineFactory, DeleteVacunoBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<DeleteVacunoCommand>, DeleteVacunoValidator>();
        
        services.AddScoped<IExportarArbolGenealogicoInputPort,ExportarArbolGenealogicoInteractor>();
        services.AddScoped<IExportarArbolGenealogicoBehaviorPipelineFactory, ExportarArbolGenealogicoBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<ExportarArbolGenealogicoCommand>, ExportarArbolGenealogicoCommandValidator>();

        services.AddScoped<IExportarActividadVacunosInputPort, ExportarActividadVacunosInteractor>();
        services.AddScoped<ICommandQueryValidator<ExportarActividadVacunosQuery>, ExportarActividadVacunosQueryValidator>();
        
        services.AddScoped<IGetArbolGenealogicoInputPort, GetArbolGenealogicoInteractor>();
        services.AddScoped<IGetArbolGenealogicoBehaviorPipelineFactory, GetArbolGenealogicoBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<GetArbolGenealogicoCommand>, GetArbolGenealogicoCommandValidator>();
        
        services.AddScoped<IGetVacunoByIdInputPort, GetVacunoByIdInteractor>();
        services.AddScoped<IGetVacunoByIdBehaviorPipelineFactory, GetVacunoByIdBehaviorPipelineFactory>();
                
        services.AddScoped<IGetVacunoCatalogsInputPort, GetVacunoCatalogsInteractor>();
        services.AddScoped<IGetVacunoCatalogsBehaviorPipelineFactory, GetVacunoCatalogsBehaviorPipelineFactory>();
        
        services.AddScoped<IListarVacunosInputPort, ListarVacunosInteractor>();
        services.AddScoped<IListarVacunosBehaviorPipelineFactory, ListarVacunosBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<ListarVacunosCommand>, ListarVacunosCommandValidator>();
        
        services.AddScoped<IGetActivityStatsInputPort, GetActivityStatsInteractor>();
        services.AddScoped<IVacunoActivityStatsService, VacunoActivityStatsService>();
        services.AddScoped<ICommandQueryValidator<GetActivityStatsQuery>, GetActivityStatsQueryValidator>();

        services.AddScoped(typeof(IReportStrategyResolver<>), typeof(ReportStrategyResolver<>));
        services.AddScoped<IVacunoReportFormatPolicy, VacunoReportFormatPolicy>();
        
        services.AddScoped<IListarVacunosReporteUseCase, ListarVacunosReporteUseCase>();
        services.AddScoped<IListarVacunosReporteBehaviorPipelineFactory, ListarVacunosReporteBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<ListarVacunosReporteQuery>, ListarVacunosReporteQueryValidator>();
        
        services.AddScoped<IObtenerRegistroVacunoReporteUseCase, ObtenerRegistroVacunoReporteUseCase>();
        services.AddScoped<IObtenerRegistroVacunoReporteBehaviorPipelineFactory, ObtenerRegistroVacunoReporteBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<ObtenerRegistroVacunoReporteQuery>, ObtenerRegistroVacunoReporteQueryValidator>();

        services.AddScoped<IUpdateVacunoInputPort, UpdateVacunoInteractor>();
        services.AddScoped<IUpdateVacunoBehaviorPipelineFactory, UpdateVacunoBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<UpdateVacunoCommand>, UpdateVacunoValidator>();

        // ============================================
        // Use Cases - Module_Auth
        // ============================================
        services.AddScoped<ICommandQueryValidator<AdminLoginCommand>, AdminLoginValidator>();
        services.AddScoped<ICommandQueryValidator<RegularLoginCommand>, RegularLoginValidator>();

        // ============================================
        // Use Cases - Module_Tenancing
        // ============================================                
        services.AddScoped<ICommandQueryValidator<CreateTenantCommand>, CreateTenantValidation>();
        services.AddScoped<ICommandQueryValidator<CreateUserInTenantCommand>, CreateUserInTenantValidator>();
        
        services.AddScoped<ICommandQueryValidator<DeleteAdminUserCommand>, DeleteAdminUserValidator>();
        services.AddScoped<ICommandQueryValidator<CreateAdminUserCommand> , CreateAdminUserValidator>();
        
        services.AddScoped(typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(AuditBehavior<,>));

        // Registro de Behaviors
        services.AddTransient(
            typeof(IBehavior<,>),
            typeof(ValidationBehavior<,>)
        );
        services.AddTransient(
            typeof(IBehavior<,>),
            typeof(LoggingBehavior<,>)
        );
        services.AddTransient(
            typeof(IBehavior<,>),
            typeof(AuditBehavior<,>)
        );

        // Registro de Interactors
        services.AddInteractors();

        // Registro de Dispatcher
        services.AddScoped<IBehaviorDispatcher , BehaviorDispatcher>();

        return services;
    }
}
