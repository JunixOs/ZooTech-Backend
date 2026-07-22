using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("business_setting_parameter_id", Name = "IX_business_setting_parameter_values_parameter_id")]
[Index("business_setting_parameter_id", "actor_type", "actor_id", Name = "UQ_business_setting_parameter_values", IsUnique = true)]
public partial class business_setting_parameter_value
{
    [Key]
    public long id { get; set; }

    public long business_setting_parameter_id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string actor_type { get; set; } = null!;

    public long? actor_id { get; set; }

    public string value { get; set; } = null!;

    public bool is_active { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [ForeignKey("business_setting_parameter_id")]
    [InverseProperty("business_setting_parameter_values")]
    public virtual business_setting_parameter business_setting_parameter { get; set; } = null!;
}
