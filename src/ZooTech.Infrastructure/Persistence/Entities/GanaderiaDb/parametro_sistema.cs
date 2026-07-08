using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("parametro_sistema")]
[Index("modulo_code", "clave", Name = "idx_parametro_modulo_clave")]
[Index("modulo_code", "clave", Name = "uq_parametro_modulo_clave", IsUnique = true)]
public partial class parametro_sistema
{
    [Key]
    public long id { get; set; }

    [StringLength(40)]
    [Unicode(false)]
    public string modulo_code { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string clave { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string valor { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("modulo_code")]
    [InverseProperty("parametro_sistemas")]
    public virtual cat_modulo modulo_codeNavigation { get; set; } = null!;
}
