using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("email", Name = "UQ__admin_us__AB6E61648CDDD8AC", IsUnique = true)]
[Index("username", Name = "UQ__admin_us__F3DBC57261271ED8", IsUnique = true)]
public partial class admin_user
{
    [Key]
    public long id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string email { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? username { get; set; }

    public string? password_hash { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? first_name { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? last_name { get; set; }

    public bool is_active { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? last_login_at { get; set; }

    public string? metadata { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime created_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? updated_at { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? deleted_at { get; set; }

    [InverseProperty("admin_user")]
    public virtual ICollection<refresh_token> refresh_tokens { get; set; } = new List<refresh_token>();
}
