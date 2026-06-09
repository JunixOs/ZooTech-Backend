using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_color")]
[Index("nombre", Name = "uq_cat_color_nombre", IsUnique = true)]
public partial class cat_color
{
    [Key]
    [StringLength(30)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(30)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    public bool activo { get; set; }

    [InverseProperty("color_codeNavigation")]
    public virtual ICollection<vacuno> vacunos { get; set; } = new List<vacuno>();
}
