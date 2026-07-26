using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
// Removed Animals references
using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
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
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser.Ports;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant.Ports;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser.Ports;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers.Ports;
using ZooTech.Application.Modules.Module_Celo.Validators;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.Validators;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.DeleteOrdenio;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GetOrdenioById;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.ListOrdenios;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.UpdateOrdenio;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.Validators;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.DeleteTriaje;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTipoPesos;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTriajes;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllVacunosSanidad;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialByVacunoId;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetTriajeById;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.UpdateTriaje;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesExcel;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesPdf;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialGeneral;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDetalleTriajeByVacunoId;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetDetallesTriajeByVacunoId;
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
        services.AddScoped<ICreateTriajeInputPort, CreateTriajeInteractor>();
        services.AddScoped<ICreateTriajeBehaviorPipelineFactory, CreateTriajeBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<CreateTriajeCommand>, CreateTriajeValidator>();

        services.AddScoped<IDeleteTriajeInputPort, DeleteTriajeInteractor>();
        services.AddScoped<IDeleteTriajeBehaviorPipelineFactory, DeleteTriajeBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<DeleteTriajeCommand>, DeleteTriajeValidator>();

        services.AddScoped<IGetAllTipoPesosInputPort, GetAllTipoPesosInteractor>();
        services.AddScoped<IGetAllTipoPesosBehaviorPipelineFactory, GetAllTipoPesosBehaviorPipelineFactory>();

        services.AddScoped<IGetAllTriajesInputPort, GetAllTriajesInteractor>();
        services.AddScoped<IGetAllTriajesBehaviorPipelineFactory, GetAllTriajesBehaviorPipelineFactory>();

        services.AddScoped<IGetAllVacunosSanidadInputPort, GetAllVacunosSanidadInteractor>();
        services.AddScoped<IGetAllVacunosSanidadBehaviorPipelineFactory, GetAllVacunosSanidadBehaviorPipelineFactory>();

        services.AddScoped<IGetHistorialByVacunoIdInputPort, GetHistorialByVacunoIdInteractor>();
        services.AddScoped<IGetHistorialByVacunoIdBehaviorPipelineFactory, GetHistorialByVacunoIdBehaviorPipelineFactory>();

        services.AddScoped<IGetTriajeByIdInputPort, GetTriajeByIdInteractor>();
        services.AddScoped<IGetTriajeByIdBehaviorPipelineFactory, GetTriajeByIdBehaviorPipelineFactory>();

        services.AddScoped<IUpdateTriajeInputPort, UpdateTriajeInteractor>();
        services.AddScoped<IUpdateTriajeBehaviorPipelineFactory, UpdateTriajeBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<UpdateTriajeCommand>, UpdateTriajeValidator>();

        services.AddScoped<IGenerateTriajesExcelInputPort, GenerateTriajesExcelInteractor>();
        services.AddScoped<IGenerateTriajesExcelBehaviorPipelineFactory, GenerateTriajesExcelBehaviorPipelineFactory>();

        services.AddScoped<IGenerateTriajesPdfInputPort, GenerateTriajesPdfInteractor>();
        services.AddScoped<IGenerateTriajesPdfBehaviorPipelineFactory, GenerateTriajesPdfBehaviorPipelineFactory>();

        services.AddScoped<IGetHistorialGeneralInputPort, GetHistorialGeneralInteractor>();
        services.AddScoped<IGetHistorialGeneralBehaviorPipelineFactory, GetHistorialGeneralBehaviorPipelineFactory>();

        services.AddScoped<IGetDetallesTriajeByVacunoIdInputPort, GetDetallesTriajeByVacunoIdInteractor>();
        services.AddScoped<IGetDetallesTriajeByVacunoIdBehaviorPipelineFactory, GetDetallesTriajeByVacunoIdBehaviorPipelineFactory>();

        // ============================================
        // Use Cases - Module_ProduccionLeche
        // ============================================
        services.AddScoped<ICreateOrdenioInputPort, CreateOrdenioInteractor>();
        services.AddScoped<ICreateOrdenioBehaviorPipelineFactory, CreateOrdenioBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<CreateOrdenioCommand>, CreateOrdenioValidator>();
        
        services.AddScoped<IDeleteOrdenioInputPort, DeleteOrdenioInteractor>();
        services.AddScoped<IDeleteOrdenioBehaviorPipelineFactory, DeleteOrdenioBehaviorPipelineFactory>();
        services.AddScoped<ICommandQueryValidator<DeleteOrdenioCommand>, DeleteOrdenioValidator>();

        services.AddScoped<IGetOrdeniosExcelInputPort, GenerateOrdeniosExcelInteractor>();
        services.AddScoped<IGenerateOrdeniosExcelBehaviorPipelineFactory, GenerateOrdeniosExcelBehaviorPipelineFactory>();
        
        services.AddScoped<IGetOrdeniosPdfInputPort, GenerateOrdeniosPdfInteractor>();
        services.AddScoped<IGenerateOrdeniosPdfBehaviorPipelineFactory, GenerateOrdeniosPdfBehaviorPipelineFactory>();
        
        services.AddScoped<IGetOrdenioByIdInputPort, GetOrdenioByIdInteractor>();
        services.AddScoped<IGetOrdenioByIdBehaviorPipelineFactory, GetOrdenioByIdBehaviorPipelineFactory>();
        
        services.AddScoped<IListOrdeniosInputPort, ListOrdeniosInteractor>();
        services.AddScoped<IListOrdeniosBehaviorPipelineFactory, ListOrdeniosBehaviorPipelineFactory>();

        services.AddScoped<IUpdateOrdenioInputPort, UpdateOrdenioInteractor>();
        services.AddScoped<IUpdateOrdenioBehaviorPipelineFactory, UpdateOrdenioBehaviorPipelineFactory>();
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
