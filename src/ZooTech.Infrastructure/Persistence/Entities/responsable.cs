using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("responsable")]
[Index("tipo_responsable_code", Name = "idx_responsable_tipo")]
[Index("usuario_id", Name = "idx_responsable_usuario")]
public partial class responsable
{
    [Key]
    public long id { get; set; }

    public long? usuario_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string tipo_responsable_code { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? nombre_completo { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? documento { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? telefono { get; set; }

    public bool activo { get; set; }

    public DateTime created_at { get; set; }

    [InverseProperty("responsable")]
    public virtual ICollection<fecundacion> fecundacions { get; set; } = new List<fecundacion>();

    [ForeignKey("tipo_responsable_code")]
    [InverseProperty("responsables")]
    public virtual cat_tipo_responsable tipo_responsable_codeNavigation { get; set; } = null!;

    [ForeignKey("usuario_id")]
    [InverseProperty("responsables")]
    public virtual usuario? usuario { get; set; }
}
