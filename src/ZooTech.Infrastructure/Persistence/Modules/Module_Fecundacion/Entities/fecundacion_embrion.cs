using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("fecundacion_embrion")]
public partial class fecundacion_embrion
{
    [Key]
    public long fecundacion_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string codigo_embrion { get; set; } = null!;

    [ForeignKey("fecundacion_id")]
    [InverseProperty("fecundacion_embrion")]
    public virtual fecundacion fecundacion { get; set; } = null!;
}
