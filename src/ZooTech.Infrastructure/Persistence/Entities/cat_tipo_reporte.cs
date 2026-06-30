using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_tipo_reporte")]
[Index("modulo_code", Name = "idx_cat_tipo_reporte_modulo")]
[Index("modulo_code", "nombre", Name = "uq_cat_tipo_reporte_modulo_nombre", IsUnique = true)]
public partial class cat_tipo_reporte
{
    [Key]
    [StringLength(60)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(40)]
    [Unicode(false)]
    public string modulo_code { get; set; } = null!;

    [StringLength(120)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    public bool activo { get; set; }

    [ForeignKey("modulo_code")]
    [InverseProperty("cat_tipo_reportes")]
    public virtual cat_modulo modulo_codeNavigation { get; set; } = null!;

    [InverseProperty("tipo_reporte_codeNavigation")]
    public virtual ICollection<reporte_descarga> reporte_descargas { get; set; } = new List<reporte_descarga>();
}
