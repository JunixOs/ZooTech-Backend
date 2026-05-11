using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Entities;

[Table("triaje")]
[Index("DeletedAt", Name = "idx_triaje_deleted_at")]
[Index("EstadoRegistroCode", Name = "idx_triaje_estado")]
[Index("FechaHora", Name = "idx_triaje_fecha_hora")]
[Index("TipoPesoCode", Name = "idx_triaje_tipo_peso")]
[Index("VacunoId", Name = "idx_triaje_vacuno")]
[Index("Codigo", Name = "uq_triaje_codigo", IsUnique = true)]
[Index("VacunoId", "FechaHora", "TipoPesoCode", Name = "uq_triaje_vacuno_fecha_tipo", IsUnique = true)]
public partial class Triaje
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("codigo")]
    [StringLength(15)]
    [Unicode(false)]
    public string Codigo { get; set; } = null!;

    [Column("fecha_hora")]
    public DateTime FechaHora { get; set; }

    [Column("vacuno_id")]
    public long VacunoId { get; set; }

    [Column("tipo_peso_code")]
    [StringLength(30)]
    [Unicode(false)]
    public string TipoPesoCode { get; set; } = null!;

    [Column("peso_kg", TypeName = "numeric(8, 2)")]
    public decimal PesoKg { get; set; }

    [Column("observaciones")]
    [StringLength(150)]
    [Unicode(false)]
    public string? Observaciones { get; set; }

    [Column("estado_registro_code")]
    [StringLength(30)]
    [Unicode(false)]
    public string EstadoRegistroCode { get; set; } = null!;

    [Column("encargado_usuario_id")]
    public long? EncargadoUsuarioId { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("updated_by")]
    public long? UpdatedBy { get; set; }

    [Column("deleted_by")]
    public long? DeletedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("motivo_eliminacion")]
    [StringLength(250)]
    [Unicode(false)]
    public string? MotivoEliminacion { get; set; }

    [ForeignKey("EstadoRegistroCode")]
    [InverseProperty("Triajes")]
    public virtual CatEstadoRegistro EstadoRegistroCodeNavigation { get; set; } = null!;

    [ForeignKey("TipoPesoCode")]
    [InverseProperty("Triajes")]
    public virtual CatTipoPeso TipoPesoCodeNavigation { get; set; } = null!;
}
