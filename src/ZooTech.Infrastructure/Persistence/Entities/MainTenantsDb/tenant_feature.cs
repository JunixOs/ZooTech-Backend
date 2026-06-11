using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[PrimaryKey("tenant_id", "feature_id")]
public partial class tenant_feature
{
    [Key]
    public long tenant_id { get; set; }

    [Key]
    public long feature_id { get; set; }

    public bool is_enabled { get; set; }

    [Precision(3)]
    public DateTimeOffset? enabled_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? expires_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? updated_at { get; set; }

    public string? metadata { get; set; }

    [ForeignKey("feature_id")]
    [InverseProperty("tenant_features")]
    public virtual feature feature { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("tenant_features")]
    public virtual tenant tenant { get; set; } = null!;
}
