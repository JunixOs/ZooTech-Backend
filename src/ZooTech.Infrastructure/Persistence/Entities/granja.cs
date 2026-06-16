using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("granja")]
[Index("distrito_codigo", Name = "idx_granja_distrito")]
[Index("nombre", "distrito_codigo", Name = "uq_granja_nombre_distrito", IsUnique = true)]
public partial class granja
{
    [Key]
    public long id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(6)]
    [Unicode(false)]
    public string distrito_codigo { get; set; } = null!;

    public bool activo { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("distrito_codigo")]
    [InverseProperty("granjas")]
    public virtual geo_distrito distrito_codigoNavigation { get; set; } = null!;

    [InverseProperty("granja")]
    public virtual ICollection<vacuno> vacunos { get; set; } = new List<vacuno>();
}
