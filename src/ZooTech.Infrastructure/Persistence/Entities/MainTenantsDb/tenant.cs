using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("code", Name = "UX_tenants_code", IsUnique = true)]
[Index("sub_domain", Name = "UX_tenants_sub_domain", IsUnique = true)]
public partial class tenant
{
    [Key]
    public long id { get; set; }

    [StringLength(100)]
    public string code { get; set; } = null!;

    [StringLength(255)]
    public string sub_domain { get; set; } = null!;

    [StringLength(255)]
    public string display_name { get; set; } = null!;

    [StringLength(255)]
    public string legal_name { get; set; } = null!;

    [StringLength(255)]
    public string email { get; set; } = null!;

    [StringLength(50)]
    public string phone { get; set; } = null!;

    [StringLength(50)]
    public string status { get; set; } = null!;

    public string? metadata { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [InverseProperty("tenant")]
    public virtual address? address { get; set; }

    [InverseProperty("tenant")]
    public virtual tenant_branding? tenant_branding { get; set; }

    [InverseProperty("tenant")]
    public virtual tenant_database_connection? tenant_database_connection { get; set; }
}
