using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("fecundacion_inseminacion")]
public partial class fecundacion_inseminacion
{
    [Key]
    public long fecundacion_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string codigo_semen { get; set; } = null!;

    [ForeignKey("fecundacion_id")]
    [InverseProperty("fecundacion_inseminacion")]
    public virtual fecundacion fecundacion { get; set; } = null!;
}
