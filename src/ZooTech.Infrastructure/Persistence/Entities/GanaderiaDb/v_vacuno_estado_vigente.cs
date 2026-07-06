using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Keyless]
public partial class v_vacuno_estado_vigente
{
    public long vacuno_id { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string estado_code { get; set; } = null!;

    public DateOnly fecha_estado { get; set; }

    public DateTime fecha_registro { get; set; }
}
