using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Keyless]
public partial class v_celo_estado_vacuno
{
    public long vacuno_id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string vacuno_nombre { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string sexo_code { get; set; } = null!;

    public DateOnly? fecha_ultimo_celo { get; set; }

    public int? veces_celo { get; set; }

    public int? crias { get; set; }

    public DateOnly? fecha_proximo_celo_estimada { get; set; }

    public int? dias_restantes { get; set; }

    [StringLength(9)]
    [Unicode(false)]
    public string estado_celo_calculado { get; set; } = null!;
}
