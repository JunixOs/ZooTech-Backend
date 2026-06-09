using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_estado_registro")]
[Index("nombre", Name = "uq_cat_estado_registro_nombre", IsUnique = true)]
public partial class cat_estado_registro
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

    [InverseProperty("estado_registro_codeNavigation")]
    public virtual ICollection<celo_registro> celo_registros { get; set; } = new List<celo_registro>();

    [InverseProperty("estado_registro_codeNavigation")]
    public virtual ICollection<incidente_vacuno> incidente_vacunos { get; set; } = new List<incidente_vacuno>();

    [InverseProperty("estado_codeNavigation")]
    public virtual ICollection<reporte_descarga> reporte_descargas { get; set; } = new List<reporte_descarga>();

    [InverseProperty("estado_registro_codeNavigation")]
    public virtual ICollection<triaje> triajes { get; set; } = new List<triaje>();
}
