using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
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
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Behaviors.Module_Auth.AdminLogin;
using ZooTech.Application.Common.Behaviors.Module_Auth.RegularLogin;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateAdminUser;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateTenant;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateUserInTenant;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.DeleteAdminUser;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.ListAdminUsers;
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
using ZooTech.Application.Common.Behaviors.Module_Celo.CreateCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.DeleteCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.UpdateCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetCelos;
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
using ZooTech.Application.Common.Behaviors.Module_Celo.GetComparacionCelosRealVsEstandar;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetComparacionCelosRealVsEstandarPorVacuno;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetReporteCelos;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetVacasEnCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.ListCelos;
using ZooTech.Application.Common.Behaviors.Module_Celo.ListReporteCeloGeneral;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesExcel;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesPdf;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialGeneral;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDetalleTriajeByVacunoId;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetDetallesTriajeByVacunoId;
using ZooTech.Application.Common.Behaviors.Module_Celo.FecundacionEstado.UpdateFecundacionEstado;
using ZooTech.Application.Common.Behaviors.Module_Celo.FecundacionEstado.GetFecundacionEstado;
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
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.CreateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.Validators;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.DeleteFecundacion;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.GetFecundacionForEdit;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.GetFecundacionOptions;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.ListarFecundacion;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.SearchFecundacionVacunos;


