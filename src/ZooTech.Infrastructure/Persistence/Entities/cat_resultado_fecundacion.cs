using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_resultado_fecundacion")]
[Index("nombre", Name = "uq_cat_resultado_fecundacion_nombre", IsUnique = true)]
public partial class cat_resultado_fecundacion
{
    [Key]
    [StringLength(30)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(60)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    [InverseProperty("resultado_codeNavigation")]
    public virtual ICollection<fecundacion> fecundacions { get; set; } = new List<fecundacion>();
}
