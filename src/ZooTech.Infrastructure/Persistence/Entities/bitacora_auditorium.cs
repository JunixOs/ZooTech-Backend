using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Index("created_at", Name = "idx_bitacora_created_at")]
[Index("entidad", Name = "idx_bitacora_entidad")]
[Index("entidad_id", Name = "idx_bitacora_entidad_id")]
[Index("modulo_code", Name = "idx_bitacora_modulo")]
[Index("usuario_id", Name = "idx_bitacora_usuario")]
public partial class bitacora_auditorium
{
    [Key]
    public long id { get; set; }

    [StringLength(40)]
    [Unicode(false)]
    public string modulo_code { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string entidad { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string entidad_id { get; set; } = null!;

    [StringLength(40)]
    [Unicode(false)]
    public string accion { get; set; } = null!;

    public string? datos_anteriores { get; set; }

    public string? datos_nuevos { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? motivo { get; set; }

    public long? usuario_id { get; set; }

    [StringLength(45)]
    public string? ip_origen { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? user_agent { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("modulo_code")]
    [InverseProperty("bitacora_auditoria")]
    public virtual cat_modulo modulo_codeNavigation { get; set; } = null!;

    [ForeignKey("usuario_id")]
    [InverseProperty("bitacora_auditoria")]
    public virtual usuario? usuario { get; set; }
}
