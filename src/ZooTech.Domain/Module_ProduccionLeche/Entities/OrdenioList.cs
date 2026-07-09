using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Domain.Module_ProduccionLeche.Entities;

public sealed class OrdenioList
{
    private OrdenioList(
        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string nombreVacuno,
        long encargadoUsuarioId,
        string nombreCompleto,
        decimal litros,
        string estadoOrdenioCode,
        string? observaciones,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt,
        string? motivoEliminacion
     )
    {
        Id = id;
        Codigo = codigo;
        FechaHora = fechaHora;
        VacunoId = vacunoId;
        NombreVacuno = nombreVacuno;
        EncargadoUsuarioId = encargadoUsuarioId;
        NombreCompleto = nombreCompleto;
        Litros = litros;
        EstadoOrdenioCode = estadoOrdenioCode;
        Observaciones = observaciones;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
        MotivoEliminacion = motivoEliminacion;
        
    }

    public long Id { get; }
    public string Codigo { get; private set; }
    public DateTime FechaHora { get; private set; }
    public long VacunoId { get; private set; }
    public string NombreVacuno { get; private set; }
    public long EncargadoUsuarioId { get; private set; }
    public string NombreCompleto { get; private set; }
    public decimal Litros { get; private set; }
    public string EstadoOrdenioCode { get; private set; }
    public string? Observaciones { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? MotivoEliminacion { get; private set; }


    public bool IsDeleted => DeletedAt.HasValue;

    public static OrdenioList Rehydrate(
        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string nombreVacuno,
        long encargadoUsuarioId,
        string nombreCompleto,
        decimal litros,
        string estadoOrdenioCode,
        string? observaciones,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt,
        string? motivoEliminacion
        )
    {

        return new OrdenioList(
            id,
            codigo.Trim(),
            fechaHora,
            vacunoId,
            nombreVacuno,
            encargadoUsuarioId,
            nombreCompleto,
            litros,
            estadoOrdenioCode.Trim(),
            observaciones,
            createdAt,
            updatedAt,
            deletedAt,
            motivoEliminacion
          );
    }


}

