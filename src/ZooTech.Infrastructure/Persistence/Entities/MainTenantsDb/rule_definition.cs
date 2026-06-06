using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("code", Name = "UQ__rule_def__357D4CF99F03CA87", IsUnique = true)]
public partial class rule_definition
{
    [Key]
    public long id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? code { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? name { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? module { get; set; }

    public string? condition_schema { get; set; }

    public string? action_schema { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [InverseProperty("rule_definition")]
    public virtual ICollection<tenant_business_rule> tenant_business_rules { get; set; } = new List<tenant_business_rule>();
}
