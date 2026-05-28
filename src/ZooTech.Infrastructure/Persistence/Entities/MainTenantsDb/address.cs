using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("tenant_id", Name = "UQ__addresse__D6F29F3F9442B267", IsUnique = true)]
public partial class address
{
    [Key]
    public long id { get; set; }

    public long tenant_id { get; set; }

    [StringLength(150)]
    public string country { get; set; } = null!;

    [StringLength(150)]
    public string state { get; set; } = null!;

    [StringLength(150)]
    public string province { get; set; } = null!;

    [StringLength(150)]
    public string city { get; set; } = null!;

    [StringLength(500)]
    public string address_line1 { get; set; } = null!;

    [StringLength(500)]
    public string? address_line2 { get; set; }

    public string? metadata { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("address")]
    public virtual tenant tenant { get; set; } = null!;
}
