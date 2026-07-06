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
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoCatalogs;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunosPaginado;


namespace ZooTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // ============================================
        // Use Cases - Module_Sanidad
        // ============================================
        services.AddScoped<IGetAllTriajesInputPort, GetAllTriajesInteractor>();
        services.AddScoped<IGetTriajeByIdInputPort, GetTriajeByIdInteractor>();
        services.AddScoped<ICreateTriajeInputPort, CreateTriajeInteractor>();
        services.AddScoped<IUpdateTriajeInputPort, UpdateTriajeInteractor>();
        services.AddScoped<IDeleteTriajeInputPort, DeleteTriajeInteractor>();
        services.AddScoped<IGetAllTipoPesosInputPort, GetAllTipoPesosInteractor>();
        services.AddScoped<IGetAllVacunosSanidadInputPort, GetAllVacunosSanidadInteractor>();
        services.AddScoped<IGetHistorialByVacunoIdInputPort, GetHistorialByVacunoIdInteractor>();
        services.AddScoped<IGetDetallesTriajeByVacunoIdInputPort, GetDetallesTriajeByVacunoIdInteractor>();
        services.AddScoped<IGenerateTriajesPdfInputPort, GenerateTriajesPdfInteractor>();
        services.AddScoped<IGenerateTriajesExcelInputPort, GenerateTriajesExcelInteractor>();
        services.AddScoped<IGetHistorialGeneralInputPort, GetHistorialGeneralInteractor>();

        // ============================================
        // Use Cases - Module_ProduccionLeche
        // ============================================
        services.AddScoped<ICreateOrdenioInputPort, CreateOrdenioInteractor>();
        services.AddScoped<IGetOrdeniosPdfInputPort, GenerateOrdeniosPdfInteractor>();
        services.AddScoped<IGetOrdeniosExcelInputPort, GenerateOrdeniosExcelInteractor>();
        services.AddScoped<IGetOrdenioByIdInputPort, GetOrdenioByIdInteractor>();
        services.AddScoped<IListOrdeniosInputPort, ListOrdeniosInteractor>();
        services.AddScoped<IUpdateOrdenioInputPort, UpdateOrdenioInteractor>();
        services.AddScoped<IDeleteOrdenioInputPort, DeleteOrdenioInteractor>();

        // ============================================
        // FluentValidation — all assemblies
        // ============================================
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        // ============================================
        // Use Cases - Module_Celo
        // ============================================
        services.AddScoped<IGetCelosInputPort, GetCelosInteractor>();
        services.AddScoped<IGetReporteCelosInputPort, GetReporteCelosInteractor>();
        services.AddScoped<IGetComparacionCelosRealVsEstandarInputPort, GetComparacionCelosRealVsEstandarInteractor>();
        services.AddScoped<IGetComparacionCelosRealVsEstandarPorVacunoInputPort, GetComparacionCelosRealVsEstandarPorVacunoInteractor>();
        services.AddScoped<IGetVacasEnCeloInputPort, GetVacasEnCeloInteractor>();
        services.AddScoped<IListCelosInputPort, ListCelosInteractor>();
        services.AddScoped<IListReporteCeloGeneralInputPort, ListReporteCeloGeneralInteractor>();
        services.AddScoped<ICreateCeloInputPort, CreateCeloInteractor>();
        services.AddScoped<IUpdateCeloInputPort, UpdateCeloInteractor>();
        services.AddScoped<IDeleteCeloInputPort, DeleteCeloInteractor>();
        services.AddScoped<IGetFecundacionEstadoInputPort, GetFecundacionEstadoInteractor>();
        services.AddScoped<IUpdateFecundacionEstadoInputPort, UpdateFecundacionEstadoInteractor>();
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
        services.AddScoped<IDeleteFecundacionInputPort, DeleteFecundacionInteractor>();
        services.AddScoped<IGetFecundacionForEditInputPort, GetFecundacionForEditInteractor>();
        services.AddScoped<IGetFecundacionOptionsInputPort, GetFecundacionOptionsInteractor>();
        services.AddScoped<ISearchFecundacionVacunosInputPort, SearchFecundacionVacunosInteractor>();
        services.AddScoped<IUpdateFecundacionInputPort, UpdateFecundacionInteractor>();

        // ============================================
        // Use Cases - Module_Vacuno
        // ============================================
        services.AddScoped<IListarVacunosInputPort, ListarVacunosInteractor>();
        services.AddScoped<ICreateVacunoInputPort, CreateVacunoInteractor>();
        services.AddScoped<IGetVacunoByIdInputPort, GetVacunoByIdInteractor>();
        services.AddScoped<IGetVacunoCatalogsInputPort, GetVacunoCatalogsInteractor>();
        services.AddScoped<IUpdateVacunoInputPort, UpdateVacunoInteractor>();
        services.AddScoped<IDeleteVacunoInputPort, DeleteVacunoInteractor>();
        services.AddScoped<IExportarArbolGenealogicoInputPort,ExportarArbolGenealogicoInteractor>();
        services.AddScoped<IGetArbolGenealogicoInputPort, GetArbolGenealogicoInteractor>();
        services.AddScoped<ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.IObtenerRegistroVacunoReporteUseCase, ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.ObtenerRegistroVacunoReporteUseCase>();
        services.AddScoped<IListarVacunosReporteUseCase, ListarVacunosReporteUseCase>();

        // ============================================
        // Use Cases - Module_Vacuno (listado paginado/genealogia)
        // ============================================
        services.AddScoped<ICreateFecundacionInputPort, CreateFecundacionInteractor>();
        services.AddScoped<IListarFecundacionInputPort, ListarFecundacionInteractor>();
        services.AddScoped<IListarVacunosPaginadoInputPort, ListarVacunosPaginadoInteractor>();
        services.AddScoped<IGenerarArbolGenealogicoInputPort, GenerarArbolGenealogicoInteractor>();
        services.AddScoped<ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico.IExportarArbolGenealogicoInputPort, ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico.ExportarArbolGenealogicoInteractor>();

        return services;
    }
}
