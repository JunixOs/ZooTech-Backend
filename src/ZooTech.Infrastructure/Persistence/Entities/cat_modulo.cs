using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_modulo")]
[Index("nombre", Name = "uq_cat_modulo_nombre", IsUnique = true)]
public partial class cat_modulo
{
    [Key]
    [StringLength(40)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    public bool activo { get; set; }

    [InverseProperty("modulo_codeNavigation")]
    public virtual ICollection<archivo> archivos { get; set; } = new List<archivo>();

    [InverseProperty("modulo_codeNavigation")]
    public virtual ICollection<bitacora_auditorium> bitacora_auditoria { get; set; } = new List<bitacora_auditorium>();

    [InverseProperty("modulo_codeNavigation")]
    public virtual ICollection<cat_tipo_reporte> cat_tipo_reportes { get; set; } = new List<cat_tipo_reporte>();

    [InverseProperty("modulo_codeNavigation")]
    public virtual ICollection<parametro_sistema> parametro_sistemas { get; set; } = new List<parametro_sistema>();
}
