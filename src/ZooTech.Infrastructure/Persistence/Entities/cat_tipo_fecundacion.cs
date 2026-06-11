using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_tipo_fecundacion")]
[Index("nombre", Name = "uq_cat_tipo_fecundacion_nombre", IsUnique = true)]
public partial class cat_tipo_fecundacion
{
    [Key]
    [StringLength(40)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    [InverseProperty("tipo_fecundacion_codeNavigation")]
    public virtual ICollection<fecundacion> fecundacions { get; set; } = new List<fecundacion>();
}
