using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_tipo_adquisicion")]
[Index("nombre", Name = "uq_cat_tipo_adquisicion_nombre", IsUnique = true)]
public partial class cat_tipo_adquisicion
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

    [InverseProperty("tipo_adquisicion_codeNavigation")]
    public virtual ICollection<vacuno_adquisicion> vacuno_adquisicions { get; set; } = new List<vacuno_adquisicion>();

    [InverseProperty("tipo_adquisicion_codeNavigation")]
    public virtual ICollection<vacuno> vacunos { get; set; } = new List<vacuno>();
}
