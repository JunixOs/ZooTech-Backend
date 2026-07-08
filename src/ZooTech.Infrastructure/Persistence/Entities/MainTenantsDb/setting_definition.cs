using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("setting_group_id", "code", Name = "UX_setting_definitions_group_code", IsUnique = true)]
public partial class setting_definition
{
    [Key]
    public int id { get; set; }

    public int setting_group_id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string? description { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string data_type { get; set; } = null!;

    public string? default_value { get; set; }

    public string? validation_schema { get; set; }

    public bool is_required { get; set; }

    public bool is_sensitive { get; set; }

    public bool is_active { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? deleted_at { get; set; }

    [ForeignKey("setting_group_id")]
    [InverseProperty("setting_definitions")]
    public virtual setting_group setting_group { get; set; } = null!;

    [InverseProperty("setting_definition")]
    public virtual ICollection<setting_value> setting_values { get; set; } = new List<setting_value>();
}
