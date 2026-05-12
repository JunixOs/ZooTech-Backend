using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_estado_trazabilidad")]
[Index("nombre", Name = "uq_cat_estado_trazabilidad_nombre", IsUnique = true)]
public partial class cat_estado_trazabilidad
{
    [Key]
    [StringLength(30)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(60)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    [InverseProperty("estado_trazabilidad_codeNavigation")]
    public virtual ICollection<fecundacion_crium> fecundacion_cria { get; set; } = new List<fecundacion_crium>();
}
