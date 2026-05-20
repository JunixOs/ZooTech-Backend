using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("celo_registro")]
[Index("deleted_at", Name = "idx_celo_registro_deleted_at")]
[Index("encargado_usuario_id", Name = "idx_celo_registro_encargado")]
[Index("estado_registro_code", Name = "idx_celo_registro_estado")]
[Index("fecha_hora", Name = "idx_celo_registro_fecha_hora")]
[Index("vacuno_id", Name = "idx_celo_registro_vacuno")]
[Index("codigo", Name = "uq_celo_registro_codigo", IsUnique = true)]
[Index("vacuno_id", "fecha_hora", Name = "uq_celo_registro_vacuno_fecha", IsUnique = true)]
public partial class celo_registro
{
    [Key]
    public long id { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string codigo { get; set; } = null!;

    public DateTime fecha_hora { get; set; }

    public long vacuno_id { get; set; }

    public long encargado_usuario_id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? observaciones { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string estado_registro_code { get; set; } = null!;

    public long? created_by { get; set; }

    public long? updated_by { get; set; }

    public long? deleted_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? motivo_eliminacion { get; set; }

    [InverseProperty("celo_registro")]
    public virtual ICollection<celo_registro_caracteristica_libre> celo_registro_caracteristica_libres { get; set; } = new List<celo_registro_caracteristica_libre>();

    [ForeignKey("created_by")]
    [InverseProperty("celo_registrocreated_byNavigations")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("deleted_by")]
    [InverseProperty("celo_registrodeleted_byNavigations")]
    public virtual usuario? deleted_byNavigation { get; set; }

    [ForeignKey("encargado_usuario_id")]
    [InverseProperty("celo_registroencargado_usuarios")]
    public virtual usuario encargado_usuario { get; set; } = null!;

    [ForeignKey("estado_registro_code")]
    [InverseProperty("celo_registros")]
    public virtual cat_estado_registro estado_registro_codeNavigation { get; set; } = null!;

    [InverseProperty("celo_registro")]
    public virtual ICollection<fecundacion> fecundacions { get; set; } = new List<fecundacion>();

    [ForeignKey("updated_by")]
    [InverseProperty("celo_registroupdated_byNavigations")]
    public virtual usuario? updated_byNavigation { get; set; }

    [ForeignKey("vacuno_id")]
    [InverseProperty("celo_registros")]
    public virtual vacuno vacuno { get; set; } = null!;

    [ForeignKey("celo_registro_id")]
    [InverseProperty("celo_registros")]
    public virtual ICollection<cat_caracteristica_celo> caracteristica_codes { get; set; } = new List<cat_caracteristica_celo>();
}
