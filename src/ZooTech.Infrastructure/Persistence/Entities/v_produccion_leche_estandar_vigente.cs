using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Keyless]
public partial class v_produccion_leche_estandar_vigente
{
    public long? vacuno_id { get; set; }

    [Column(TypeName = "numeric(8, 3)")]
    public decimal litros_esperados_dia { get; set; }

    public DateOnly fecha_inicio { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? descripcion { get; set; }
}
