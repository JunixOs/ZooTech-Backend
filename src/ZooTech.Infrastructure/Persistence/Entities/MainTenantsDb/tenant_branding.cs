using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Table("tenant_branding")]
[Index("tenant_id", Name = "UQ__tenant_b__D6F29F3F263BA3CE", IsUnique = true)]
public partial class tenant_branding
{
    [Key]
    public int id { get; set; }

    public int tenant_id { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? primary_color { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? secondary_color { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? accent_color { get; set; }

    [Unicode(false)]
    public string? logo_url { get; set; }

    [Unicode(false)]
    public string? favicon_url { get; set; }

    [Unicode(false)]
    public string? login_background_url { get; set; }

    [Unicode(false)]
    public string? custom_css { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? deleted_at { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("tenant_branding")]
    public virtual tenant tenant { get; set; } = null!;
}
