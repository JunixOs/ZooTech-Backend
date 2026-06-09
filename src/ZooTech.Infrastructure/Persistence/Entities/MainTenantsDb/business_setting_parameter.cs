using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("business_setting_id", Name = "IX_business_setting_parameters_business_setting_id")]
[Index("business_setting_id", "code", Name = "UQ_business_setting_parameters", IsUnique = true)]
public partial class business_setting_parameter
{
    [Key]
    public long id { get; set; }

    public long business_setting_id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string data_type { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string? description { get; set; }

    public string? default_value { get; set; }

    public string? validation_schema { get; set; }

    public bool is_required { get; set; }

    public bool is_sensitive { get; set; }

    public bool is_active { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [ForeignKey("business_setting_id")]
    [InverseProperty("business_setting_parameters")]
    public virtual business_setting business_setting { get; set; } = null!;

    [InverseProperty("business_setting_parameter")]
    public virtual ICollection<business_setting_parameter_value> business_setting_parameter_values { get; set; } = new List<business_setting_parameter_value>();
}
