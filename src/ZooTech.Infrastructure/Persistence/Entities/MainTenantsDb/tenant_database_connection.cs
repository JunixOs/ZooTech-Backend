using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("tenant_id", Name = "UQ__tenant_d__D6F29F3F276C576F", IsUnique = true)]
public partial class tenant_database_connection
{
    [Key]
    public long id { get; set; }

    public long tenant_id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? database_name { get; set; }

    public bool is_active { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("tenant_database_connection")]
    public virtual tenant tenant { get; set; } = null!;
}
