using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("code", Name = "UQ__setting___357D4CF9AEEE7616", IsUnique = true)]
public partial class setting_group
{
    [Key]
    public int id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string? description { get; set; }

    public bool is_active { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? deleted_at { get; set; }

    [InverseProperty("setting_group")]
    public virtual ICollection<setting_definition> setting_definitions { get; set; } = new List<setting_definition>();
}
