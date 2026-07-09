using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("tenant_id", "setting_definition_id", "actor_type", "actor_id", Name = "UX_setting_values", IsUnique = true)]
public partial class setting_value
{
    [Key]
    public int id { get; set; }

    public int tenant_id { get; set; }

    public int setting_definition_id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string actor_type { get; set; } = null!;

    public int? actor_id { get; set; }

    public string value { get; set; } = null!;

    public bool is_active { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? deleted_at { get; set; }

    [ForeignKey("setting_definition_id")]
    [InverseProperty("setting_values")]
    public virtual setting_definition setting_definition { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("setting_values")]
    public virtual tenant tenant { get; set; } = null!;
}
