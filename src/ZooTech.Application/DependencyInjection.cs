using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

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

        // ============================================
        // Use Cases - Module_ProduccionLeche
        // ============================================
        services.AddScoped<ICreateOrdenioInputPort, CreateOrdenioInteractor>();
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
        services.AddScoped<ICreateCeloInputPort, CreateCeloInteractor>();
        services.AddScoped<IUpdateCeloInputPort, UpdateCeloInteractor>();
        services.AddScoped<IDeleteCeloInputPort, DeleteCeloInteractor>();

        // ============================================
        // Use Cases - Module_Vacuno
        // ============================================
        services.AddScoped<IListarVacunosInputPort, ListarVacunosInteractor>();
        services.AddScoped<ICreateVacunoInputPort, CreateVacunoInteractor>();
        services.AddScoped<IGetVacunoByIdInputPort, GetVacunoByIdInteractor>();
        services.AddScoped<IUpdateVacunoInputPort, UpdateVacunoInteractor>();
        services.AddScoped<IDeleteVacunoInputPort, DeleteVacunoInteractor>();

        // ============================================
        // Use Cases - Module_ReporteVacuno
        // ============================================
<<<<<<< HEAD
        services.AddScoped<ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarReportesDisponibles.IListarReportesDisponiblesUseCase, ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarReportesDisponibles.ListarReportesDisponiblesUseCase>();
        services.AddScoped<ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.IObtenerRegistroVacunoReporteUseCase, ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.ObtenerRegistroVacunoReporteUseCase>();
=======
        services.AddScoped<ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles.IListarReportesDisponiblesUseCase, ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles.ListarReportesDisponiblesUseCase>();
        services.AddScoped<ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos.IListarReporteVacunosUseCase, ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos.ListarReporteVacunosUseCase>();
        services.AddScoped<ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte.IObtenerRegistroVacunoReporteUseCase, ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte.ObtenerRegistroVacunoReporteUseCase>();
>>>>>>> origin/LEONARDO_PARDO/feature

        return services;
    }
}
