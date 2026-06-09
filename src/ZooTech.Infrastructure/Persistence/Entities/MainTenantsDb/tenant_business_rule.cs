using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[PrimaryKey("tenant_id", "rule_definition_id")]
[Index("rule_definition_id", Name = "IX_tenant_business_rules_rule_definition_id")]
public partial class tenant_business_rule
{
    [Key]
    public long tenant_id { get; set; }

    [Key]
    public long rule_definition_id { get; set; }

    public bool is_active { get; set; }

    public int? priority { get; set; }

    public int? rule_version { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? execution_mode { get; set; }

    public string? custom_condition { get; set; }

    public string? custom_action { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [ForeignKey("rule_definition_id")]
    [InverseProperty("tenant_business_rules")]
    public virtual rule_definition rule_definition { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("tenant_business_rules")]
    public virtual tenant tenant { get; set; } = null!;
}
