using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_estado_ordenio")]
[Index("nombre", Name = "uq_cat_estado_ordenio_nombre", IsUnique = true)]
public partial class cat_estado_ordenio
{
    [Key]
    [StringLength(30)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    [InverseProperty("estado_ordenio_codeNavigation")]
    public virtual ICollection<ordenio> ordenios { get; set; } = new List<ordenio>();
}
