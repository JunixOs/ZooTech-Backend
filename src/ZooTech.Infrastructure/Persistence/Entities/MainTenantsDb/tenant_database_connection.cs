using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("tenant_id", Name = "UQ__tenant_d__D6F29F3FBD889E7C", IsUnique = true)]
public partial class tenant_database_connection
{
    [Key]
    public int id { get; set; }

    public int tenant_id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string database_name { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? server_name { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? provider { get; set; }

    [Unicode(false)]
    public string? connection_string_encrypted { get; set; }

    public bool is_active { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? deleted_at { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("tenant_database_connection")]
    public virtual tenant tenant { get; set; } = null!;
}
