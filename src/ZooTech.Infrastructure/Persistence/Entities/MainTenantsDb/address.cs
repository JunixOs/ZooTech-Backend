using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

public partial class address
{
    [Key]
    public long id { get; set; }

    public long tenant_id { get; set; }

    [StringLength(100)]
    public string country { get; set; } = null!;

    [StringLength(100)]
    public string state { get; set; } = null!;

    [StringLength(100)]
    public string province { get; set; } = null!;

    [StringLength(100)]
    public string city { get; set; } = null!;

    [StringLength(200)]
    public string address_line_1 { get; set; } = null!;

    [StringLength(200)]
    public string? address_line_2 { get; set; }

    public string? metadata { get; set; }

    [Precision(3)]
    public DateTimeOffset? created_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? updated_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? deleted_at { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("addresses")]
    public virtual tenant tenant { get; set; } = null!;
}
