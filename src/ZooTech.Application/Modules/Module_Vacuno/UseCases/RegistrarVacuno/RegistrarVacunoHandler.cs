using MediatR;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Exceptions;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.RegistrarVacuno;

/// <summary>
/// Handler del caso de uso "Registrar Vacuno".
/// Orquesta: verificar duplicado → crear entidades → guardar → retornar resultado.
/// No tiene lógica de HTTP ni de base de datos; delega en ports (interfaces).
/// </summary>
public sealed class RegistrarVacunoHandler : IRequestHandler<RegistrarVacunoCommand, RegistrarVacunoResult>
{
    private readonly IVacunoRepository _vacunoRepository;
    private readonly IArchivoService _archivoService;
    private readonly ITimeProvider _timeProvider;

    // Catálogos necesarios para resolver IDs
    private const int IdEstadoVivo = 1;  // cat_estado_vacuno: 1 = vivo (confirmar con DBA)

    public RegistrarVacunoHandler(
        IVacunoRepository vacunoRepository,
        IArchivoService archivoService,
        ITimeProvider timeProvider)
    {
        _vacunoRepository = vacunoRepository;
        _archivoService = archivoService;
        _timeProvider = timeProvider;
    }

    public async Task<RegistrarVacunoResult> Handle(
        RegistrarVacunoCommand command,
        CancellationToken cancellationToken)
    {
        // ── 1. Verificar duplicado por código ─────────────────────────
        var yaExiste = await _vacunoRepository.ExisteCodigoAsync(command.Codigo, cancellationToken);
        if (yaExiste)
        {
            // Lanzamos excepción de dominio; el middleware la mapea a 409 Conflict
            throw new VacunoYaExisteException(command.Codigo);
        }

        var ahora = _timeProvider.UtcNow;

        // ── 2. Crear entidad principal ────────────────────────────────
        var vacuno = Vacuno.Crear(
            codigo: command.Codigo,
            nombre: command.Nombre,
            fechaNacimiento: command.FechaNacimiento,
            idRaza: command.IdRaza,
            raza: command.Raza,
            idColor: command.IdColor,
            color: command.Color,
            idSexo: command.IdSexo,
            sexo: command.Sexo,
            codigoPadre: command.CodigoPadre,
            codigoMadre: command.CodigoMadre,
            idGranja: await ResolverGranjaIdAsync(command.NombreGranja, cancellationToken),
            granja: command.NombreGranja,
            idDistrito: command.IdDistrito,
            distrito: command.Distrito,
            idDepartamento: command.IdDepartamento,
            departamento: command.Departamento,
            idProvincia: command.IdProvincia,
            provincia: command.Provincia,
            idEstadoVivo: IdEstadoVivo,
            ahora: ahora
        );

        // ── 3. Crear entidad de adquisición ───────────────────────────
        var adquisicion = VacunoAdquisicion.Crear(
            idVacuno: 0, // EF asignará el FK real tras la inserción
            idTipoAdquisicion: command.IdTipoAdquisicion,
            precioCompra: command.PrecioCompra
        );

        // ── 4. Crear entidad de utilización ───────────────────────────
        var utilizacion = VacunoUtilizacionHistorial.Crear(
            idVacuno: 0,
            idTipoUtilizacion: command.IdTipoUtilizacion,
            aptoPara: command.AptoPara,
            fechaEspecificacion: command.FechaEspecificacion,
            observaciones: command.Observaciones
        );

        // ── 5. Procesar foto si fue adjuntada ─────────────────────────
        VacunoFoto? foto = null;
        string? fotoUrl = null;

        if (command.FotoStream is not null && command.FotoNombreOriginal is not null)
        {
            var ruta = await _archivoService.GuardarAsync(
                stream: command.FotoStream,
                nombreOriginal: command.FotoNombreOriginal,
                carpeta: "vacunos/fotos",
                ct: cancellationToken
            );

            foto = VacunoFoto.Crear(idVacuno: 0, rutaArchivo: ruta);
            fotoUrl = ruta; // El controller construye la URL pública completa
        }

        // ── 6. Persistir todo en una transacción ──────────────────────
        var vacunoGuardado = await _vacunoRepository.RegistrarAsync(
            vacuno, adquisicion, utilizacion, foto, cancellationToken);

        // ── 7. Retornar resultado ─────────────────────────────────────
        return new RegistrarVacunoResult(
            Id: vacunoGuardado.Id,
            Codigo: vacunoGuardado.Codigo,
            Nombre: vacunoGuardado.Nombre,
            FechaNacimiento: vacunoGuardado.FechaNacimiento,
            TipoAdquisicion: command.IdTipoAdquisicion == 2 ? "compra" : "monta",
            PrecioCompra: command.PrecioCompra,
            FotoUrl: fotoUrl,
            CreadoEn: vacunoGuardado.CreadoEn
        );
    }

    // Nota: si granja ya existe en BD por nombre se reutiliza el ID;
    // si no existe se crea. Este método se puede extraer a un servicio de dominio.
    private Task<int> ResolverGranjaIdAsync(string nombreGranja, CancellationToken ct)
    {
        // TODO: inyectar IGranjaRepository y resolver o crear la granja.
        // Por ahora retornamos un placeholder para que el Handler compile.
        // Implementar en conjunto con el módulo de granjas.
        return Task.FromResult(1);
    }
}
