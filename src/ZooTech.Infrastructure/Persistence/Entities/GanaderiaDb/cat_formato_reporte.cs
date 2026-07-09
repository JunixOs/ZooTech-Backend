using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_formato_reporte")]
[Index("nombre", Name = "uq_cat_formato_reporte_nombre", IsUnique = true)]
public partial class cat_formato_reporte
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [InverseProperty("formato_codeNavigation")]
    public virtual ICollection<reporte_descarga> reporte_descargas { get; set; } = new List<reporte_descarga>();
}
