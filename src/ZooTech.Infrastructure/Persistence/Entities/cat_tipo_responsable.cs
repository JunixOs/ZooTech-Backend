using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_tipo_responsable")]
[Index("nombre", Name = "uq_cat_tipo_responsable_nombre", IsUnique = true)]
public partial class cat_tipo_responsable
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

    [InverseProperty("tipo_responsable_codeNavigation")]
    public virtual ICollection<responsable> responsables { get; set; } = new List<responsable>();
}
