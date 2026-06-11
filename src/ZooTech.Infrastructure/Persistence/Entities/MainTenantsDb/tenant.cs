using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("code", Name = "UQ__tenants__357D4CF9ADE2F5DB", IsUnique = true)]
[Index("subdomain", Name = "UQ__tenants__E956860B905B77E0", IsUnique = true)]
public partial class tenant
{
    [Key]
    public long id { get; set; }

    [StringLength(50)]
    public string code { get; set; } = null!;

    [StringLength(50)]
    public string subdomain { get; set; } = null!;

    [StringLength(150)]
    public string display_name { get; set; } = null!;

    [StringLength(200)]
    public string legal_name { get; set; } = null!;

    [StringLength(150)]
    public string email { get; set; } = null!;

    [StringLength(50)]
    public string phone { get; set; } = null!;

    [StringLength(100)]
    public string timezone { get; set; } = null!;

    [StringLength(30)]
    public string status { get; set; } = null!;

    public string? metadata { get; set; }

    [Precision(3)]
    public DateTimeOffset? created_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? updated_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? deleted_at { get; set; }

    [InverseProperty("tenant")]
    public virtual ICollection<address> addresses { get; set; } = new List<address>();

    [InverseProperty("tenant")]
    public virtual tenant_branding? tenant_branding { get; set; }

    [InverseProperty("tenant")]
    public virtual ICollection<tenant_business_rule> tenant_business_rules { get; set; } = new List<tenant_business_rule>();

    [InverseProperty("tenant")]
    public virtual tenant_database_connection? tenant_database_connection { get; set; }

    [InverseProperty("tenant")]
    public virtual ICollection<tenant_feature> tenant_features { get; set; } = new List<tenant_feature>();

    [InverseProperty("tenant")]
    public virtual ICollection<tenant_setting> tenant_settings { get; set; } = new List<tenant_setting>();
}
