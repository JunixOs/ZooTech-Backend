using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("vacuno_adquisicion")]
[Index("fecha_adquisicion", Name = "idx_vacuno_adquisicion_fecha")]
[Index("tipo_adquisicion_code", Name = "idx_vacuno_adquisicion_tipo")]
public partial class vacuno_adquisicion
{
    [Key]
    public long vacuno_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string tipo_adquisicion_code { get; set; } = null!;

    [Column(TypeName = "numeric(12, 2)")]
    public decimal? precio_compra { get; set; }

    public DateOnly fecha_adquisicion { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? proveedor { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? observaciones { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("tipo_adquisicion_code")]
    [InverseProperty("vacuno_adquisicions")]
    public virtual cat_tipo_adquisicion tipo_adquisicion_codeNavigation { get; set; } = null!;

    [ForeignKey("vacuno_id")]
    [InverseProperty("vacuno_adquisicion")]
    public virtual vacuno vacuno { get; set; } = null!;
}
