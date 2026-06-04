using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("code", Name = "UQ__setting___357D4CF980FA33C6", IsUnique = true)]
public partial class setting_definition
{
    [Key]
    public long id { get; set; }

    [StringLength(100)]
    public string? code { get; set; }

    [StringLength(150)]
    public string? name { get; set; }

    [StringLength(100)]
    public string? category { get; set; }

    [StringLength(30)]
    public string? data_type { get; set; }

    public string? default_value { get; set; }

    public string? validation_schema { get; set; }

    public bool? is_required { get; set; }

    public bool? is_sensitive { get; set; }

    public string? metadata { get; set; }

    [Precision(3)]
    public DateTimeOffset? created_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? updated_at { get; set; }

    [InverseProperty("setting_definition")]
    public virtual ICollection<tenant_setting> tenant_settings { get; set; } = new List<tenant_setting>();
}
