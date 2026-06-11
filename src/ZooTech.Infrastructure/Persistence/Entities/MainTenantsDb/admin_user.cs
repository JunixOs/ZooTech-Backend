using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

[Index("email", Name = "UQ__admin_us__AB6E6164D869032A", IsUnique = true)]
[Index("username", Name = "UQ__admin_us__F3DBC572049A4738", IsUnique = true)]
public partial class admin_user
{
    [Key]
    public long id { get; set; }

    [StringLength(150)]
    public string email { get; set; } = null!;

    [StringLength(100)]
    public string? username { get; set; }

    public string? password_hash { get; set; }

    [StringLength(100)]
    public string? first_name { get; set; }

    [StringLength(100)]
    public string? last_name { get; set; }

    public bool is_active { get; set; }

    [Precision(3)]
    public DateTimeOffset? last_login_at { get; set; }

    public string? metadata { get; set; }

    [Precision(3)]
    public DateTimeOffset? created_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? updated_at { get; set; }

    [Precision(3)]
    public DateTimeOffset? deleted_at { get; set; }

    [InverseProperty("admin_user")]
    public virtual ICollection<refresh_token> refresh_tokens { get; set; } = new List<refresh_token>();
}
