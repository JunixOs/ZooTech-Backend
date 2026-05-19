using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("triaje")]
[Index("deleted_at", Name = "idx_triaje_deleted_at")]
[Index("estado_registro_code", Name = "idx_triaje_estado")]
[Index("fecha_hora", Name = "idx_triaje_fecha_hora")]
[Index("tipo_peso_code", Name = "idx_triaje_tipo_peso")]
[Index("vacuno_id", Name = "idx_triaje_vacuno")]
[Index("codigo", Name = "uq_triaje_codigo", IsUnique = true)]
[Index("vacuno_id", "fecha_hora", "tipo_peso_code", Name = "uq_triaje_vacuno_fecha_tipo", IsUnique = true)]
public partial class triaje
{
    [Key]
    public long id { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string codigo { get; set; } = null!;

    public DateTime fecha_hora { get; set; }

    public long vacuno_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string tipo_peso_code { get; set; } = null!;

    [Column(TypeName = "numeric(8, 2)")]
    public decimal peso_kg { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? observaciones { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string estado_registro_code { get; set; } = null!;

    public long? encargado_usuario_id { get; set; }

    public long? created_by { get; set; }

    public long? updated_by { get; set; }

    public long? deleted_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? motivo_eliminacion { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("triajecreated_byNavigations")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("deleted_by")]
    [InverseProperty("triajedeleted_byNavigations")]
    public virtual usuario? deleted_byNavigation { get; set; }

    [ForeignKey("encargado_usuario_id")]
    [InverseProperty("triajeencargado_usuarios")]
    public virtual usuario? encargado_usuario { get; set; }

    [ForeignKey("estado_registro_code")]
    [InverseProperty("triajes")]
    public virtual cat_estado_registro estado_registro_codeNavigation { get; set; } = null!;

    [ForeignKey("tipo_peso_code")]
    [InverseProperty("triajes")]
    public virtual cat_tipo_peso tipo_peso_codeNavigation { get; set; } = null!;

    [ForeignKey("updated_by")]
    [InverseProperty("triajeupdated_byNavigations")]
    public virtual usuario? updated_byNavigation { get; set; }

    [ForeignKey("vacuno_id")]
    [InverseProperty("triajes")]
    public virtual vacuno vacuno { get; set; } = null!;
}
