using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("geo_distrito")]
[Index("provincia_codigo", "nombre", Name = "uq_geo_distrito_prov_nombre", IsUnique = true)]
public partial class geo_distrito
{
    [Key]
    [StringLength(6)]
    [Unicode(false)]
    public string codigo { get; set; } = null!;

    [StringLength(4)]
    [Unicode(false)]
    public string provincia_codigo { get; set; } = null!;

    [StringLength(60)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [InverseProperty("distrito_codigoNavigation")]
    public virtual ICollection<granja> granjas { get; set; } = new List<granja>();

    [ForeignKey("provincia_codigo")]
    [InverseProperty("geo_distritos")]
    public virtual geo_provincium provincia_codigoNavigation { get; set; } = null!;
}
