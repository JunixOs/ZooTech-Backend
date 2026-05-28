using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("email", Name = "UX_admin_users_email", IsUnique = true)]
public partial class admin_user
{
    [Key]
    public long id { get; set; }

    [StringLength(255)]
    public string name { get; set; } = null!;

    [StringLength(255)]
    public string email { get; set; } = null!;

    [StringLength(1000)]
    public string password_hash { get; set; } = null!;

    [StringLength(100)]
    public string? role { get; set; }

    public bool is_active { get; set; }

    public DateTime? last_login_at { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [InverseProperty("admin_user")]
    public virtual ICollection<refresh_token> refresh_tokens { get; set; } = new List<refresh_token>();
}
