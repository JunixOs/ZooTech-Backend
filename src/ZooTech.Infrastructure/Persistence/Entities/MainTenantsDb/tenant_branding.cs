using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Table("tenant_branding")]
[Index("tenant_id", Name = "UQ__tenant_b__D6F29F3F54C8268B", IsUnique = true)]
public partial class tenant_branding
{
    [Key]
    public long id { get; set; }

    public long tenant_id { get; set; }

    [StringLength(20)]
    public string? primary_color { get; set; }

    [StringLength(20)]
    public string? secondary_color { get; set; }

    public string? logo_url { get; set; }

    public string? metadata { get; set; }

    [Precision(3)]
    public DateTimeOffset? created_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? updated_at { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("tenant_branding")]
    public virtual tenant tenant { get; set; } = null!;
}
