using ZooTech.API.Models;
using ZooTech.API.Security;
using ZooTech.API.Services;

namespace ZooTech.API.Endpoints;

public static class VacunoEndpoints
{
    public static RouteGroupBuilder MapVacunoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/vacunos")
            .WithTags("Vacunos");

        group.MapGet("/", (
                string? search,
                int? page,
                int? pageSize,
                IVacunoRepository repository) =>
            {
                var response = repository.Search(search, page ?? 1, pageSize ?? 10);
                return Results.Ok(response);
            })
            .WithName("ListarVacunos")
            .WithSummary("Lista vacunos paginados para la vista principal.");

        group.MapGet("/{codigo}", (string codigo, IVacunoRepository repository) =>
            {
                var vacuno = repository.GetByCodigo(codigo);
                return vacuno is null ? Results.NotFound() : Results.Ok(vacuno);
            })
            .WithName("VerVacuno")
            .WithSummary("Obtiene el detalle completo de un vacuno por codigo.");

        group.MapPut("/{codigo}", (
                string codigo,
                UpdateVacunoRequest request,
                IVacunoRepository repository) =>
            {
                var updated = repository.Update(codigo, request);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            })
            .WithName("EditarVacuno")
            .WithSummary("Actualiza los datos visibles en la pantalla Ver Vacuno.");

        group.MapDelete("/{codigo}", (string codigo, IVacunoRepository repository) =>
            {
                return repository.Delete(codigo)
                    ? Results.NoContent()
                    : Results.NotFound();
            })
            .WithName("EliminarVacuno")
            .WithSummary("Elimina logicamente un vacuno de la lista.");

        group.MapGet("/options", (IVacunoRepository repository) => Results.Ok(repository.GetOptions()))
            .WithName("OpcionesVacuno")
            .WithSummary("Devuelve valores usados por los combos de la vista.");

        return group;
    }
}
