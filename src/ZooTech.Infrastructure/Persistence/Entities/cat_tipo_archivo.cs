using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("cat_tipo_archivo")]
public partial class cat_tipo_archivo
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string extension { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string mime_type { get; set; } = null!;

    [InverseProperty("extensionNavigation")]
    public virtual ICollection<archivo> archivos { get; set; } = new List<archivo>();
}
