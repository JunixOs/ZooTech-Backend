using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("fecundacion_donante")]
[Index("externo_donante_id", Name = "idx_fd_externo_donante")]
[Index("vacuno_donante_id", Name = "idx_fd_vacuno_donante")]
public partial class fecundacion_donante
{
    [Key]
    public long fecundacion_id { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string tipo_donante { get; set; } = null!;

    public long? vacuno_donante_id { get; set; }

    public long? externo_donante_id { get; set; }

    [ForeignKey("externo_donante_id")]
    [InverseProperty("fecundacion_donantes")]
    public virtual reproductor_externo? externo_donante { get; set; }

    [ForeignKey("fecundacion_id")]
    [InverseProperty("fecundacion_donante")]
    public virtual fecundacion fecundacion { get; set; } = null!;

    [ForeignKey("vacuno_donante_id")]
    [InverseProperty("fecundacion_donantes")]
    public virtual vacuno? vacuno_donante { get; set; }
}
