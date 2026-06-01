using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Index("estado_sequia_code", Name = "idx_sequia_estado")]
[Index("fecha_fin_estimada", Name = "idx_sequia_fecha_fin_e")]
[Index("fecha_fin_real", Name = "idx_sequia_fecha_fin_r")]
[Index("fecha_inicio", Name = "idx_sequia_fecha_ini")]
[Index("vacuno_id", Name = "idx_sequia_vacuno")]
public partial class periodo_sequium
{
    [Key]
    public long id { get; set; }

    public long vacuno_id { get; set; }

    public DateOnly fecha_inicio { get; set; }

    public DateOnly? fecha_fin_estimada { get; set; }

    public DateOnly? fecha_fin_real { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string estado_sequia_code { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? motivo { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? observaciones { get; set; }

    public long? created_by { get; set; }

    public long? updated_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("periodo_sequiumcreated_byNavigations")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("estado_sequia_code")]
    [InverseProperty("periodo_sequia")]
    public virtual cat_estado_sequium estado_sequia_codeNavigation { get; set; } = null!;

    [ForeignKey("updated_by")]
    [InverseProperty("periodo_sequiumupdated_byNavigations")]
    public virtual usuario? updated_byNavigation { get; set; }

    [ForeignKey("vacuno_id")]
    [InverseProperty("periodo_sequia")]
    public virtual vacuno vacuno { get; set; } = null!;
}
