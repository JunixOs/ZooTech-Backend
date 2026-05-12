using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("reporte_descarga")]
[Index("estado_code", Name = "idx_reporte_estado")]
[Index("filtro_fecha_inicio", "filtro_fecha_fin", Name = "idx_reporte_fechas")]
[Index("formato_code", Name = "idx_reporte_formato")]
[Index("filtro_granja_id", Name = "idx_reporte_granja")]
[Index("solicitado_por", Name = "idx_reporte_solicitado")]
[Index("tipo_reporte_code", Name = "idx_reporte_tipo")]
[Index("filtro_vacuno_id", Name = "idx_reporte_vacuno")]
public partial class reporte_descarga
{
    [Key]
    public long id { get; set; }

    [StringLength(60)]
    [Unicode(false)]
    public string tipo_reporte_code { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string formato_code { get; set; } = null!;

    public DateOnly? filtro_fecha_inicio { get; set; }

    public DateOnly? filtro_fecha_fin { get; set; }

    public long? filtro_vacuno_id { get; set; }

    public long? filtro_granja_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? filtro_raza_code { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? filtro_palabra_clave { get; set; }

    public long? archivo_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string estado_code { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? mensaje { get; set; }

    public long? solicitado_por { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("archivo_id")]
    [InverseProperty("reporte_descargas")]
    public virtual archivo? archivo { get; set; }

    [ForeignKey("estado_code")]
    [InverseProperty("reporte_descargas")]
    public virtual cat_estado_registro estado_codeNavigation { get; set; } = null!;

    [ForeignKey("formato_code")]
    [InverseProperty("reporte_descargas")]
    public virtual cat_formato_reporte formato_codeNavigation { get; set; } = null!;

    [ForeignKey("solicitado_por")]
    [InverseProperty("reporte_descargas")]
    public virtual usuario? solicitado_porNavigation { get; set; }

    [ForeignKey("tipo_reporte_code")]
    [InverseProperty("reporte_descargas")]
    public virtual cat_tipo_reporte tipo_reporte_codeNavigation { get; set; } = null!;
}
