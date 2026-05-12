using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("fecundacion")]
[Index("celo_registro_id", Name = "idx_fecundacion_celo")]
[Index("fecha_procedimiento", Name = "idx_fecundacion_fecha")]
[Index("vacuno_receptor_id", Name = "idx_fecundacion_receptor")]
[Index("responsable_id", Name = "idx_fecundacion_responsable")]
[Index("resultado_code", Name = "idx_fecundacion_resultado")]
[Index("tipo_fecundacion_code", Name = "idx_fecundacion_tipo")]
[Index("codigo", Name = "uq_fecundacion_codigo", IsUnique = true)]
public partial class fecundacion
{
    [Key]
    public long id { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string codigo { get; set; } = null!;

    [StringLength(40)]
    [Unicode(false)]
    public string tipo_fecundacion_code { get; set; } = null!;

    public long vacuno_receptor_id { get; set; }

    public long? celo_registro_id { get; set; }

    public DateOnly fecha_procedimiento { get; set; }

    public long responsable_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string resultado_code { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string? observaciones_veterinarias { get; set; }

    public long? created_by { get; set; }

    public long? updated_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("celo_registro_id")]
    [InverseProperty("fecundacions")]
    public virtual celo_registro? celo_registro { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("fecundacioncreated_byNavigations")]
    public virtual usuario? created_byNavigation { get; set; }

    [InverseProperty("fecundacion")]
    public virtual ICollection<fecundacion_crium> fecundacion_cria { get; set; } = new List<fecundacion_crium>();

    [InverseProperty("fecundacion")]
    public virtual fecundacion_donante? fecundacion_donante { get; set; }

    [InverseProperty("fecundacion")]
    public virtual fecundacion_embrion? fecundacion_embrion { get; set; }

    [InverseProperty("fecundacion")]
    public virtual fecundacion_inseminacion? fecundacion_inseminacion { get; set; }

    [ForeignKey("responsable_id")]
    [InverseProperty("fecundacions")]
    public virtual responsable responsable { get; set; } = null!;

    [ForeignKey("resultado_code")]
    [InverseProperty("fecundacions")]
    public virtual cat_resultado_fecundacion resultado_codeNavigation { get; set; } = null!;

    [ForeignKey("tipo_fecundacion_code")]
    [InverseProperty("fecundacions")]
    public virtual cat_tipo_fecundacion tipo_fecundacion_codeNavigation { get; set; } = null!;

    [ForeignKey("updated_by")]
    [InverseProperty("fecundacionupdated_byNavigations")]
    public virtual usuario? updated_byNavigation { get; set; }

    [InverseProperty("fecundacion")]
    public virtual ICollection<vacuno_estado_fecundacion_historial> vacuno_estado_fecundacion_historials { get; set; } = new List<vacuno_estado_fecundacion_historial>();

    [ForeignKey("vacuno_receptor_id")]
    [InverseProperty("fecundacions")]
    public virtual vacuno vacuno_receptor { get; set; } = null!;
}
