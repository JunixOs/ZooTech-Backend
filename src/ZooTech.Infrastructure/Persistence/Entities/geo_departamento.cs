using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("geo_departamento")]
[Index("nombre", Name = "uq_geo_departamento_nombre", IsUnique = true)]
public partial class geo_departamento
{
    [Key]
    [StringLength(2)]
    [Unicode(false)]
    public string codigo { get; set; } = null!;

    [StringLength(60)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [InverseProperty("departamento_codigoNavigation")]
    public virtual ICollection<geo_provincium> geo_provincia { get; set; } = new List<geo_provincium>();
}
