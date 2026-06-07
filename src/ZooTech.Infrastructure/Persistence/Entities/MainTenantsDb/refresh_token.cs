using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("token", Name = "UQ__refresh___CA90DA7AC0D8FA8D", IsUnique = true)]
public partial class refresh_token
{
    [Key]
    public long id { get; set; }

    public int admin_user_id { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string token { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? jwt_id { get; set; }

    public bool is_revoked { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime expires_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? revoked_at { get; set; }

    [ForeignKey("admin_user_id")]
    [InverseProperty("refresh_tokens")]
    public virtual admin_user admin_user { get; set; } = null!;
}
