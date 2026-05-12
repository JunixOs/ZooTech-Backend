using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("incidente_vacuno")]
[Index("estado_registro_code", Name = "idx_incidente_estado")]
[Index("fecha_incidente", Name = "idx_incidente_fecha")]
[Index("vacuno_id", Name = "idx_incidente_vacuno")]
public partial class incidente_vacuno
{
    [Key]
    public long id { get; set; }

    public long vacuno_id { get; set; }

    public DateOnly fecha_incidente { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string descripcion { get; set; } = null!;

    [StringLength(30)]
    [Unicode(false)]
    public string estado_registro_code { get; set; } = null!;

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("incidente_vacunos")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("estado_registro_code")]
    [InverseProperty("incidente_vacunos")]
    public virtual cat_estado_registro estado_registro_codeNavigation { get; set; } = null!;

    [ForeignKey("vacuno_id")]
    [InverseProperty("incidente_vacunos")]
    public virtual vacuno vacuno { get; set; } = null!;
}
