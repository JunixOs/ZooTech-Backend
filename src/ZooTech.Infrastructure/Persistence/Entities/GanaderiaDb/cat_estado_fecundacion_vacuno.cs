using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_estado_fecundacion_vacuno")]
[Index("nombre", Name = "uq_cat_estado_fec_vacuno_nombre", IsUnique = true)]
public partial class cat_estado_fecundacion_vacuno
{
    [Key]
    [StringLength(30)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(60)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    [InverseProperty("estado_fecundacion_codeNavigation")]
    public virtual ICollection<vacuno_estado_fecundacion_historial> vacuno_estado_fecundacion_historials { get; set; } = new List<vacuno_estado_fecundacion_historial>();
}
