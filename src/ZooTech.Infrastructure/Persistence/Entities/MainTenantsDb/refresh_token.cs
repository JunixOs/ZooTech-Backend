using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("admin_user_id", Name = "IX_refresh_tokens_admin_user_id")]
[Index("expires_at", Name = "IX_refresh_tokens_expires_at")]
[Index("token", Name = "UQ__refresh___CA90DA7AAEA6B793", IsUnique = true)]
public partial class refresh_token
{
    [Key]
    public long id { get; set; }

    public long admin_user_id { get; set; }

    [StringLength(2000)]
    public string token { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? jwt_id { get; set; }

    public bool is_revoked { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime expires_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? revoked_at { get; set; }

    [ForeignKey("admin_user_id")]
    [InverseProperty("refresh_tokens")]
    public virtual admin_user admin_user { get; set; } = null!;
}