namespace ZooTech.Application;

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
        services.AddScoped<ICommandValidator<CreateTriajeCommand>, CreateTriajeValidator>();

        services.AddScoped<IDeleteTriajeInputPort, DeleteTriajeInteractor>();
        services.AddScoped<IDeleteTriajeBehaviorPipelineFactory, DeleteTriajeBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<DeleteTriajeCommand>, DeleteTriajeValidator>();

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
        services.AddScoped<ICommandValidator<UpdateTriajeCommand>, UpdateTriajeValidator>();

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
        services.AddScoped<ICommandValidator<CreateOrdenioCommand>, CreateOrdenioValidator>();
        
        services.AddScoped<IDeleteOrdenioInputPort, DeleteOrdenioInteractor>();
        services.AddScoped<IDeleteOrdenioBehaviorPipelineFactory, DeleteOrdenioBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<DeleteOrdenioCommand>, DeleteOrdenioValidator>();

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
        services.AddScoped<ICommandValidator<UpdateOrdenioCommand>, UpdateOrdenioValidator>();

        // ============================================
        // FluentValidation â€” all assemblies
        // ============================================
        // services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        // ============================================
        // Use Cases - Module_Celo
        // ============================================
        services.AddScoped<IGetCelosInputPort, GetCelosInteractor>();
        services.AddScoped<IGetCelosBehaviorPipelineFactory, GetCelosBehaviorPipelineFactory>();

        services.AddScoped<IGetComparacionCelosRealVsEstandarInputPort, GetComparacionCelosRealVsEstandarInteractor>();
        services.AddScoped<IGetComparacionCelosRealVsEstandarBehaviorPipelineFactory, GetComparacionCelosRealVsEstandarBehaviorPipelineFactory>();
        
        services.AddScoped<IGetComparacionCelosRealVsEstandarPorVacunoInputPort, GetComparacionCelosRealVsEstandarPorVacunoInteractor>();
        services.AddScoped<IGetComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory, GetComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory>();

        services.AddScoped<IGetReporteCelosInputPort, GetReporteCelosInteractor>();
        services.AddScoped<IGetReporteCelosBehaviorPipelineFactory, GetReporteCelosBehaviorPipelineFactory>();

        services.AddScoped<IGetVacasEnCeloInputPort, GetVacasEnCeloInteractor>();
        services.AddScoped<IGetVacasEnCeloBehaviorPipelineFactory, GetVacasEnCeloBehaviorPipelineFactory>();

        services.AddScoped<IListCelosInputPort, ListCelosInteractor>();
        services.AddScoped<IListCelosBehaviorPipelineFactory, ListCelosBehaviorPipelineFactory>();

        services.AddScoped<IListReporteCeloGeneralInputPort, ListReporteCeloGeneralInteractor>();
        services.AddScoped<IListReporteCeloGeneralBehaviorPipelineFactory, ListReporteCeloGeneralBehaviorPipelineFactory>();

        services.AddScoped<ICreateCeloInputPort, CreateCeloInteractor>();
        services.AddScoped<ICreateCeloBehaviorPipelineFactory, CreateCeloBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<CreateCeloCommand>, CreateCeloValidator>();

        services.AddScoped<IDeleteCeloInputPort, DeleteCeloInteractor>();
        services.AddScoped<IDeleteCeloBehaviorPipelineFactory, DeleteCeloBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<DeleteCeloCommand>, DeleteCeloValidator>();

        services.AddScoped<IUpdateCeloInputPort, UpdateCeloInteractor>();
        services.AddScoped<IUpdateCeloBehaviorPipelineFactory, UpdateCeloBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<UpdateCeloCommand>, UpdateCeloValidator>();

        services.AddScoped<IUpdateFecundacionEstadoInputPort, UpdateFecundacionEstadoInteractor>();
        services.AddScoped<IUpdateFecundacionEstadoBehaviorPipelineFactory, UpdateFecundacionEstadoBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<UpdateFecundacionEstadoCommand>, UpdateFecundacionEstadoValidator>();

        services.AddScoped<IGetFecundacionEstadoInputPort, GetFecundacionEstadoInteractor>();
        services.AddScoped<IGetFecundacionEstadoBehaviorPipelineFactory, GetFecundacionEstadoBehaviorPipelineFactory>();

        services.AddScoped<UpdateFecundacionEstadoValidator>();
        services.AddScoped<FecundacionEstadoTransitionValidator>();

        // ============================================
        // Use Cases - Animals
        // ============================================
        services.AddScoped<DeleteAnimalValidator>();
        services.AddScoped<IDeleteAnimalInputPort, DeleteAnimalInteractor>();
        services.AddScoped<ReportAnimalListValidator>();
        services.AddScoped<IReportAnimalListInputPort, ReportAnimalListInteractor>();

        // ============================================
        // Use Cases - Module_Fecundacion
        // ============================================
        services.AddScoped<ICreateFecundacionInputPort, CreateFecundacionInteractor>();
        services.AddScoped<ICreateFecundacionBehaviorPipelineFactory, CreateFecundacionBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<CreateFecundacionCommand>, CreateFecundacionValidator>();

        services.AddScoped<IDeleteFecundacionInputPort, DeleteFecundacionInteractor>();
        services.AddScoped<IDeleteFecundacionBehaviorPipelineFactory, DeleteFecundacionBehaviorPipelineFactory>();

        services.AddScoped<IGetFecundacionForEditInputPort, GetFecundacionForEditInteractor>();
        services.AddScoped<IGetFecundacionForEditBehaviorPipelineFactory, GetFecundacionForEditBehaviorPipelineFactory>();
        
        services.AddScoped<IGetFecundacionOptionsInputPort, GetFecundacionOptionsInteractor>();
        services.AddScoped<IGetFecundacionOptionsBehaviorPipelineFactory, GetFecundacionOptionsBehaviorPipelineFactory>();
        
        services.AddScoped<IListarFecundacionInputPort, ListarFecundacionInteractor>();
        services.AddScoped<IListarFecundacionBehaviorPipelineFactory, ListarFecundacionBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<ListarFecundacionCommand>, ListarFecundacionCommandValidator>();

        services.AddScoped<ISearchFecundacionVacunosInputPort, SearchFecundacionVacunosInteractor>();
        services.AddScoped<ISearchFecundacionVacunosBehaviorPipelineFactory, SearchFecundacionVacunosBehaviorPipelineFactory>();
        
        services.AddScoped<IUpdateFecundacionInputPort, UpdateFecundacionInteractor>();
        services.AddScoped<IUpdateFecundacionEstadoBehaviorPipelineFactory, UpdateFecundacionEstadoBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<UpdateFecundacionCommand>, UpdateFecundacionValidator>();


        // ============================================
        // Use Cases - Module_Vacuno
        // ============================================
        services.AddScoped<ICreateVacunoInputPort, CreateVacunoInteractor>();
        services.AddScoped<ICreateVacunoBehaviorPipelineFactory, CreateVacunoBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<CreateVacunoCommand>, CreateVacunoValidator>();

        services.AddScoped<IDeleteVacunoInputPort, DeleteVacunoInteractor>();
        services.AddScoped<IDeleteVacunoBehaviorPipelineFactory, DeleteVacunoBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<DeleteVacunoCommand>, DeleteVacunoValidator>();
        
        services.AddScoped<IExportarArbolGenealogicoInputPort,ExportarArbolGenealogicoInteractor>();
        services.AddScoped<IExportarArbolGenealogicoBehaviorPipelineFactory, ExportarArbolGenealogicoBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<ExportarArbolGenealogicoCommand>, ExportarArbolGenealogicoCommandValidator>();
        
        services.AddScoped<IGetArbolGenealogicoInputPort, GetArbolGenealogicoInteractor>();
        services.AddScoped<IGetArbolGenealogicoBehaviorPipelineFactory, GetArbolGenealogicoBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<GetArbolGenealogicoCommand>, GetArbolGenealogicoCommandValidator>();
        
        services.AddScoped<IGetVacunoByIdInputPort, GetVacunoByIdInteractor>();
        services.AddScoped<IGetVacunoByIdBehaviorPipelineFactory, GetVacunoByIdBehaviorPipelineFactory>();
                
        services.AddScoped<IGetVacunoCatalogsInputPort, GetVacunoCatalogsInteractor>();
        services.AddScoped<IGetVacunoCatalogsBehaviorPipelineFactory, GetVacunoCatalogsBehaviorPipelineFactory>();
        
        services.AddScoped<IListarVacunosInputPort, ListarVacunosInteractor>();
        services.AddScoped<IListarVacunosBehaviorPipelineFactory, ListarVacunosBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<ListarVacunosCommand>, ListarVacunosCommandValidator>();
        
        services.AddScoped<IListarVacunosReporteUseCase, ListarVacunosReporteUseCase>();
        services.AddScoped<IListarVacunosReporteBehaviorPipelineFactory, ListarVacunosReporteBehaviorPipelineFactory>();
        
        services.AddScoped<IObtenerRegistroVacunoReporteUseCase, ObtenerRegistroVacunoReporteUseCase>();
        services.AddScoped<IObtenerRegistroVacunoReporteBehaviorPipelineFactory, ObtenerRegistroVacunoReporteBehaviorPipelineFactory>();

        services.AddScoped<IUpdateVacunoInputPort, UpdateVacunoInteractor>();
        services.AddScoped<IUpdateVacunoBehaviorPipelineFactory, UpdateVacunoBehaviorPipelineFactory>();
        services.AddScoped<ICommandValidator<UpdateVacunoCommand>, UpdateVacunoValidator>();

        // ============================================
        // Use Cases - Module_Auth
        // ============================================
        services.AddScoped<IAdminLoginBehaviorPipelineFactory, AdminLoginBehaviorPipelineFactory>();
        services.AddScoped<IRegularLoginBehaviorPipelineFactory, RegularLoginBehaviorPipelineFactory>();

        services.AddScoped<IAdminLoginInputPort, AdminLoginInteractor>();
        services.AddScoped<IRegularLoginInputPort, RegularLoginInteractor>();

        services.AddScoped<ICommandValidator<AdminLoginCommand>, AdminLoginValidator>();
        services.AddScoped<ICommandValidator<RegularLoginCommand>, RegularLoginValidator>();


        // ============================================
        // Use Cases - Module_Tenancing
        // ============================================
        services.AddScoped<ICreateTenantBehaviorPipelineFactory, CreateTenantBehaviorPipelineFactory>();
        services.AddScoped<ICreateUserInTenantBehaviorPipelineFactory, CreateUserInTenantBehaviorPipelineFactory>();

        services.AddScoped<IDeleteAdminUserBehaviorPipelineFactory, DeleteAdminUserBehaviorPipelineFactory>();
        services.AddScoped<ICreateAdminUserBehaviorPipelineFactory, CreateAdminUserBehaviorPipelineFactory>();
        services.AddScoped<IListAdminUsersBehaviorPipelineFactory, ListAdminUsersBehaviorPipelineFactory>();

        services.AddScoped<ICreateTenantInputPort, CreateTenantInteractor>();
        services.AddScoped<ICreateUserInTenantInputPort, CreateUserInTenantInteractor>();
        
        services.AddScoped<IDeleteAdminUserInputPort, DeleteAdminUserInteractor>();
        services.AddScoped<ICreateAdminUserInputPort , CreateAdminUserInteractor>();
        services.AddScoped<IListAdminUsersInputPort, ListAdminUsersInteractor>();
        
        services.AddScoped<ICommandValidator<CreateTenantCommand>, CreateTenantValidation>();
        services.AddScoped<ICommandValidator<CreateUserInTenantCommand>, CreateUserInTenantValidator>();
        
        services.AddScoped<ICommandValidator<DeleteAdminUserCommand>, DeleteAdminUserValidator>();
        services.AddScoped<ICommandValidator<CreateAdminUserCommand> , CreateAdminUserValidator>();
        
        // Registro de Behaviors
        services.AddTransient(typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(AuditBehavior<,>));

        return services;
    }
}
