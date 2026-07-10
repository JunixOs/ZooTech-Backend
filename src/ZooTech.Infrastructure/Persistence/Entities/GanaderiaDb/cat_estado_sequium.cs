using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Index("nombre", Name = "uq_cat_estado_sequia_nombre", IsUnique = true)]
public partial class cat_estado_sequium
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

    [InverseProperty("estado_sequia_codeNavigation")]
    public virtual ICollection<periodo_sequium> periodo_sequia { get; set; } = new List<periodo_sequium>();
}
