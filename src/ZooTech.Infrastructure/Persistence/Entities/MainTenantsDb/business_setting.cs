using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("code", Name = "UQ__business__357D4CF96440C9C1", IsUnique = true)]
public partial class business_setting
{
    [Key]
    public long id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string code { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string? description { get; set; }

    public bool is_active { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [InverseProperty("business_setting")]
    public virtual ICollection<business_setting_parameter> business_setting_parameters { get; set; } = new List<business_setting_parameter>();
}
