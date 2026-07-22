using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Index("departamento_codigo", "nombre", Name = "uq_geo_provincia_dep_nombre", IsUnique = true)]
public partial class geo_provincium
{
    [Key]
    [StringLength(4)]
    [Unicode(false)]
    public string codigo { get; set; } = null!;

    [StringLength(2)]
    [Unicode(false)]
    public string departamento_codigo { get; set; } = null!;

    [StringLength(60)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [ForeignKey("departamento_codigo")]
    [InverseProperty("geo_provincia")]
    public virtual geo_departamento departamento_codigoNavigation { get; set; } = null!;

    [InverseProperty("provincia_codigoNavigation")]
    public virtual ICollection<geo_distrito> geo_distritos { get; set; } = new List<geo_distrito>();
}
