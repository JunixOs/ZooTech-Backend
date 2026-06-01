using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("vacuno_utilizacion_historial")]
[Index("created_at", Name = "idx_vuh_created_at")]
[Index("tipo_utilizacion_code", Name = "idx_vuh_tipo")]
[Index("vacuno_id", Name = "idx_vuh_vacuno")]
public partial class vacuno_utilizacion_historial
{
    [Key]
    public long id { get; set; }

    public long vacuno_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string tipo_utilizacion_code { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? motivo { get; set; }

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("vacuno_utilizacion_historials")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("tipo_utilizacion_code")]
    [InverseProperty("vacuno_utilizacion_historials")]
    public virtual cat_tipo_utilizacion tipo_utilizacion_codeNavigation { get; set; } = null!;

    [ForeignKey("vacuno_id")]
    [InverseProperty("vacuno_utilizacion_historials")]
    public virtual vacuno vacuno { get; set; } = null!;
}
