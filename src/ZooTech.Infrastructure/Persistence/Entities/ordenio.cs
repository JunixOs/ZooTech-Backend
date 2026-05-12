using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("ordenio")]
[Index("deleted_at", Name = "idx_ordenio_deleted_at")]
[Index("encargado_usuario_id", Name = "idx_ordenio_encargado")]
[Index("estado_ordenio_code", Name = "idx_ordenio_estado")]
[Index("fecha_hora", Name = "idx_ordenio_fecha_hora")]
[Index("vacuno_id", Name = "idx_ordenio_vacuno")]
[Index("codigo", Name = "uq_ordenio_codigo", IsUnique = true)]
[Index("vacuno_id", "fecha_hora", Name = "uq_ordenio_vacuno_fecha", IsUnique = true)]
public partial class ordenio
{
    [Key]
    public long id { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string codigo { get; set; } = null!;

    public DateTime fecha_hora { get; set; }

    public long vacuno_id { get; set; }

    public long encargado_usuario_id { get; set; }

    [Column(TypeName = "numeric(8, 3)")]
    public decimal litros { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? observaciones { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string estado_ordenio_code { get; set; } = null!;

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
    [InverseProperty("ordeniocreated_byNavigations")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("deleted_by")]
    [InverseProperty("ordeniodeleted_byNavigations")]
    public virtual usuario? deleted_byNavigation { get; set; }

    [ForeignKey("encargado_usuario_id")]
    [InverseProperty("ordenioencargado_usuarios")]
    public virtual usuario encargado_usuario { get; set; } = null!;

    [ForeignKey("estado_ordenio_code")]
    [InverseProperty("ordenios")]
    public virtual cat_estado_ordenio estado_ordenio_codeNavigation { get; set; } = null!;

    [ForeignKey("updated_by")]
    [InverseProperty("ordenioupdated_byNavigations")]
    public virtual usuario? updated_byNavigation { get; set; }

    [ForeignKey("vacuno_id")]
    [InverseProperty("ordenios")]
    public virtual vacuno vacuno { get; set; } = null!;
}
