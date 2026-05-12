using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("archivo")]
[Index("extension", Name = "idx_archivo_extension")]
[Index("hash_sha256", Name = "idx_archivo_hash")]
[Index("modulo_code", Name = "idx_archivo_modulo")]
public partial class archivo
{
    [Key]
    public long id { get; set; }

    [StringLength(40)]
    [Unicode(false)]
    public string modulo_code { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string nombre_original { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string nombre_almacenado { get; set; } = null!;

    public string ruta_archivo { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string extension { get; set; } = null!;

    public long? tamano_bytes { get; set; }

    [StringLength(64)]
    [Unicode(false)]
    public string? hash_sha256 { get; set; }

    public long? creado_por { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("creado_por")]
    [InverseProperty("archivos")]
    public virtual usuario? creado_porNavigation { get; set; }

    [ForeignKey("extension")]
    [InverseProperty("archivos")]
    public virtual cat_tipo_archivo extensionNavigation { get; set; } = null!;

    [ForeignKey("modulo_code")]
    [InverseProperty("archivos")]
    public virtual cat_modulo modulo_codeNavigation { get; set; } = null!;

    [InverseProperty("archivo")]
    public virtual ICollection<reporte_descarga> reporte_descargas { get; set; } = new List<reporte_descarga>();

    [InverseProperty("archivo")]
    public virtual ICollection<vacuno_foto> vacuno_fotos { get; set; } = new List<vacuno_foto>();
}
