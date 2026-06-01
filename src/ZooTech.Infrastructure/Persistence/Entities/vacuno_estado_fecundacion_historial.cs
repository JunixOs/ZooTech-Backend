using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("vacuno_estado_fecundacion_historial")]
[Index("created_at", Name = "idx_vefh_created_at")]
[Index("deleted_at", Name = "idx_vefh_deleted_at")]
[Index("estado_fecundacion_code", Name = "idx_vefh_estado")]
[Index("fecha_actualizacion", Name = "idx_vefh_fecha")]
[Index("fecundacion_id", Name = "idx_vefh_fecundacion")]
[Index("vacuno_id", Name = "idx_vefh_vacuno")]
public partial class vacuno_estado_fecundacion_historial
{
    [Key]
    public long id { get; set; }

    public long vacuno_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string estado_fecundacion_code { get; set; } = null!;

    public long? fecundacion_id { get; set; }

    public DateOnly fecha_actualizacion { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? observaciones { get; set; }

    public long? created_by { get; set; }

    public long? deleted_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? motivo_eliminacion { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("vacuno_estado_fecundacion_historialcreated_byNavigations")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("deleted_by")]
    [InverseProperty("vacuno_estado_fecundacion_historialdeleted_byNavigations")]
    public virtual usuario? deleted_byNavigation { get; set; }

    [ForeignKey("estado_fecundacion_code")]
    [InverseProperty("vacuno_estado_fecundacion_historials")]
    public virtual cat_estado_fecundacion_vacuno estado_fecundacion_codeNavigation { get; set; } = null!;

    [ForeignKey("fecundacion_id")]
    [InverseProperty("vacuno_estado_fecundacion_historials")]
    public virtual fecundacion? fecundacion { get; set; }

    [ForeignKey("vacuno_id")]
    [InverseProperty("vacuno_estado_fecundacion_historials")]
    public virtual vacuno vacuno { get; set; } = null!;
}
