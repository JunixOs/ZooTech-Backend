using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Models;

namespace ZooTech.Infrastructure.Persistence.Context;

public partial class GanaderiaDbContext : DbContext
{
    public GanaderiaDbContext(DbContextOptions<GanaderiaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<archivo> archivos { get; set; }

    public virtual DbSet<bitacora_auditorium> bitacora_auditoria { get; set; }

    public virtual DbSet<cat_caracteristica_celo> cat_caracteristica_celos { get; set; }

    public virtual DbSet<cat_color> cat_colors { get; set; }

    public virtual DbSet<cat_estado_celo> cat_estado_celos { get; set; }

    public virtual DbSet<cat_estado_fecundacion_vacuno> cat_estado_fecundacion_vacunos { get; set; }

    public virtual DbSet<cat_estado_ordenio> cat_estado_ordenios { get; set; }

    public virtual DbSet<cat_estado_registro> cat_estado_registros { get; set; }

    public virtual DbSet<cat_estado_sequium> cat_estado_sequia { get; set; }

    public virtual DbSet<cat_estado_trazabilidad> cat_estado_trazabilidads { get; set; }

    public virtual DbSet<cat_estado_vacuno> cat_estado_vacunos { get; set; }

    public virtual DbSet<cat_formato_reporte> cat_formato_reportes { get; set; }

    public virtual DbSet<cat_modulo> cat_modulos { get; set; }

    public virtual DbSet<cat_raza> cat_razas { get; set; }

    public virtual DbSet<cat_resultado_fecundacion> cat_resultado_fecundacions { get; set; }

    public virtual DbSet<cat_sexo> cat_sexos { get; set; }

    public virtual DbSet<cat_tipo_adquisicion> cat_tipo_adquisicions { get; set; }

    public virtual DbSet<cat_tipo_archivo> cat_tipo_archivos { get; set; }

    public virtual DbSet<cat_tipo_fecundacion> cat_tipo_fecundacions { get; set; }

    public virtual DbSet<cat_tipo_peso> cat_tipo_pesos { get; set; }

    public virtual DbSet<cat_tipo_reporte> cat_tipo_reportes { get; set; }

    public virtual DbSet<cat_tipo_responsable> cat_tipo_responsables { get; set; }

    public virtual DbSet<cat_tipo_utilizacion> cat_tipo_utilizacions { get; set; }

    public virtual DbSet<celo_configuracion> celo_configuracions { get; set; }

    public virtual DbSet<celo_registro> celo_registros { get; set; }

    public virtual DbSet<celo_registro_caracteristica_libre> celo_registro_caracteristica_libres { get; set; }

    public virtual DbSet<fecundacion> fecundacions { get; set; }

    public virtual DbSet<fecundacion_crium> fecundacion_cria { get; set; }

    public virtual DbSet<fecundacion_donante> fecundacion_donantes { get; set; }

    public virtual DbSet<fecundacion_embrion> fecundacion_embrions { get; set; }

    public virtual DbSet<fecundacion_inseminacion> fecundacion_inseminacions { get; set; }

    public virtual DbSet<geo_departamento> geo_departamentos { get; set; }

    public virtual DbSet<geo_distrito> geo_distritos { get; set; }

    public virtual DbSet<geo_provincium> geo_provincia { get; set; }

    public virtual DbSet<granja> granjas { get; set; }

    public virtual DbSet<incidente_vacuno> incidente_vacunos { get; set; }

    public virtual DbSet<ordenio> ordenios { get; set; }

    public virtual DbSet<parametro_sistema> parametro_sistemas { get; set; }

    public virtual DbSet<periodo_sequium> periodo_sequia { get; set; }

    public virtual DbSet<produccion_leche_estandar> produccion_leche_estandars { get; set; }

    public virtual DbSet<reporte_descarga> reporte_descargas { get; set; }

    public virtual DbSet<reproductor_externo> reproductor_externos { get; set; }

    public virtual DbSet<responsable> responsables { get; set; }

    public virtual DbSet<triaje> triajes { get; set; }

    public virtual DbSet<usuario> usuarios { get; set; }

    public virtual DbSet<v_celo_estado_vacuno> v_celo_estado_vacunos { get; set; }

    public virtual DbSet<v_produccion_leche_estandar_vigente> v_produccion_leche_estandar_vigentes { get; set; }

    public virtual DbSet<v_vacuno_estado_vigente> v_vacuno_estado_vigentes { get; set; }

    public virtual DbSet<v_vacuno_utilizacion_vigente> v_vacuno_utilizacion_vigentes { get; set; }

    public virtual DbSet<vacuno> vacunos { get; set; }

    public virtual DbSet<vacuno_adquisicion> vacuno_adquisicions { get; set; }

    public virtual DbSet<vacuno_estado_fecundacion_historial> vacuno_estado_fecundacion_historials { get; set; }

    public virtual DbSet<vacuno_estado_historial> vacuno_estado_historials { get; set; }

    public virtual DbSet<vacuno_foto> vacuno_fotos { get; set; }

    public virtual DbSet<vacuno_utilizacion_historial> vacuno_utilizacion_historials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfiguration(new VacunoRawRowConfiguration());

        modelBuilder.Entity<archivo>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__archivo__3213E83FC88C9300");

            entity.HasOne(d => d.creado_porNavigation).WithMany(p => p.archivos).HasConstraintName("fk_archivo_creado_por");

            entity.HasOne(d => d.extensionNavigation).WithMany(p => p.archivos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_archivo_extension");

            entity.HasOne(d => d.modulo_codeNavigation).WithMany(p => p.archivos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_archivo_modulo");
        });

        modelBuilder.Entity<bitacora_auditorium>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__bitacora__3213E83FFF8F8901");

            entity.HasOne(d => d.modulo_codeNavigation).WithMany(p => p.bitacora_auditoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bitacora_modulo");

            entity.HasOne(d => d.usuario).WithMany(p => p.bitacora_auditoria).HasConstraintName("fk_bitacora_usuario");
        });

