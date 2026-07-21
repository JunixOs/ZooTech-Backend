using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

namespace ZooTech.InterfaceAdapters.UnitTests.ApiContract;

public class OfficialRouteContractTests
{
    [Fact]
    public void DeleteVacuno_ShouldUseOfficialSwaggerRoute()
    {
        var method = typeof(VacunoController).GetMethod(nameof(VacunoController.Delete));

        method.Should().NotBeNull();
        var templates = method!
            .GetCustomAttributes<HttpDeleteAttribute>()
            .Select(attribute => attribute.Template)
            .ToArray();

        templates.Should().Contain("{id:long}");
    }

    [Fact]
    public void ReporteListadoVacunos_ShouldUseOfficialSwaggerRoutes()
    {
        GetHttpGetTemplate(nameof(VacunoController.ReportesListado))
            .Should().Be("reportes");
        GetHttpGetTemplate(nameof(VacunoController.ReportesListadoExcel))
            .Should().Be("reportes/excel");
        GetHttpGetTemplate(nameof(VacunoController.ReportesListadoPdf))
            .Should().Be("reportes/pdf");
    }

    [Fact]
    public void FecundacionEstado_ShouldUseOfficialSwaggerRoutes()
    {
        var getMethod = typeof(FecundacionEstadoController).GetMethod(nameof(FecundacionEstadoController.GetEstado));
        var getTemplates = getMethod!
            .GetCustomAttributes<HttpGetAttribute>()
            .Select(attribute => attribute.Template)
            .ToArray();

        getTemplates.Should().BeEquivalentTo(
            "vacunos/fecundacion-estado",
            "vacunos/{vacunoId:long}/fecundacion-estado");

        var putMethod = typeof(FecundacionEstadoController).GetMethod(nameof(FecundacionEstadoController.UpdateEstado));
        putMethod.Should().NotBeNull();
        putMethod!.GetCustomAttribute<HttpPutAttribute>()!.Template
            .Should().Be("fecundaciones/{fecundacionId:long}/estado");
    }

    private static string? GetHttpGetTemplate(string methodName)
    {
        var method = typeof(VacunoController).GetMethod(methodName);
        method.Should().NotBeNull();
        return method!.GetCustomAttribute<HttpGetAttribute>()!.Template;
    }
}
