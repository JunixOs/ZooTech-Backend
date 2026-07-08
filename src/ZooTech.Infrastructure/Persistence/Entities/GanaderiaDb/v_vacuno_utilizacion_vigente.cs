using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Keyless]
public partial class v_vacuno_utilizacion_vigente
{
    public long vacuno_id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string tipo_utilizacion_code { get; set; } = null!;

    public DateTime fecha_desde { get; set; }
}
