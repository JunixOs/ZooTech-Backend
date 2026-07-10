using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("reproductor_externo")]
[Index("codigo_externo", Name = "idx_reproductor_codigo")]
[Index("nombre", Name = "idx_reproductor_nombre")]
public partial class reproductor_externo
{
    [Key]
    public long id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? codigo_externo { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? sexo_code { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    public bool activo { get; set; }

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("reproductor_externos")]
    public virtual usuario? created_byNavigation { get; set; }

    [InverseProperty("externo_donante")]
    public virtual ICollection<fecundacion_donante> fecundacion_donantes { get; set; } = new List<fecundacion_donante>();

    [ForeignKey("sexo_code")]
    [InverseProperty("reproductor_externos")]
    public virtual cat_sexo? sexo_codeNavigation { get; set; }
}
