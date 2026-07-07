using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("celo_registro_caracteristica_libre")]
[Index("celo_registro_id", Name = "idx_crcl_celo_registro")]
public partial class celo_registro_caracteristica_libre
{
    [Key]
    public long id { get; set; }

    public long celo_registro_id { get; set; }

    [StringLength(80)]
    [Unicode(false)]
    public string descripcion { get; set; } = null!;

    public DateTime created_at { get; set; }

    [ForeignKey("celo_registro_id")]
    [InverseProperty("celo_registro_caracteristica_libres")]
    public virtual celo_registro celo_registro { get; set; } = null!;
}
