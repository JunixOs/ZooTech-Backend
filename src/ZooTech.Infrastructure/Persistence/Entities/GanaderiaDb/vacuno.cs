using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("vacuno")]
[Index("deleted_at", Name = "idx_vacuno_deleted_at")]
[Index("fecha_nacimiento", Name = "idx_vacuno_fecha_nacimiento")]
[Index("fecha_registro", Name = "idx_vacuno_fecha_registro")]
[Index("granja_id", Name = "idx_vacuno_granja")]
[Index("madre_id", Name = "idx_vacuno_madre")]
[Index("nombre", Name = "idx_vacuno_nombre")]
[Index("padre_id", Name = "idx_vacuno_padre")]
[Index("raza_code", Name = "idx_vacuno_raza")]
[Index("sexo_code", Name = "idx_vacuno_sexo")]
[Index("codigo", Name = "uq_vacuno_codigo", IsUnique = true)]
public partial class vacuno
{
    [Key]
    public long id { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string codigo { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    public DateOnly fecha_nacimiento { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string tipo_adquisicion_code { get; set; } = null!;

    [StringLength(30)]
    [Unicode(false)]
    public string raza_code { get; set; } = null!;

    [StringLength(30)]
    [Unicode(false)]
    public string color_code { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string sexo_code { get; set; } = null!;

    public long? padre_id { get; set; }

    public long? madre_id { get; set; }

    public long granja_id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? observaciones { get; set; }

    public DateOnly fecha_registro { get; set; }

    public long? created_by { get; set; }

    public long? updated_by { get; set; }

    public long? deleted_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? motivo_eliminacion { get; set; }

    [InverseProperty("madre")]
    public virtual ICollection<vacuno> Inversemadre { get; set; } = new List<vacuno>();

    [InverseProperty("padre")]
    public virtual ICollection<vacuno> Inversepadre { get; set; } = new List<vacuno>();

    [InverseProperty("vacuno")]
    public virtual ICollection<celo_registro> celo_registros { get; set; } = new List<celo_registro>();

    [ForeignKey("color_code")]
    [InverseProperty("vacunos")]
    public virtual cat_color color_codeNavigation { get; set; } = null!;

    [ForeignKey("created_by")]
    [InverseProperty("vacunocreated_byNavigations")]
    public virtual usuario? created_byNavigation { get; set; }

    [ForeignKey("deleted_by")]
    [InverseProperty("vacunodeleted_byNavigations")]
    public virtual usuario? deleted_byNavigation { get; set; }

    [InverseProperty("vacuno_hijo")]
    public virtual ICollection<fecundacion_crium> fecundacion_cria { get; set; } = new List<fecundacion_crium>();

    [InverseProperty("vacuno_donante")]
    public virtual ICollection<fecundacion_donante> fecundacion_donantes { get; set; } = new List<fecundacion_donante>();

    [InverseProperty("vacuno_receptor")]
    public virtual ICollection<fecundacion> fecundacions { get; set; } = new List<fecundacion>();

    [ForeignKey("granja_id")]
    [InverseProperty("vacunos")]
    public virtual granja granja { get; set; } = null!;

    [InverseProperty("vacuno")]
    public virtual ICollection<incidente_vacuno> incidente_vacunos { get; set; } = new List<incidente_vacuno>();

    [ForeignKey("madre_id")]
    [InverseProperty("Inversemadre")]
    public virtual vacuno? madre { get; set; }

    [InverseProperty("vacuno")]
    public virtual ICollection<ordenio> ordenios { get; set; } = new List<ordenio>();

    [ForeignKey("padre_id")]
    [InverseProperty("Inversepadre")]
    public virtual vacuno? padre { get; set; }

    [InverseProperty("vacuno")]
    public virtual ICollection<periodo_sequium> periodo_sequia { get; set; } = new List<periodo_sequium>();

    [InverseProperty("vacuno")]
    public virtual ICollection<produccion_leche_estandar> produccion_leche_estandars { get; set; } = new List<produccion_leche_estandar>();

    [ForeignKey("raza_code")]
    [InverseProperty("vacunos")]
    public virtual cat_raza raza_codeNavigation { get; set; } = null!;

    [ForeignKey("sexo_code")]
    [InverseProperty("vacunos")]
    public virtual cat_sexo sexo_codeNavigation { get; set; } = null!;

    [ForeignKey("tipo_adquisicion_code")]
    [InverseProperty("vacunos")]
    public virtual cat_tipo_adquisicion tipo_adquisicion_codeNavigation { get; set; } = null!;

    [InverseProperty("vacuno")]
    public virtual ICollection<triaje> triajes { get; set; } = new List<triaje>();

    [ForeignKey("updated_by")]
    [InverseProperty("vacunoupdated_byNavigations")]
    public virtual usuario? updated_byNavigation { get; set; }

    [InverseProperty("vacuno")]
    public virtual vacuno_adquisicion? vacuno_adquisicion { get; set; }

    [InverseProperty("vacuno")]
    public virtual ICollection<vacuno_estado_fecundacion_historial> vacuno_estado_fecundacion_historials { get; set; } = new List<vacuno_estado_fecundacion_historial>();

    [InverseProperty("vacuno")]
    public virtual ICollection<vacuno_estado_historial> vacuno_estado_historials { get; set; } = new List<vacuno_estado_historial>();

    [InverseProperty("vacuno")]
    public virtual vacuno_foto? vacuno_foto { get; set; }

    [InverseProperty("vacuno")]
    public virtual ICollection<vacuno_utilizacion_historial> vacuno_utilizacion_historials { get; set; } = new List<vacuno_utilizacion_historial>();
}
