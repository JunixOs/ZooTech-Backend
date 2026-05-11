using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Entities;

[Table("cat_estado_registro")]
[Index("Nombre", Name = "uq_cat_estado_registro_nombre", IsUnique = true)]
public partial class CatEstadoRegistro
{
    [Key]
    [Column("code")]
    [StringLength(30)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("nombre")]
    [StringLength(50)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(150)]
    [Unicode(false)]
    public string? Descripcion { get; set; }

    [InverseProperty("EstadoRegistroCodeNavigation")]
    public virtual ICollection<Triaje> Triajes { get; set; } = new List<Triaje>();
}
