using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Index("fecundacion_id", Name = "idx_fc_fecundacion")]
[Index("estado_trazabilidad_code", Name = "idx_fc_trazabilidad")]
[Index("vacuno_hijo_id", Name = "idx_fc_vacuno_hijo")]
[Index("fecundacion_id", "vacuno_hijo_id", Name = "uq_fecundacion_cria_fec_vacuno", IsUnique = true)]
public partial class fecundacion_crium
{
    [Key]
    public long id { get; set; }

    public long fecundacion_id { get; set; }

    public long vacuno_hijo_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string estado_trazabilidad_code { get; set; } = null!;

    public DateOnly fecha_vinculacion { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? observaciones { get; set; }

    public long? created_by { get; set; }

    public long? updated_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("fecundacion_criumcreated_byNavigations")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("estado_trazabilidad_code")]
    [InverseProperty("fecundacion_cria")]
    public virtual cat_estado_trazabilidad estado_trazabilidad_codeNavigation { get; set; } = null!;

    [ForeignKey("fecundacion_id")]
    [InverseProperty("fecundacion_cria")]
    public virtual fecundacion fecundacion { get; set; } = null!;

    [ForeignKey("updated_by")]
    [InverseProperty("fecundacion_criumupdated_byNavigations")]
    public virtual usuario? updated_byNavigation { get; set; }

    [ForeignKey("vacuno_hijo_id")]
    [InverseProperty("fecundacion_cria")]
    public virtual vacuno vacuno_hijo { get; set; } = null!;
}
