using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("celo_configuracion")]
[Index("activo", Name = "idx_celo_config_activo")]
public partial class celo_configuracion
{
    [Key]
    public long id { get; set; }

    [StringLength(80)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    public int dias_ciclo_estandar { get; set; }

    public int dias_alerta_previa { get; set; }

    public int dias_tolerancia_posterior { get; set; }

    public bool activo { get; set; }

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("celo_configuracions")]
    public virtual usuario? created_byNavigation { get; set; }
}
