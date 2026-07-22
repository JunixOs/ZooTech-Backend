using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("vacuno_estado_historial")]
[Index("created_at", Name = "idx_veh_created_at")]
[Index("estado_code", Name = "idx_veh_estado")]
[Index("fecha_estado", Name = "idx_veh_fecha")]
[Index("vacuno_id", Name = "idx_veh_vacuno")]
public partial class vacuno_estado_historial
{
    [Key]
    public long id { get; set; }

    public long vacuno_id { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string estado_code { get; set; } = null!;

    public DateOnly fecha_estado { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? motivo { get; set; }

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("vacuno_estado_historials")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("estado_code")]
    [InverseProperty("vacuno_estado_historials")]
    public virtual cat_estado_vacuno estado_codeNavigation { get; set; } = null!;

    [ForeignKey("vacuno_id")]
    [InverseProperty("vacuno_estado_historials")]
    public virtual vacuno vacuno { get; set; } = null!;
}
