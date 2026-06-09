using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_sexo")]
[Index("nombre", Name = "uq_cat_sexo_nombre", IsUnique = true)]
public partial class cat_sexo
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    [InverseProperty("sexo_codeNavigation")]
    public virtual ICollection<reproductor_externo> reproductor_externos { get; set; } = new List<reproductor_externo>();

    [InverseProperty("sexo_codeNavigation")]
    public virtual ICollection<vacuno> vacunos { get; set; } = new List<vacuno>();
}
