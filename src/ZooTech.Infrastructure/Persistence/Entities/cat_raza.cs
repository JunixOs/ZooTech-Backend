using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_raza")]
[Index("nombre", Name = "uq_cat_raza_nombre", IsUnique = true)]
public partial class cat_raza
{
    [Key]
    [StringLength(30)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    public bool activo { get; set; }

    [InverseProperty("raza_codeNavigation")]
    public virtual ICollection<vacuno> vacunos { get; set; } = new List<vacuno>();
}
