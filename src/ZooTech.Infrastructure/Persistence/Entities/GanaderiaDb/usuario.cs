using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Entities;

[Table("usuario")]
[Index("activo", Name = "idx_usuario_activo")]
[Index("codigo", Name = "uq_usuario_codigo", IsUnique = true)]
[Index("correo", Name = "uq_usuario_correo", IsUnique = true)]
[Index("nombre_usuario", Name = "uq_usuario_nombre_usuario", IsUnique = true)]
public partial class usuario
{
    [Key]
    public long id { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string codigo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string nombre_usuario { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string nombre_completo { get; set; } = null!;

    [StringLength(120)]
    [Unicode(false)]
    public string? correo { get; set; }

    public bool activo { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [InverseProperty("creado_porNavigation")]
    public virtual ICollection<archivo> archivos { get; set; } = new List<archivo>();

    [InverseProperty("usuario")]
    public virtual ICollection<bitacora_auditorium> bitacora_auditoria { get; set; } = new List<bitacora_auditorium>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<celo_configuracion> celo_configuracions { get; set; } = new List<celo_configuracion>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<celo_registro> celo_registrocreated_byNavigations { get; set; } = new List<celo_registro>();

    [InverseProperty("deleted_byNavigation")]
    public virtual ICollection<celo_registro> celo_registrodeleted_byNavigations { get; set; } = new List<celo_registro>();

    [InverseProperty("encargado_usuario")]
    public virtual ICollection<celo_registro> celo_registroencargado_usuarios { get; set; } = new List<celo_registro>();

    [InverseProperty("updated_byNavigation")]
    public virtual ICollection<celo_registro> celo_registroupdated_byNavigations { get; set; } = new List<celo_registro>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<fecundacion_crium> fecundacion_criumcreated_byNavigations { get; set; } = new List<fecundacion_crium>();

    [InverseProperty("updated_byNavigation")]
    public virtual ICollection<fecundacion_crium> fecundacion_criumupdated_byNavigations { get; set; } = new List<fecundacion_crium>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<fecundacion> fecundacioncreated_byNavigations { get; set; } = new List<fecundacion>();

    [InverseProperty("updated_byNavigation")]
    public virtual ICollection<fecundacion> fecundacionupdated_byNavigations { get; set; } = new List<fecundacion>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<incidente_vacuno> incidente_vacunos { get; set; } = new List<incidente_vacuno>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<ordenio> ordeniocreated_byNavigations { get; set; } = new List<ordenio>();

    [InverseProperty("deleted_byNavigation")]
    public virtual ICollection<ordenio> ordeniodeleted_byNavigations { get; set; } = new List<ordenio>();

    [InverseProperty("encargado_usuario")]
    public virtual ICollection<ordenio> ordenioencargado_usuarios { get; set; } = new List<ordenio>();

    [InverseProperty("updated_byNavigation")]
    public virtual ICollection<ordenio> ordenioupdated_byNavigations { get; set; } = new List<ordenio>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<periodo_sequium> periodo_sequiumcreated_byNavigations { get; set; } = new List<periodo_sequium>();

    [InverseProperty("updated_byNavigation")]
    public virtual ICollection<periodo_sequium> periodo_sequiumupdated_byNavigations { get; set; } = new List<periodo_sequium>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<produccion_leche_estandar> produccion_leche_estandars { get; set; } = new List<produccion_leche_estandar>();

    [InverseProperty("solicitado_porNavigation")]
    public virtual ICollection<reporte_descarga> reporte_descargas { get; set; } = new List<reporte_descarga>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<reproductor_externo> reproductor_externos { get; set; } = new List<reproductor_externo>();

    [InverseProperty("usuario")]
    public virtual ICollection<responsable> responsables { get; set; } = new List<responsable>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<triaje> triajecreated_byNavigations { get; set; } = new List<triaje>();

    [InverseProperty("deleted_byNavigation")]
    public virtual ICollection<triaje> triajedeleted_byNavigations { get; set; } = new List<triaje>();

    [InverseProperty("encargado_usuario")]
    public virtual ICollection<triaje> triajeencargado_usuarios { get; set; } = new List<triaje>();

    [InverseProperty("updated_byNavigation")]
    public virtual ICollection<triaje> triajeupdated_byNavigations { get; set; } = new List<triaje>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<vacuno_estado_fecundacion_historial> vacuno_estado_fecundacion_historialcreated_byNavigations { get; set; } = new List<vacuno_estado_fecundacion_historial>();

    [InverseProperty("deleted_byNavigation")]
    public virtual ICollection<vacuno_estado_fecundacion_historial> vacuno_estado_fecundacion_historialdeleted_byNavigations { get; set; } = new List<vacuno_estado_fecundacion_historial>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<vacuno_estado_historial> vacuno_estado_historials { get; set; } = new List<vacuno_estado_historial>();

    [InverseProperty("uploaded_byNavigation")]
    public virtual ICollection<vacuno_foto> vacuno_fotos { get; set; } = new List<vacuno_foto>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<vacuno_utilizacion_historial> vacuno_utilizacion_historials { get; set; } = new List<vacuno_utilizacion_historial>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<vacuno> vacunocreated_byNavigations { get; set; } = new List<vacuno>();

    [InverseProperty("deleted_byNavigation")]
    public virtual ICollection<vacuno> vacunodeleted_byNavigations { get; set; } = new List<vacuno>();

    [InverseProperty("updated_byNavigation")]
    public virtual ICollection<vacuno> vacunoupdated_byNavigations { get; set; } = new List<vacuno>();
}
