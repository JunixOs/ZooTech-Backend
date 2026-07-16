using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Mappers;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_ProduccionLeche.Mappers;

public class ProduccionLecheMapperTests
{
    [Fact]
    public void ToCommand_FromCreateOrdenioRequest_MapsAllFields()
    {
        var fechaHora = new DateTime(2026, 7, 7, 10, 30, 0);
        var request = new CreateOrdenioRequest(
            "ORD-001",
            fechaHora,
            10,
            20,
            12.5m,
            "ACTIVO",
            "Sin observaciones");

        var command = ProduccionLecheMapper.ToCommand(request);

        Assert.Equal(request.Codigo, command.Codigo);
        Assert.Equal(request.FechaHora, command.FechaHora);
        Assert.Equal(request.VacunoId, command.VacunoId);
        Assert.Equal(request.EncargadoUsuarioId, command.EncargadoUsuarioId);
        Assert.Equal(request.Litros, command.Litros);
        Assert.Equal(request.EstadoOrdenioCode, command.EstadoOrdenioCode);
        Assert.Equal(request.Observaciones, command.Observaciones);
    }

    [Fact]
    public void ToCommand_FromUpdateOrdenioRequest_MapsAllFields()
    {
        var fechaHora = new DateTime(2026, 7, 7, 11, 0, 0);
        var request = new UpdateOrdenioRequest(
            fechaHora,
            30,
            9.75m,
            "FINALIZADO",
            "Actualizado");

        var command = ProduccionLecheMapper.ToCommand(request);

        Assert.Equal(request.FechaHora, command.FechaHora);
        Assert.Equal(request.EncargadoUsuarioId, command.EncargadoUsuarioId);
        Assert.Equal(request.Litros, command.Litros);
        Assert.Equal(request.EstadoOrdenioCode, command.EstadoOrdenioCode);
        Assert.Equal(request.Observaciones, command.Observaciones);
    }

    [Fact]
    public void ToResponse_FromCreateOrdenioOutput_MapsAllFields()
    {
        var output = new CreateOrdenioOutput(CreateOrdenioOutputData());

        var response = ProduccionLecheMapper.ToResponse(output);

        Assert.Equal(output.Data.Id, response.Id);
        Assert.Equal(output.Data.Codigo, response.Codigo);
        Assert.Equal(output.Data.FechaHora, response.FechaHora);
        Assert.Equal(output.Data.VacunoId, response.VacunoId);
        Assert.Equal(output.Data.NombreVacuno, response.NombreVacuno);
        Assert.Equal(output.Data.EncargadoUsuarioId, response.EncargadoUsuarioId);
        Assert.Equal(output.Data.NombreCompleto, response.NombreEncargado);
        Assert.Equal(output.Data.Litros, response.Litros);
        Assert.Equal(output.Data.EstadoOrdenioCode, response.EstadoOrdenioCode);
        Assert.Equal(output.Data.Observaciones, response.Observaciones);
        Assert.Equal(output.Data.CreatedAt, response.CreatedAt);
        Assert.Equal(output.Data.UpdatedAt, response.UpdatedAt);
    }

    [Fact]
    public void ToResponse_FromListOrdeniosOutput_CalculatesPagination()
    {
        var output = new ListOrdeniosOutput(
            new[] { CreateOrdenioListOutputData() },
            TotalCount: 45);

        var response = ProduccionLecheMapper.ToResponse(output, page: 2, pageSize: 20);

        Assert.Single(response.Data);
        Assert.Equal(2, response.Pagination.Page);
        Assert.Equal(20, response.Pagination.Limit);
        Assert.Equal(45, response.Pagination.Total);
        Assert.Equal(3, response.Pagination.TotalPages);
    }

    private static OrdenioOutput CreateOrdenioOutputData()
    {
        var createdAt = new DateTime(2026, 7, 7, 10, 0, 0);
        var updatedAt = new DateTime(2026, 7, 7, 10, 5, 0);

        return new OrdenioOutput(
            1,
            "ORD-001",
            createdAt,
            10,
            "Luna",
            20,
            "Juan Perez",
            12.5m,
            "ACTIVO",
            "Sin observaciones",
            createdAt,
            updatedAt);
    }

    private static OrdenioListOutput CreateOrdenioListOutputData()
    {
        var createdAt = new DateTime(2026, 7, 7, 10, 0, 0);
        var updatedAt = new DateTime(2026, 7, 7, 10, 5, 0);

        return new OrdenioListOutput(
            1,
            "ORD-001",
            createdAt,
            10,
            "Luna",
            "VAC-001",
            20,
            "Juan Perez",
            12.5m,
            "ACTIVO",
            "Sin observaciones",
            createdAt,
            updatedAt);
    }
}
