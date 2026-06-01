using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_caracteristica_celo")]
[Index("nombre", Name = "uq_cat_caracteristica_celo_nombre", IsUnique = true)]
public partial class cat_caracteristica_celo
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

    public bool activo { get; set; }

    [ForeignKey("caracteristica_code")]
    [InverseProperty("caracteristica_codes")]
    public virtual ICollection<celo_registro> celo_registros { get; set; } = new List<celo_registro>();
}
