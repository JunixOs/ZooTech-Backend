using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("code", Name = "UQ__features__357D4CF9DA7C46EA", IsUnique = true)]
public partial class feature
{
    [Key]
    public int id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    [Unicode(false)]
    public string? description { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? category { get; set; }

    public bool is_active { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? deleted_at { get; set; }

    [InverseProperty("feature")]
    public virtual ICollection<tenant_feature> tenant_features { get; set; } = new List<tenant_feature>();
}
