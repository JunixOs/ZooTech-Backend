using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("tenant_id", Name = "UQ__addresse__D6F29F3F7BCB37A9", IsUnique = true)]
public partial class address
{
    [Key]
    public int id { get; set; }

    public int tenant_id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string country { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string state { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string province { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string city { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string address_line_1 { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? address_line_2 { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? deleted_at { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("address")]
    public virtual tenant tenant { get; set; } = null!;
}