        modelBuilder.Entity<cat_caracteristica_celo>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_cara__357D4CF858E76519");
        });

        modelBuilder.Entity<cat_color>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_colo__357D4CF894DF5306");
        });

        modelBuilder.Entity<cat_estado_celo>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_esta__357D4CF840685346");
        });

        modelBuilder.Entity<cat_estado_fecundacion_vacuno>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_esta__357D4CF838EB0524");
        });

        modelBuilder.Entity<cat_estado_ordenio>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_esta__357D4CF8FBA8C253");
        });

        modelBuilder.Entity<cat_estado_registro>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_esta__357D4CF8DC626498");
        });

        modelBuilder.Entity<cat_estado_sequium>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_esta__357D4CF8784A38BD");
        });

        modelBuilder.Entity<cat_estado_trazabilidad>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_esta__357D4CF833C0D6A2");
        });

        modelBuilder.Entity<cat_estado_vacuno>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_esta__357D4CF8811A8632");
        });

        modelBuilder.Entity<cat_formato_reporte>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_form__357D4CF85032BFA3");
        });

        modelBuilder.Entity<cat_modulo>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_modu__357D4CF859FEE271");
        });

        modelBuilder.Entity<cat_raza>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_raza__357D4CF8AEF2926E");
        });

        modelBuilder.Entity<cat_resultado_fecundacion>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_resu__357D4CF84DD5891D");
        });

        modelBuilder.Entity<cat_sexo>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_sexo__357D4CF81E3A02DA");
        });

        modelBuilder.Entity<cat_tipo_adquisicion>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_tipo__357D4CF84A5809D0");
        });

        modelBuilder.Entity<cat_tipo_archivo>(entity =>
        {
            entity.HasKey(e => e.extension).HasName("PK__cat_tipo__51EBE911EE7FD96A");
        });

        modelBuilder.Entity<cat_tipo_fecundacion>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_tipo__357D4CF89E9CC9C8");
        });

        modelBuilder.Entity<cat_tipo_peso>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_tipo__357D4CF8E03DBB3A");
        });

        modelBuilder.Entity<cat_tipo_reporte>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_tipo__357D4CF89CA3AAA9");

            entity.HasOne(d => d.modulo_codeNavigation).WithMany(p => p.cat_tipo_reportes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cat_tipo_reporte_modulo");
        });

        modelBuilder.Entity<cat_tipo_responsable>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_tipo__357D4CF80C263D30");
        });

        modelBuilder.Entity<cat_tipo_utilizacion>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_tipo__357D4CF841B174F0");
        });

        modelBuilder.Entity<celo_configuracion>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__celo_con__3213E83F01A0C7E2");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.celo_configuracions).HasConstraintName("fk_celo_config_created_by");
        });

        modelBuilder.Entity<celo_registro>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__celo_reg__3213E83FEC3B1926");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.celo_registrocreated_byNavigations).HasConstraintName("fk_celo_registro_created_by");

            entity.HasOne(d => d.deleted_byNavigation).WithMany(p => p.celo_registrodeleted_byNavigations).HasConstraintName("fk_celo_registro_deleted_by");

            entity.HasOne(d => d.encargado_usuario).WithMany(p => p.celo_registroencargado_usuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_celo_registro_encargado");

            entity.HasOne(d => d.estado_registro_codeNavigation).WithMany(p => p.celo_registros)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_celo_registro_estado");

            entity.HasOne(d => d.updated_byNavigation).WithMany(p => p.celo_registroupdated_byNavigations).HasConstraintName("fk_celo_registro_updated_by");

            entity.HasOne(d => d.vacuno).WithMany(p => p.celo_registros)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_celo_registro_vacuno");

            entity.HasMany(d => d.caracteristica_codes).WithMany(p => p.celo_registros)
                .UsingEntity<Dictionary<string, object>>(
                    "celo_registro_caracteristica",
                    r => r.HasOne<cat_caracteristica_celo>().WithMany()
                        .HasForeignKey("caracteristica_code")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_crc_caracteristica"),
                    l => l.HasOne<celo_registro>().WithMany()
                        .HasForeignKey("celo_registro_id")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_crc_celo_registro"),
                    j =>
                    {
                        j.HasKey("celo_registro_id", "caracteristica_code").HasName("pk_celo_reg_caract");
                        j.ToTable("celo_registro_caracteristica");
                        j.HasIndex(new[] { "caracteristica_code" }, "idx_crc_caracteristica");
                        j.HasIndex(new[] { "celo_registro_id" }, "idx_crc_celo_registro");
                        j.IndexerProperty<string>("caracteristica_code")
                            .HasMaxLength(30)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<celo_registro_caracteristica_libre>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__celo_reg__3213E83F9964464C");

            entity.HasOne(d => d.celo_registro).WithMany(p => p.celo_registro_caracteristica_libres)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_crcl_celo_registro");
        });

        modelBuilder.Entity<fecundacion>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__fecundac__3213E83F293328D0");

            entity.HasOne(d => d.celo_registro).WithMany(p => p.fecundacions).HasConstraintName("fk_fecundacion_celo");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.fecundacioncreated_byNavigations).HasConstraintName("fk_fecundacion_created_by");

            entity.HasOne(d => d.responsable).WithMany(p => p.fecundacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fecundacion_responsable");

            entity.HasOne(d => d.resultado_codeNavigation).WithMany(p => p.fecundacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fecundacion_resultado");

            entity.HasOne(d => d.tipo_fecundacion_codeNavigation).WithMany(p => p.fecundacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fecundacion_tipo");

            entity.HasOne(d => d.updated_byNavigation).WithMany(p => p.fecundacionupdated_byNavigations).HasConstraintName("fk_fecundacion_updated_by");

            entity.HasOne(d => d.vacuno_receptor).WithMany(p => p.fecundacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fecundacion_receptor");
        });

        modelBuilder.Entity<fecundacion_crium>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__fecundac__3213E83F49BCDB39");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.fecundacion_criumcreated_byNavigations).HasConstraintName("fk_fc_created_by");

            entity.HasOne(d => d.estado_trazabilidad_codeNavigation).WithMany(p => p.fecundacion_cria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fc_trazabilidad");

            entity.HasOne(d => d.fecundacion).WithMany(p => p.fecundacion_cria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fc_fecundacion");

            entity.HasOne(d => d.updated_byNavigation).WithMany(p => p.fecundacion_criumupdated_byNavigations).HasConstraintName("fk_fc_updated_by");

            entity.HasOne(d => d.vacuno_hijo).WithMany(p => p.fecundacion_cria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fc_vacuno_hijo");
        });

        modelBuilder.Entity<fecundacion_donante>(entity =>
        {
            entity.HasKey(e => e.fecundacion_id).HasName("pk_fecundacion_donante");

            entity.Property(e => e.fecundacion_id).ValueGeneratedNever();

            entity.HasOne(d => d.externo_donante).WithMany(p => p.fecundacion_donantes).HasConstraintName("fk_fd_externo_donante");

            entity.HasOne(d => d.fecundacion).WithOne(p => p.fecundacion_donante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fd_fecundacion");

            entity.HasOne(d => d.vacuno_donante).WithMany(p => p.fecundacion_donantes).HasConstraintName("fk_fd_vacuno_donante");
        });

        modelBuilder.Entity<fecundacion_embrion>(entity =>
        {
            entity.HasKey(e => e.fecundacion_id).HasName("PK__fecundac__A6B542F27EAB98D1");

            entity.Property(e => e.fecundacion_id).ValueGeneratedNever();

            entity.HasOne(d => d.fecundacion).WithOne(p => p.fecundacion_embrion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_femb_fecundacion");
        });

        modelBuilder.Entity<fecundacion_inseminacion>(entity =>
        {
            entity.HasKey(e => e.fecundacion_id).HasName("PK__fecundac__A6B542F22BFDDD0A");

            entity.Property(e => e.fecundacion_id).ValueGeneratedNever();

            entity.HasOne(d => d.fecundacion).WithOne(p => p.fecundacion_inseminacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fins_fecundacion");
        });

        modelBuilder.Entity<geo_departamento>(entity =>
        {
            entity.HasKey(e => e.codigo).HasName("PK__geo_depa__40F9A20746960A5A");
        });

        modelBuilder.Entity<geo_distrito>(entity =>
        {
            entity.HasKey(e => e.codigo).HasName("PK__geo_dist__40F9A207408F138A");

            entity.HasOne(d => d.provincia_codigoNavigation).WithMany(p => p.geo_distritos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_distrito_provincia");
        });

        modelBuilder.Entity<geo_provincium>(entity =>
        {
            entity.HasKey(e => e.codigo).HasName("PK__geo_prov__40F9A207128DD835");

            entity.HasOne(d => d.departamento_codigoNavigation).WithMany(p => p.geo_provincia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_provincia_departamento");
        });

        modelBuilder.Entity<granja>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__granja__3213E83F4767B2FF");

            entity.HasOne(d => d.distrito_codigoNavigation).WithMany(p => p.granjas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_granja_distrito");
        });

        modelBuilder.Entity<incidente_vacuno>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__incident__3213E83FF80BE653");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.incidente_vacunos).HasConstraintName("fk_incidente_created");

            entity.HasOne(d => d.estado_registro_codeNavigation).WithMany(p => p.incidente_vacunos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_incidente_estado");

            entity.HasOne(d => d.vacuno).WithMany(p => p.incidente_vacunos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_incidente_vacuno");
        });

        modelBuilder.Entity<ordenio>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ordenio__3213E83FBA23CB4D");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.ordeniocreated_byNavigations).HasConstraintName("fk_ordenio_created_by");

            entity.HasOne(d => d.deleted_byNavigation).WithMany(p => p.ordeniodeleted_byNavigations).HasConstraintName("fk_ordenio_deleted_by");

            entity.HasOne(d => d.encargado_usuario).WithMany(p => p.ordenioencargado_usuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ordenio_encargado");

            entity.HasOne(d => d.estado_ordenio_codeNavigation).WithMany(p => p.ordenios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ordenio_estado");

            entity.HasOne(d => d.updated_byNavigation).WithMany(p => p.ordenioupdated_byNavigations).HasConstraintName("fk_ordenio_updated_by");

            entity.HasOne(d => d.vacuno).WithMany(p => p.ordenios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ordenio_vacuno");
        });

        modelBuilder.Entity<parametro_sistema>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__parametr__3213E83F4E6A83B1");

            entity.HasOne(d => d.modulo_codeNavigation).WithMany(p => p.parametro_sistemas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_parametro_modulo");
        });

        modelBuilder.Entity<periodo_sequium>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__periodo___3213E83FB780762A");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.periodo_sequiumcreated_byNavigations).HasConstraintName("fk_sequia_created_by");

            entity.HasOne(d => d.estado_sequia_codeNavigation).WithMany(p => p.periodo_sequia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sequia_estado");

            entity.HasOne(d => d.updated_byNavigation).WithMany(p => p.periodo_sequiumupdated_byNavigations).HasConstraintName("fk_sequia_updated_by");

            entity.HasOne(d => d.vacuno).WithMany(p => p.periodo_sequia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sequia_vacuno");
        });

        modelBuilder.Entity<produccion_leche_estandar>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__producci__3213E83F3A3B7013");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.produccion_leche_estandars).HasConstraintName("fk_ple_created_by");

            entity.HasOne(d => d.vacuno).WithMany(p => p.produccion_leche_estandars).HasConstraintName("fk_ple_vacuno");
        });

        modelBuilder.Entity<reporte_descarga>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__reporte___3213E83F3EB28E0B");

            entity.HasOne(d => d.archivo).WithMany(p => p.reporte_descargas).HasConstraintName("fk_reporte_archivo");

            entity.HasOne(d => d.estado_codeNavigation).WithMany(p => p.reporte_descargas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reporte_estado");

            entity.HasOne(d => d.formato_codeNavigation).WithMany(p => p.reporte_descargas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reporte_formato");

            entity.HasOne(d => d.solicitado_porNavigation).WithMany(p => p.reporte_descargas).HasConstraintName("fk_reporte_solicitado_por");

            entity.HasOne(d => d.tipo_reporte_codeNavigation).WithMany(p => p.reporte_descargas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reporte_tipo_reporte");
        });

        modelBuilder.Entity<reproductor_externo>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__reproduc__3213E83F73DA3FD4");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.reproductor_externos).HasConstraintName("fk_reproductor_created_by");

            entity.HasOne(d => d.sexo_codeNavigation).WithMany(p => p.reproductor_externos).HasConstraintName("fk_reproductor_sexo");
        });

        modelBuilder.Entity<responsable>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__responsa__3213E83FAAA888EC");

            entity.HasOne(d => d.tipo_responsable_codeNavigation).WithMany(p => p.responsables)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_responsable_tipo");

            entity.HasOne(d => d.usuario).WithMany(p => p.responsables).HasConstraintName("fk_responsable_usuario");
        });

        modelBuilder.Entity<triaje>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__triaje__3213E83F8AC578E0");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.triajecreated_byNavigations).HasConstraintName("fk_triaje_created_by");

            entity.HasOne(d => d.deleted_byNavigation).WithMany(p => p.triajedeleted_byNavigations).HasConstraintName("fk_triaje_deleted_by");

            entity.HasOne(d => d.encargado_usuario).WithMany(p => p.triajeencargado_usuarios).HasConstraintName("fk_triaje_encargado");

            entity.HasOne(d => d.estado_registro_codeNavigation).WithMany(p => p.triajes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_triaje_estado");

            entity.HasOne(d => d.tipo_peso_codeNavigation).WithMany(p => p.triajes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_triaje_tipo_peso");

            entity.HasOne(d => d.updated_byNavigation).WithMany(p => p.triajeupdated_byNavigations).HasConstraintName("fk_triaje_updated_by");

            entity.HasOne(d => d.vacuno).WithMany(p => p.triajes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_triaje_vacuno");
        });

        modelBuilder.Entity<usuario>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__usuario__3213E83F94B0B488");
        });

        modelBuilder.Entity<v_celo_estado_vacuno>(entity =>
        {
            entity.ToView("v_celo_estado_vacuno");
        });

        modelBuilder.Entity<v_produccion_leche_estandar_vigente>(entity =>
        {
            entity.ToView("v_produccion_leche_estandar_vigente");
        });

        modelBuilder.Entity<v_vacuno_estado_vigente>(entity =>
        {
            entity.ToView("v_vacuno_estado_vigente");
        });

        modelBuilder.Entity<v_vacuno_utilizacion_vigente>(entity =>
        {
            entity.ToView("v_vacuno_utilizacion_vigente");
        });

        modelBuilder.Entity<vacuno>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__vacuno__3213E83F977B081D");

            entity.HasOne(d => d.color_codeNavigation).WithMany(p => p.vacunos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vacuno_color");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.vacunocreated_byNavigations).HasConstraintName("fk_vacuno_created_by");

            entity.HasOne(d => d.deleted_byNavigation).WithMany(p => p.vacunodeleted_byNavigations).HasConstraintName("fk_vacuno_deleted_by");

            entity.HasOne(d => d.granja).WithMany(p => p.vacunos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vacuno_granja");

            entity.HasOne(d => d.madre).WithMany(p => p.Inversemadre).HasConstraintName("fk_vacuno_madre");

            entity.HasOne(d => d.padre).WithMany(p => p.Inversepadre).HasConstraintName("fk_vacuno_padre");

            entity.HasOne(d => d.raza_codeNavigation).WithMany(p => p.vacunos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vacuno_raza");

            entity.HasOne(d => d.sexo_codeNavigation).WithMany(p => p.vacunos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vacuno_sexo");

            entity.HasOne(d => d.tipo_adquisicion_codeNavigation).WithMany(p => p.vacunos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vacuno_adquisicion");

            entity.HasOne(d => d.updated_byNavigation).WithMany(p => p.vacunoupdated_byNavigations).HasConstraintName("fk_vacuno_updated_by");
        });

        modelBuilder.Entity<vacuno_adquisicion>(entity =>
        {
            entity.HasKey(e => e.vacuno_id).HasName("PK__vacuno_a__2ACE7902C49F7DA4");

            entity.Property(e => e.vacuno_id).ValueGeneratedNever();

            entity.HasOne(d => d.tipo_adquisicion_codeNavigation).WithMany(p => p.vacuno_adquisicions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vacuno_adquisicion_tipo");

            entity.HasOne(d => d.vacuno).WithOne(p => p.vacuno_adquisicion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vacuno_adquisicion_vacuno");
        });

        modelBuilder.Entity<vacuno_estado_fecundacion_historial>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__vacuno_e__3213E83F9B76A954");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.vacuno_estado_fecundacion_historialcreated_byNavigations).HasConstraintName("fk_vefh_created_by");

            entity.HasOne(d => d.deleted_byNavigation).WithMany(p => p.vacuno_estado_fecundacion_historialdeleted_byNavigations).HasConstraintName("fk_vefh_deleted_by");

            entity.HasOne(d => d.estado_fecundacion_codeNavigation).WithMany(p => p.vacuno_estado_fecundacion_historials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vefh_estado");

            entity.HasOne(d => d.fecundacion).WithMany(p => p.vacuno_estado_fecundacion_historials).HasConstraintName("fk_vefh_fecundacion");

            entity.HasOne(d => d.vacuno).WithMany(p => p.vacuno_estado_fecundacion_historials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vefh_vacuno");
        });

        modelBuilder.Entity<vacuno_estado_historial>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__vacuno_e__3213E83FE8860E56");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.vacuno_estado_historials).HasConstraintName("fk_veh_created");

            entity.HasOne(d => d.estado_codeNavigation).WithMany(p => p.vacuno_estado_historials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_veh_estado");

            entity.HasOne(d => d.vacuno).WithMany(p => p.vacuno_estado_historials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_veh_vacuno");
        });

        modelBuilder.Entity<vacuno_foto>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__vacuno_f__3213E83FEA9E2F9C");

            entity.HasOne(d => d.archivo).WithMany(p => p.vacuno_fotos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vacuno_foto_archivo");

            entity.HasOne(d => d.uploaded_byNavigation).WithMany(p => p.vacuno_fotos).HasConstraintName("fk_vacuno_foto_uploaded");

            entity.HasOne(d => d.vacuno).WithOne(p => p.vacuno_foto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vacuno_foto_vacuno");
        });

        modelBuilder.Entity<vacuno_utilizacion_historial>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__vacuno_u__3213E83FCB7F78F0");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.vacuno_utilizacion_historials).HasConstraintName("fk_vuh_created_by");

            entity.HasOne(d => d.tipo_utilizacion_codeNavigation).WithMany(p => p.vacuno_utilizacion_historials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vuh_utilizacion");

            entity.HasOne(d => d.vacuno).WithMany(p => p.vacuno_utilizacion_historials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vuh_vacuno");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
