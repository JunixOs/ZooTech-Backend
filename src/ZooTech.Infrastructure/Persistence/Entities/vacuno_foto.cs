using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("vacuno_foto")]
[Index("archivo_id", Name = "idx_vacuno_foto_archivo")]
[Index("vacuno_id", Name = "idx_vacuno_foto_vacuno")]
[Index("vacuno_id", Name = "uq_vacuno_foto_principal", IsUnique = true)]
public partial class vacuno_foto
{
    [Key]
    public long id { get; set; }

    public long vacuno_id { get; set; }

    public long archivo_id { get; set; }

    public bool es_principal { get; set; }

    public long? uploaded_by { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("archivo_id")]
    [InverseProperty("vacuno_fotos")]
    public virtual archivo archivo { get; set; } = null!;

    [ForeignKey("uploaded_by")]
    [InverseProperty("vacuno_fotos")]
    public virtual usuario? uploaded_byNavigation { get; set; }

    [ForeignKey("vacuno_id")]
    [InverseProperty("vacuno_foto")]
    public virtual vacuno vacuno { get; set; } = null!;
}
