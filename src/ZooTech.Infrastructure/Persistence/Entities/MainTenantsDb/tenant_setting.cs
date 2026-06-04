using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[PrimaryKey("tenant_id", "setting_definition_id")]
public partial class tenant_setting
{
    [Key]
    public long tenant_id { get; set; }

    [Key]
    public long setting_definition_id { get; set; }

    public string? value { get; set; }

    public string? metadata { get; set; }

    [Precision(3)]
    public DateTimeOffset? updated_at { get; set; }

    [ForeignKey("setting_definition_id")]
    [InverseProperty("tenant_settings")]
    public virtual setting_definition setting_definition { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("tenant_settings")]
    public virtual tenant tenant { get; set; } = null!;
}
