using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_tipo_peso")]
[Index("nombre", Name = "uq_cat_tipo_peso_nombre", IsUnique = true)]
public partial class cat_tipo_peso
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

    public bool activo { get; set; }

    [InverseProperty("tipo_peso_codeNavigation")]
    public virtual ICollection<triaje> triajes { get; set; } = new List<triaje>();
}
