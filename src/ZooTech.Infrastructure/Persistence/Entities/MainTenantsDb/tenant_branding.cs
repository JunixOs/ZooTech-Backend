using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("tenant_id", Name = "UQ__tenant_b__D6F29F3F39D50C33", IsUnique = true)]
public partial class tenant_branding
{
    [Key]
    public long id { get; set; }

    public long tenant_id { get; set; }

    [StringLength(50)]
    public string? primary_color { get; set; }

    [StringLength(50)]
    public string? secondary_color { get; set; }

    [StringLength(1000)]
    public string? logo_url { get; set; }

    public string? metadata { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("tenant_branding")]
    public virtual tenant tenant { get; set; } = null!;
}
