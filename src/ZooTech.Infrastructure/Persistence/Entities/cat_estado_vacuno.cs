using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_estado_vacuno")]
[Index("nombre", Name = "uq_cat_estado_vacuno_nombre", IsUnique = true)]
public partial class cat_estado_vacuno
{
    [Key]
    [StringLength(15)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(30)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    [InverseProperty("estado_codeNavigation")]
    public virtual ICollection<vacuno_estado_historial> vacuno_estado_historials { get; set; } = new List<vacuno_estado_historial>();
}
