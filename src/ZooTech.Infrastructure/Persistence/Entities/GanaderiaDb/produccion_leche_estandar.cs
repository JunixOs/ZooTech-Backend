using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("produccion_leche_estandar")]
[Index("fecha_fin", Name = "idx_ple_fecha_fin")]
[Index("fecha_inicio", Name = "idx_ple_fecha_ini")]
[Index("vacuno_id", Name = "idx_ple_vacuno")]
public partial class produccion_leche_estandar
{
    [Key]
    public long id { get; set; }

    public long? vacuno_id { get; set; }

    public DateOnly fecha_inicio { get; set; }

    public DateOnly? fecha_fin { get; set; }

    [Column(TypeName = "numeric(8, 3)")]
    public decimal litros_esperados_dia { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("produccion_leche_estandars")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("vacuno_id")]
    [InverseProperty("produccion_leche_estandars")]
    public virtual vacuno? vacuno { get; set; }
}
