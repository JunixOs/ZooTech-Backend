using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("code", Name = "UQ__tenants__357D4CF9BCD87FD5", IsUnique = true)]
[Index("subdomain", Name = "UQ__tenants__E956860BD48158C8", IsUnique = true)]
public partial class tenant
{
    [Key]
    public int id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string subdomain { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string display_name { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string legal_name { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string email { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string phone { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string timezone { get; set; } = null!;

    [StringLength(30)]
    [Unicode(false)]
    public string status { get; set; } = null!;

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? deleted_at { get; set; }

    [InverseProperty("tenant")]
    public virtual address? address { get; set; }

    [InverseProperty("tenant")]
    public virtual ICollection<setting_value> setting_values { get; set; } = new List<setting_value>();

    [InverseProperty("tenant")]
    public virtual tenant_branding? tenant_branding { get; set; }

    [InverseProperty("tenant")]
    public virtual ICollection<tenant_business_rule> tenant_business_rules { get; set; } = new List<tenant_business_rule>();

    [InverseProperty("tenant")]
    public virtual tenant_database_connection? tenant_database_connection { get; set; }

    [InverseProperty("tenant")]
    public virtual ICollection<tenant_feature> tenant_features { get; set; } = new List<tenant_feature>();
}
