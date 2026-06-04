using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("code", Name = "UQ__features__357D4CF929F381D9", IsUnique = true)]
public partial class feature
{
    [Key]
    public long id { get; set; }

    [StringLength(100)]
    public string code { get; set; } = null!;

    [StringLength(150)]
    public string name { get; set; } = null!;

    public string? description { get; set; }

    [StringLength(100)]
    public string? category { get; set; }

    public bool is_active { get; set; }

    public string? metadata { get; set; }

    [Precision(3)]
    public DateTimeOffset? created_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? updated_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? deleted_at { get; set; }

    [InverseProperty("feature")]
    public virtual ICollection<tenant_feature> tenant_features { get; set; } = new List<tenant_feature>();
}
