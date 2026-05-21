using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZooTech.Infrastructure.Persistence.Migrations.Ganaderia
{
    /// <inheritdoc />
    public partial class InitialGanaderia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cat_caracteristica_celo",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_cara__357D4CF858E76519", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_color",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_colo__357D4CF894DF5306", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_estado_celo",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_esta__357D4CF840685346", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_estado_fecundacion_vacuno",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_esta__357D4CF838EB0524", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_estado_ordenio",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_esta__357D4CF8FBA8C253", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_estado_registro",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_esta__357D4CF8DC626498", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_estado_sequia",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_esta__357D4CF8784A38BD", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_estado_trazabilidad",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_esta__357D4CF833C0D6A2", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_estado_vacuno",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    nombre = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_esta__357D4CF8811A8632", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_formato_reporte",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    nombre = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_form__357D4CF85032BFA3", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_modulo",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    nombre = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_modu__357D4CF859FEE271", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_raza",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_raza__357D4CF8AEF2926E", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_resultado_fecundacion",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_resu__357D4CF84DD5891D", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_sexo",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    nombre = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_sexo__357D4CF81E3A02DA", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipo_adquisicion",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_tipo__357D4CF84A5809D0", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipo_archivo",
                columns: table => new
                {
                    extension = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    mime_type = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_tipo__51EBE911EE7FD96A", x => x.extension);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipo_fecundacion",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    nombre = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_tipo__357D4CF89E9CC9C8", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipo_peso",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_tipo__357D4CF8E03DBB3A", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipo_responsable",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_tipo__357D4CF80C263D30", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipo_utilizacion",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_tipo__357D4CF841B174F0", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "geo_departamento",
                columns: table => new
                {
                    codigo = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: false),
                    nombre = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__geo_depa__40F9A20746960A5A", x => x.codigo);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    nombre_usuario = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    nombre_completo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    correo = table.Column<string>(type: "varchar(120)", unicode: false, maxLength: 120, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__usuario__3213E83F94B0B488", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cat_tipo_reporte",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    modulo_code = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    nombre = table.Column<string>(type: "varchar(120)", unicode: false, maxLength: 120, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cat_tipo__357D4CF89CA3AAA9", x => x.code);
                    table.ForeignKey(
                        name: "fk_cat_tipo_reporte_modulo",
                        column: x => x.modulo_code,
                        principalTable: "cat_modulo",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "parametro_sistema",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    modulo_code = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    clave = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    valor = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__parametr__3213E83F4E6A83B1", x => x.id);
                    table.ForeignKey(
                        name: "fk_parametro_modulo",
                        column: x => x.modulo_code,
                        principalTable: "cat_modulo",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "geo_provincia",
                columns: table => new
                {
                    codigo = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: false),
                    departamento_codigo = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: false),
                    nombre = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__geo_prov__40F9A207128DD835", x => x.codigo);
                    table.ForeignKey(
                        name: "fk_provincia_departamento",
                        column: x => x.departamento_codigo,
                        principalTable: "geo_departamento",
                        principalColumn: "codigo");
                });

            migrationBuilder.CreateTable(
                name: "archivo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    modulo_code = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    nombre_original = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    nombre_almacenado = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    ruta_archivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    extension = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    tamano_bytes = table.Column<long>(type: "bigint", nullable: true),
                    hash_sha256 = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    creado_por = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__archivo__3213E83FC88C9300", x => x.id);
                    table.ForeignKey(
                        name: "fk_archivo_creado_por",
                        column: x => x.creado_por,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_archivo_extension",
                        column: x => x.extension,
                        principalTable: "cat_tipo_archivo",
                        principalColumn: "extension");
                    table.ForeignKey(
                        name: "fk_archivo_modulo",
                        column: x => x.modulo_code,
                        principalTable: "cat_modulo",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "bitacora_auditoria",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    modulo_code = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    entidad = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    entidad_id = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    accion = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    datos_anteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    datos_nuevos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    motivo = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    ip_origen = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__bitacora__3213E83FFF8F8901", x => x.id);
                    table.ForeignKey(
                        name: "fk_bitacora_modulo",
                        column: x => x.modulo_code,
                        principalTable: "cat_modulo",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_bitacora_usuario",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "celo_configuracion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    dias_ciclo_estandar = table.Column<int>(type: "int", nullable: false),
                    dias_alerta_previa = table.Column<int>(type: "int", nullable: false),
                    dias_tolerancia_posterior = table.Column<int>(type: "int", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__celo_con__3213E83F01A0C7E2", x => x.id);
                    table.ForeignKey(
                        name: "fk_celo_config_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "reproductor_externo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    codigo_externo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    sexo_code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__reproduc__3213E83F73DA3FD4", x => x.id);
                    table.ForeignKey(
                        name: "fk_reproductor_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reproductor_sexo",
                        column: x => x.sexo_code,
                        principalTable: "cat_sexo",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "responsable",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    tipo_responsable_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    nombre_completo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    documento = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__responsa__3213E83FAAA888EC", x => x.id);
                    table.ForeignKey(
                        name: "fk_responsable_tipo",
                        column: x => x.tipo_responsable_code,
                        principalTable: "cat_tipo_responsable",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_responsable_usuario",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "geo_distrito",
                columns: table => new
                {
                    codigo = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: false),
                    provincia_codigo = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: false),
                    nombre = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__geo_dist__40F9A207408F138A", x => x.codigo);
                    table.ForeignKey(
                        name: "fk_distrito_provincia",
                        column: x => x.provincia_codigo,
                        principalTable: "geo_provincia",
                        principalColumn: "codigo");
                });

            migrationBuilder.CreateTable(
                name: "reporte_descarga",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo_reporte_code = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    formato_code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    filtro_fecha_inicio = table.Column<DateOnly>(type: "date", nullable: true),
                    filtro_fecha_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    filtro_vacuno_id = table.Column<long>(type: "bigint", nullable: true),
                    filtro_granja_id = table.Column<long>(type: "bigint", nullable: true),
                    filtro_raza_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    filtro_palabra_clave = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    archivo_id = table.Column<long>(type: "bigint", nullable: true),
                    estado_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    mensaje = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    solicitado_por = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__reporte___3213E83F3EB28E0B", x => x.id);
                    table.ForeignKey(
                        name: "fk_reporte_archivo",
                        column: x => x.archivo_id,
                        principalTable: "archivo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reporte_estado",
                        column: x => x.estado_code,
                        principalTable: "cat_estado_registro",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_reporte_formato",
                        column: x => x.formato_code,
                        principalTable: "cat_formato_reporte",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_reporte_solicitado_por",
                        column: x => x.solicitado_por,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reporte_tipo_reporte",
                        column: x => x.tipo_reporte_code,
                        principalTable: "cat_tipo_reporte",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "granja",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    distrito_codigo = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__granja__3213E83F4767B2FF", x => x.id);
                    table.ForeignKey(
                        name: "fk_granja_distrito",
                        column: x => x.distrito_codigo,
                        principalTable: "geo_distrito",
                        principalColumn: "codigo");
                });

            migrationBuilder.CreateTable(
                name: "vacuno",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    tipo_adquisicion_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    raza_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    color_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    sexo_code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    padre_id = table.Column<long>(type: "bigint", nullable: true),
                    madre_id = table.Column<long>(type: "bigint", nullable: true),
                    granja_id = table.Column<long>(type: "bigint", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    fecha_registro = table.Column<DateOnly>(type: "date", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    motivo_eliminacion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vacuno__3213E83F977B081D", x => x.id);
                    table.ForeignKey(
                        name: "fk_vacuno_adquisicion",
                        column: x => x.tipo_adquisicion_code,
                        principalTable: "cat_tipo_adquisicion",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_vacuno_color",
                        column: x => x.color_code,
                        principalTable: "cat_color",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_vacuno_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vacuno_deleted_by",
                        column: x => x.deleted_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vacuno_granja",
                        column: x => x.granja_id,
                        principalTable: "granja",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vacuno_madre",
                        column: x => x.madre_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vacuno_padre",
                        column: x => x.padre_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vacuno_raza",
                        column: x => x.raza_code,
                        principalTable: "cat_raza",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_vacuno_sexo",
                        column: x => x.sexo_code,
                        principalTable: "cat_sexo",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_vacuno_updated_by",
                        column: x => x.updated_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "celo_registro",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    vacuno_id = table.Column<long>(type: "bigint", nullable: false),
                    encargado_usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    estado_registro_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    motivo_eliminacion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__celo_reg__3213E83FEC3B1926", x => x.id);
                    table.ForeignKey(
                        name: "fk_celo_registro_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_celo_registro_deleted_by",
                        column: x => x.deleted_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_celo_registro_encargado",
                        column: x => x.encargado_usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_celo_registro_estado",
                        column: x => x.estado_registro_code,
                        principalTable: "cat_estado_registro",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_celo_registro_updated_by",
                        column: x => x.updated_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_celo_registro_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "incidente_vacuno",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vacuno_id = table.Column<long>(type: "bigint", nullable: false),
                    fecha_incidente = table.Column<DateOnly>(type: "date", nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    estado_registro_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__incident__3213E83FF80BE653", x => x.id);
                    table.ForeignKey(
                        name: "fk_incidente_created",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_incidente_estado",
                        column: x => x.estado_registro_code,
                        principalTable: "cat_estado_registro",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_incidente_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ordenio",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    vacuno_id = table.Column<long>(type: "bigint", nullable: false),
                    encargado_usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    litros = table.Column<decimal>(type: "numeric(8,3)", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    estado_ordenio_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    motivo_eliminacion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ordenio__3213E83FBA23CB4D", x => x.id);
                    table.ForeignKey(
                        name: "fk_ordenio_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ordenio_deleted_by",
                        column: x => x.deleted_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ordenio_encargado",
                        column: x => x.encargado_usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ordenio_estado",
                        column: x => x.estado_ordenio_code,
                        principalTable: "cat_estado_ordenio",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_ordenio_updated_by",
                        column: x => x.updated_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ordenio_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "periodo_sequia",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vacuno_id = table.Column<long>(type: "bigint", nullable: false),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin_estimada = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_fin_real = table.Column<DateOnly>(type: "date", nullable: true),
                    estado_sequia_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    motivo = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    observaciones = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__periodo___3213E83FB780762A", x => x.id);
                    table.ForeignKey(
                        name: "fk_sequia_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_sequia_estado",
                        column: x => x.estado_sequia_code,
                        principalTable: "cat_estado_sequia",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_sequia_updated_by",
                        column: x => x.updated_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_sequia_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "produccion_leche_estandar",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vacuno_id = table.Column<long>(type: "bigint", nullable: true),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    litros_esperados_dia = table.Column<decimal>(type: "numeric(8,3)", nullable: false),
                    descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__producci__3213E83F3A3B7013", x => x.id);
                    table.ForeignKey(
                        name: "fk_ple_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ple_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "triaje",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    vacuno_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo_peso_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    peso_kg = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    estado_registro_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    encargado_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    motivo_eliminacion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__triaje__3213E83F8AC578E0", x => x.id);
                    table.ForeignKey(
                        name: "fk_triaje_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_triaje_deleted_by",
                        column: x => x.deleted_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_triaje_encargado",
                        column: x => x.encargado_usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_triaje_estado",
                        column: x => x.estado_registro_code,
                        principalTable: "cat_estado_registro",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_triaje_tipo_peso",
                        column: x => x.tipo_peso_code,
                        principalTable: "cat_tipo_peso",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_triaje_updated_by",
                        column: x => x.updated_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_triaje_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "vacuno_adquisicion",
                columns: table => new
                {
                    vacuno_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo_adquisicion_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    precio_compra = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    fecha_adquisicion = table.Column<DateOnly>(type: "date", nullable: false),
                    proveedor = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    observaciones = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vacuno_a__2ACE7902C49F7DA4", x => x.vacuno_id);
                    table.ForeignKey(
                        name: "fk_vacuno_adquisicion_tipo",
                        column: x => x.tipo_adquisicion_code,
                        principalTable: "cat_tipo_adquisicion",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_vacuno_adquisicion_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "vacuno_estado_historial",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vacuno_id = table.Column<long>(type: "bigint", nullable: false),
                    estado_code = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    fecha_estado = table.Column<DateOnly>(type: "date", nullable: false),
                    motivo = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vacuno_e__3213E83FE8860E56", x => x.id);
                    table.ForeignKey(
                        name: "fk_veh_created",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_veh_estado",
                        column: x => x.estado_code,
                        principalTable: "cat_estado_vacuno",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_veh_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "vacuno_foto",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vacuno_id = table.Column<long>(type: "bigint", nullable: false),
                    archivo_id = table.Column<long>(type: "bigint", nullable: false),
                    es_principal = table.Column<bool>(type: "bit", nullable: false),
                    uploaded_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vacuno_f__3213E83FEA9E2F9C", x => x.id);
                    table.ForeignKey(
                        name: "fk_vacuno_foto_archivo",
                        column: x => x.archivo_id,
                        principalTable: "archivo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vacuno_foto_uploaded",
                        column: x => x.uploaded_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vacuno_foto_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "vacuno_utilizacion_historial",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vacuno_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo_utilizacion_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    motivo = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vacuno_u__3213E83FCB7F78F0", x => x.id);
                    table.ForeignKey(
                        name: "fk_vuh_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vuh_utilizacion",
                        column: x => x.tipo_utilizacion_code,
                        principalTable: "cat_tipo_utilizacion",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_vuh_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "celo_registro_caracteristica",
                columns: table => new
                {
                    celo_registro_id = table.Column<long>(type: "bigint", nullable: false),
                    caracteristica_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_celo_reg_caract", x => new { x.celo_registro_id, x.caracteristica_code });
                    table.ForeignKey(
                        name: "fk_crc_caracteristica",
                        column: x => x.caracteristica_code,
                        principalTable: "cat_caracteristica_celo",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_crc_celo_registro",
                        column: x => x.celo_registro_id,
                        principalTable: "celo_registro",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "celo_registro_caracteristica_libre",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    celo_registro_id = table.Column<long>(type: "bigint", nullable: false),
                    descripcion = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__celo_reg__3213E83F9964464C", x => x.id);
                    table.ForeignKey(
                        name: "fk_crcl_celo_registro",
                        column: x => x.celo_registro_id,
                        principalTable: "celo_registro",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "fecundacion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    tipo_fecundacion_code = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    vacuno_receptor_id = table.Column<long>(type: "bigint", nullable: false),
                    celo_registro_id = table.Column<long>(type: "bigint", nullable: true),
                    fecha_procedimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    responsable_id = table.Column<long>(type: "bigint", nullable: false),
                    resultado_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    observaciones_veterinarias = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__fecundac__3213E83F293328D0", x => x.id);
                    table.ForeignKey(
                        name: "fk_fecundacion_celo",
                        column: x => x.celo_registro_id,
                        principalTable: "celo_registro",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_fecundacion_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_fecundacion_receptor",
                        column: x => x.vacuno_receptor_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_fecundacion_responsable",
                        column: x => x.responsable_id,
                        principalTable: "responsable",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_fecundacion_resultado",
                        column: x => x.resultado_code,
                        principalTable: "cat_resultado_fecundacion",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_fecundacion_tipo",
                        column: x => x.tipo_fecundacion_code,
                        principalTable: "cat_tipo_fecundacion",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_fecundacion_updated_by",
                        column: x => x.updated_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "fecundacion_cria",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fecundacion_id = table.Column<long>(type: "bigint", nullable: false),
                    vacuno_hijo_id = table.Column<long>(type: "bigint", nullable: false),
                    estado_trazabilidad_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    fecha_vinculacion = table.Column<DateOnly>(type: "date", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__fecundac__3213E83F49BCDB39", x => x.id);
                    table.ForeignKey(
                        name: "fk_fc_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_fc_fecundacion",
                        column: x => x.fecundacion_id,
                        principalTable: "fecundacion",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_fc_trazabilidad",
                        column: x => x.estado_trazabilidad_code,
                        principalTable: "cat_estado_trazabilidad",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_fc_updated_by",
                        column: x => x.updated_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_fc_vacuno_hijo",
                        column: x => x.vacuno_hijo_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "fecundacion_donante",
                columns: table => new
                {
                    fecundacion_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo_donante = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    vacuno_donante_id = table.Column<long>(type: "bigint", nullable: true),
                    externo_donante_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fecundacion_donante", x => x.fecundacion_id);
                    table.ForeignKey(
                        name: "fk_fd_externo_donante",
                        column: x => x.externo_donante_id,
                        principalTable: "reproductor_externo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_fd_fecundacion",
                        column: x => x.fecundacion_id,
                        principalTable: "fecundacion",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_fd_vacuno_donante",
                        column: x => x.vacuno_donante_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "fecundacion_embrion",
                columns: table => new
                {
                    fecundacion_id = table.Column<long>(type: "bigint", nullable: false),
                    codigo_embrion = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__fecundac__A6B542F27EAB98D1", x => x.fecundacion_id);
                    table.ForeignKey(
                        name: "fk_femb_fecundacion",
                        column: x => x.fecundacion_id,
                        principalTable: "fecundacion",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "fecundacion_inseminacion",
                columns: table => new
                {
                    fecundacion_id = table.Column<long>(type: "bigint", nullable: false),
                    codigo_semen = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__fecundac__A6B542F22BFDDD0A", x => x.fecundacion_id);
                    table.ForeignKey(
                        name: "fk_fins_fecundacion",
                        column: x => x.fecundacion_id,
                        principalTable: "fecundacion",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "vacuno_estado_fecundacion_historial",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vacuno_id = table.Column<long>(type: "bigint", nullable: false),
                    estado_fecundacion_code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    fecundacion_id = table.Column<long>(type: "bigint", nullable: true),
                    fecha_actualizacion = table.Column<DateOnly>(type: "date", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    motivo_eliminacion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vacuno_e__3213E83F9B76A954", x => x.id);
                    table.ForeignKey(
                        name: "fk_vefh_created_by",
                        column: x => x.created_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vefh_deleted_by",
                        column: x => x.deleted_by,
                        principalTable: "usuario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vefh_estado",
                        column: x => x.estado_fecundacion_code,
                        principalTable: "cat_estado_fecundacion_vacuno",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "fk_vefh_fecundacion",
                        column: x => x.fecundacion_id,
                        principalTable: "fecundacion",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vefh_vacuno",
                        column: x => x.vacuno_id,
                        principalTable: "vacuno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "idx_archivo_extension",
                table: "archivo",
                column: "extension");

            migrationBuilder.CreateIndex(
                name: "idx_archivo_hash",
                table: "archivo",
                column: "hash_sha256");

            migrationBuilder.CreateIndex(
                name: "idx_archivo_modulo",
                table: "archivo",
                column: "modulo_code");

            migrationBuilder.CreateIndex(
                name: "IX_archivo_creado_por",
                table: "archivo",
                column: "creado_por");

            migrationBuilder.CreateIndex(
                name: "idx_bitacora_created_at",
                table: "bitacora_auditoria",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_bitacora_entidad",
                table: "bitacora_auditoria",
                column: "entidad");

            migrationBuilder.CreateIndex(
                name: "idx_bitacora_entidad_id",
                table: "bitacora_auditoria",
                column: "entidad_id");

            migrationBuilder.CreateIndex(
                name: "idx_bitacora_modulo",
                table: "bitacora_auditoria",
                column: "modulo_code");

            migrationBuilder.CreateIndex(
                name: "idx_bitacora_usuario",
                table: "bitacora_auditoria",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "uq_cat_caracteristica_celo_nombre",
                table: "cat_caracteristica_celo",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_color_nombre",
                table: "cat_color",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_estado_celo_nombre",
                table: "cat_estado_celo",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_estado_fec_vacuno_nombre",
                table: "cat_estado_fecundacion_vacuno",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_estado_ordenio_nombre",
                table: "cat_estado_ordenio",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_estado_registro_nombre",
                table: "cat_estado_registro",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_estado_sequia_nombre",
                table: "cat_estado_sequia",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_estado_trazabilidad_nombre",
                table: "cat_estado_trazabilidad",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_estado_vacuno_nombre",
                table: "cat_estado_vacuno",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_formato_reporte_nombre",
                table: "cat_formato_reporte",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_modulo_nombre",
                table: "cat_modulo",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_raza_nombre",
                table: "cat_raza",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_resultado_fecundacion_nombre",
                table: "cat_resultado_fecundacion",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_sexo_nombre",
                table: "cat_sexo",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_tipo_adquisicion_nombre",
                table: "cat_tipo_adquisicion",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_tipo_fecundacion_nombre",
                table: "cat_tipo_fecundacion",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_tipo_peso_nombre",
                table: "cat_tipo_peso",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_cat_tipo_reporte_modulo",
                table: "cat_tipo_reporte",
                column: "modulo_code");

            migrationBuilder.CreateIndex(
                name: "uq_cat_tipo_reporte_modulo_nombre",
                table: "cat_tipo_reporte",
                columns: new[] { "modulo_code", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_tipo_responsable_nombre",
                table: "cat_tipo_responsable",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cat_tipo_utilizacion_nombre",
                table: "cat_tipo_utilizacion",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_celo_config_activo",
                table: "celo_configuracion",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_celo_configuracion_created_by",
                table: "celo_configuracion",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "idx_celo_registro_deleted_at",
                table: "celo_registro",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "idx_celo_registro_encargado",
                table: "celo_registro",
                column: "encargado_usuario_id");

            migrationBuilder.CreateIndex(
                name: "idx_celo_registro_estado",
                table: "celo_registro",
                column: "estado_registro_code");

            migrationBuilder.CreateIndex(
                name: "idx_celo_registro_fecha_hora",
                table: "celo_registro",
                column: "fecha_hora");

            migrationBuilder.CreateIndex(
                name: "idx_celo_registro_vacuno",
                table: "celo_registro",
                column: "vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_celo_registro_created_by",
                table: "celo_registro",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_celo_registro_deleted_by",
                table: "celo_registro",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_celo_registro_updated_by",
                table: "celo_registro",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "uq_celo_registro_codigo",
                table: "celo_registro",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_celo_registro_vacuno_fecha",
                table: "celo_registro",
                columns: new[] { "vacuno_id", "fecha_hora" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_crc_caracteristica",
                table: "celo_registro_caracteristica",
                column: "caracteristica_code");

            migrationBuilder.CreateIndex(
                name: "idx_crc_celo_registro",
                table: "celo_registro_caracteristica",
                column: "celo_registro_id");

            migrationBuilder.CreateIndex(
                name: "idx_crcl_celo_registro",
                table: "celo_registro_caracteristica_libre",
                column: "celo_registro_id");

            migrationBuilder.CreateIndex(
                name: "idx_fecundacion_celo",
                table: "fecundacion",
                column: "celo_registro_id");

            migrationBuilder.CreateIndex(
                name: "idx_fecundacion_fecha",
                table: "fecundacion",
                column: "fecha_procedimiento");

            migrationBuilder.CreateIndex(
                name: "idx_fecundacion_receptor",
                table: "fecundacion",
                column: "vacuno_receptor_id");

            migrationBuilder.CreateIndex(
                name: "idx_fecundacion_responsable",
                table: "fecundacion",
                column: "responsable_id");

            migrationBuilder.CreateIndex(
                name: "idx_fecundacion_resultado",
                table: "fecundacion",
                column: "resultado_code");

            migrationBuilder.CreateIndex(
                name: "idx_fecundacion_tipo",
                table: "fecundacion",
                column: "tipo_fecundacion_code");

            migrationBuilder.CreateIndex(
                name: "IX_fecundacion_created_by",
                table: "fecundacion",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_fecundacion_updated_by",
                table: "fecundacion",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "uq_fecundacion_codigo",
                table: "fecundacion",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_fc_fecundacion",
                table: "fecundacion_cria",
                column: "fecundacion_id");

            migrationBuilder.CreateIndex(
                name: "idx_fc_trazabilidad",
                table: "fecundacion_cria",
                column: "estado_trazabilidad_code");

            migrationBuilder.CreateIndex(
                name: "idx_fc_vacuno_hijo",
                table: "fecundacion_cria",
                column: "vacuno_hijo_id");

            migrationBuilder.CreateIndex(
                name: "IX_fecundacion_cria_created_by",
                table: "fecundacion_cria",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_fecundacion_cria_updated_by",
                table: "fecundacion_cria",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "uq_fecundacion_cria_fec_vacuno",
                table: "fecundacion_cria",
                columns: new[] { "fecundacion_id", "vacuno_hijo_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_fd_externo_donante",
                table: "fecundacion_donante",
                column: "externo_donante_id");

            migrationBuilder.CreateIndex(
                name: "idx_fd_vacuno_donante",
                table: "fecundacion_donante",
                column: "vacuno_donante_id");

            migrationBuilder.CreateIndex(
                name: "uq_geo_departamento_nombre",
                table: "geo_departamento",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_geo_distrito_prov_nombre",
                table: "geo_distrito",
                columns: new[] { "provincia_codigo", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_geo_provincia_dep_nombre",
                table: "geo_provincia",
                columns: new[] { "departamento_codigo", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_granja_distrito",
                table: "granja",
                column: "distrito_codigo");

            migrationBuilder.CreateIndex(
                name: "uq_granja_nombre_distrito",
                table: "granja",
                columns: new[] { "nombre", "distrito_codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_incidente_estado",
                table: "incidente_vacuno",
                column: "estado_registro_code");

            migrationBuilder.CreateIndex(
                name: "idx_incidente_fecha",
                table: "incidente_vacuno",
                column: "fecha_incidente");

            migrationBuilder.CreateIndex(
                name: "idx_incidente_vacuno",
                table: "incidente_vacuno",
                column: "vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_incidente_vacuno_created_by",
                table: "incidente_vacuno",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "idx_ordenio_deleted_at",
                table: "ordenio",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "idx_ordenio_encargado",
                table: "ordenio",
                column: "encargado_usuario_id");

            migrationBuilder.CreateIndex(
                name: "idx_ordenio_estado",
                table: "ordenio",
                column: "estado_ordenio_code");

            migrationBuilder.CreateIndex(
                name: "idx_ordenio_fecha_hora",
                table: "ordenio",
                column: "fecha_hora");

            migrationBuilder.CreateIndex(
                name: "idx_ordenio_vacuno",
                table: "ordenio",
                column: "vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordenio_created_by",
                table: "ordenio",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_ordenio_deleted_by",
                table: "ordenio",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_ordenio_updated_by",
                table: "ordenio",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "uq_ordenio_codigo",
                table: "ordenio",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_ordenio_vacuno_fecha",
                table: "ordenio",
                columns: new[] { "vacuno_id", "fecha_hora" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_parametro_modulo_clave",
                table: "parametro_sistema",
                columns: new[] { "modulo_code", "clave" });

            migrationBuilder.CreateIndex(
                name: "uq_parametro_modulo_clave",
                table: "parametro_sistema",
                columns: new[] { "modulo_code", "clave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sequia_estado",
                table: "periodo_sequia",
                column: "estado_sequia_code");

            migrationBuilder.CreateIndex(
                name: "idx_sequia_fecha_fin_e",
                table: "periodo_sequia",
                column: "fecha_fin_estimada");

            migrationBuilder.CreateIndex(
                name: "idx_sequia_fecha_fin_r",
                table: "periodo_sequia",
                column: "fecha_fin_real");

            migrationBuilder.CreateIndex(
                name: "idx_sequia_fecha_ini",
                table: "periodo_sequia",
                column: "fecha_inicio");

            migrationBuilder.CreateIndex(
                name: "idx_sequia_vacuno",
                table: "periodo_sequia",
                column: "vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_periodo_sequia_created_by",
                table: "periodo_sequia",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_periodo_sequia_updated_by",
                table: "periodo_sequia",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "idx_ple_fecha_fin",
                table: "produccion_leche_estandar",
                column: "fecha_fin");

            migrationBuilder.CreateIndex(
                name: "idx_ple_fecha_ini",
                table: "produccion_leche_estandar",
                column: "fecha_inicio");

            migrationBuilder.CreateIndex(
                name: "idx_ple_vacuno",
                table: "produccion_leche_estandar",
                column: "vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_produccion_leche_estandar_created_by",
                table: "produccion_leche_estandar",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "idx_reporte_estado",
                table: "reporte_descarga",
                column: "estado_code");

            migrationBuilder.CreateIndex(
                name: "idx_reporte_fechas",
                table: "reporte_descarga",
                columns: new[] { "filtro_fecha_inicio", "filtro_fecha_fin" });

            migrationBuilder.CreateIndex(
                name: "idx_reporte_formato",
                table: "reporte_descarga",
                column: "formato_code");

            migrationBuilder.CreateIndex(
                name: "idx_reporte_granja",
                table: "reporte_descarga",
                column: "filtro_granja_id");

            migrationBuilder.CreateIndex(
                name: "idx_reporte_solicitado",
                table: "reporte_descarga",
                column: "solicitado_por");

            migrationBuilder.CreateIndex(
                name: "idx_reporte_tipo",
                table: "reporte_descarga",
                column: "tipo_reporte_code");

            migrationBuilder.CreateIndex(
                name: "idx_reporte_vacuno",
                table: "reporte_descarga",
                column: "filtro_vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_reporte_descarga_archivo_id",
                table: "reporte_descarga",
                column: "archivo_id");

            migrationBuilder.CreateIndex(
                name: "idx_reproductor_codigo",
                table: "reproductor_externo",
                column: "codigo_externo");

            migrationBuilder.CreateIndex(
                name: "idx_reproductor_nombre",
                table: "reproductor_externo",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "IX_reproductor_externo_created_by",
                table: "reproductor_externo",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_reproductor_externo_sexo_code",
                table: "reproductor_externo",
                column: "sexo_code");

            migrationBuilder.CreateIndex(
                name: "idx_responsable_tipo",
                table: "responsable",
                column: "tipo_responsable_code");

            migrationBuilder.CreateIndex(
                name: "idx_responsable_usuario",
                table: "responsable",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "idx_triaje_deleted_at",
                table: "triaje",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "idx_triaje_estado",
                table: "triaje",
                column: "estado_registro_code");

            migrationBuilder.CreateIndex(
                name: "idx_triaje_fecha_hora",
                table: "triaje",
                column: "fecha_hora");

            migrationBuilder.CreateIndex(
                name: "idx_triaje_tipo_peso",
                table: "triaje",
                column: "tipo_peso_code");

            migrationBuilder.CreateIndex(
                name: "idx_triaje_vacuno",
                table: "triaje",
                column: "vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_triaje_created_by",
                table: "triaje",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_triaje_deleted_by",
                table: "triaje",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_triaje_encargado_usuario_id",
                table: "triaje",
                column: "encargado_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_triaje_updated_by",
                table: "triaje",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "uq_triaje_codigo",
                table: "triaje",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_triaje_vacuno_fecha_tipo",
                table: "triaje",
                columns: new[] { "vacuno_id", "fecha_hora", "tipo_peso_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_usuario_activo",
                table: "usuario",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "uq_usuario_codigo",
                table: "usuario",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_usuario_correo",
                table: "usuario",
                column: "correo",
                unique: true,
                filter: "[correo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "uq_usuario_nombre_usuario",
                table: "usuario",
                column: "nombre_usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_deleted_at",
                table: "vacuno",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_fecha_nacimiento",
                table: "vacuno",
                column: "fecha_nacimiento");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_fecha_registro",
                table: "vacuno",
                column: "fecha_registro");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_granja",
                table: "vacuno",
                column: "granja_id");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_madre",
                table: "vacuno",
                column: "madre_id");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_nombre",
                table: "vacuno",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_padre",
                table: "vacuno",
                column: "padre_id");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_raza",
                table: "vacuno",
                column: "raza_code");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_sexo",
                table: "vacuno",
                column: "sexo_code");

            migrationBuilder.CreateIndex(
                name: "IX_vacuno_color_code",
                table: "vacuno",
                column: "color_code");

            migrationBuilder.CreateIndex(
                name: "IX_vacuno_created_by",
                table: "vacuno",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_vacuno_deleted_by",
                table: "vacuno",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_vacuno_tipo_adquisicion_code",
                table: "vacuno",
                column: "tipo_adquisicion_code");

            migrationBuilder.CreateIndex(
                name: "IX_vacuno_updated_by",
                table: "vacuno",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "uq_vacuno_codigo",
                table: "vacuno",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_adquisicion_fecha",
                table: "vacuno_adquisicion",
                column: "fecha_adquisicion");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_adquisicion_tipo",
                table: "vacuno_adquisicion",
                column: "tipo_adquisicion_code");

            migrationBuilder.CreateIndex(
                name: "idx_vefh_created_at",
                table: "vacuno_estado_fecundacion_historial",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_vefh_deleted_at",
                table: "vacuno_estado_fecundacion_historial",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "idx_vefh_estado",
                table: "vacuno_estado_fecundacion_historial",
                column: "estado_fecundacion_code");

            migrationBuilder.CreateIndex(
                name: "idx_vefh_fecha",
                table: "vacuno_estado_fecundacion_historial",
                column: "fecha_actualizacion");

            migrationBuilder.CreateIndex(
                name: "idx_vefh_fecundacion",
                table: "vacuno_estado_fecundacion_historial",
                column: "fecundacion_id");

            migrationBuilder.CreateIndex(
                name: "idx_vefh_vacuno",
                table: "vacuno_estado_fecundacion_historial",
                column: "vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_vacuno_estado_fecundacion_historial_created_by",
                table: "vacuno_estado_fecundacion_historial",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_vacuno_estado_fecundacion_historial_deleted_by",
                table: "vacuno_estado_fecundacion_historial",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "idx_veh_created_at",
                table: "vacuno_estado_historial",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_veh_estado",
                table: "vacuno_estado_historial",
                column: "estado_code");

            migrationBuilder.CreateIndex(
                name: "idx_veh_fecha",
                table: "vacuno_estado_historial",
                column: "fecha_estado");

            migrationBuilder.CreateIndex(
                name: "idx_veh_vacuno",
                table: "vacuno_estado_historial",
                column: "vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_vacuno_estado_historial_created_by",
                table: "vacuno_estado_historial",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_foto_archivo",
                table: "vacuno_foto",
                column: "archivo_id");

            migrationBuilder.CreateIndex(
                name: "idx_vacuno_foto_vacuno",
                table: "vacuno_foto",
                column: "vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_vacuno_foto_uploaded_by",
                table: "vacuno_foto",
                column: "uploaded_by");

            migrationBuilder.CreateIndex(
                name: "uq_vacuno_foto_principal",
                table: "vacuno_foto",
                column: "vacuno_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_vuh_created_at",
                table: "vacuno_utilizacion_historial",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_vuh_tipo",
                table: "vacuno_utilizacion_historial",
                column: "tipo_utilizacion_code");

            migrationBuilder.CreateIndex(
                name: "idx_vuh_vacuno",
                table: "vacuno_utilizacion_historial",
                column: "vacuno_id");

            migrationBuilder.CreateIndex(
                name: "IX_vacuno_utilizacion_historial_created_by",
                table: "vacuno_utilizacion_historial",
                column: "created_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bitacora_auditoria");

            migrationBuilder.DropTable(
                name: "cat_estado_celo");

            migrationBuilder.DropTable(
                name: "celo_configuracion");

            migrationBuilder.DropTable(
                name: "celo_registro_caracteristica");

            migrationBuilder.DropTable(
                name: "celo_registro_caracteristica_libre");

            migrationBuilder.DropTable(
                name: "fecundacion_cria");

            migrationBuilder.DropTable(
                name: "fecundacion_donante");

            migrationBuilder.DropTable(
                name: "fecundacion_embrion");

            migrationBuilder.DropTable(
                name: "fecundacion_inseminacion");

            migrationBuilder.DropTable(
                name: "incidente_vacuno");

            migrationBuilder.DropTable(
                name: "ordenio");

            migrationBuilder.DropTable(
                name: "parametro_sistema");

            migrationBuilder.DropTable(
                name: "periodo_sequia");

            migrationBuilder.DropTable(
                name: "produccion_leche_estandar");

            migrationBuilder.DropTable(
                name: "reporte_descarga");

            migrationBuilder.DropTable(
                name: "triaje");

            migrationBuilder.DropTable(
                name: "vacuno_adquisicion");

            migrationBuilder.DropTable(
                name: "vacuno_estado_fecundacion_historial");

            migrationBuilder.DropTable(
                name: "vacuno_estado_historial");

            migrationBuilder.DropTable(
                name: "vacuno_foto");

            migrationBuilder.DropTable(
                name: "vacuno_utilizacion_historial");

            migrationBuilder.DropTable(
                name: "cat_caracteristica_celo");

            migrationBuilder.DropTable(
                name: "cat_estado_trazabilidad");

            migrationBuilder.DropTable(
                name: "reproductor_externo");

            migrationBuilder.DropTable(
                name: "cat_estado_ordenio");

            migrationBuilder.DropTable(
                name: "cat_estado_sequia");

            migrationBuilder.DropTable(
                name: "cat_formato_reporte");

            migrationBuilder.DropTable(
                name: "cat_tipo_reporte");

            migrationBuilder.DropTable(
                name: "cat_tipo_peso");

            migrationBuilder.DropTable(
                name: "cat_estado_fecundacion_vacuno");

            migrationBuilder.DropTable(
                name: "fecundacion");

            migrationBuilder.DropTable(
                name: "cat_estado_vacuno");

            migrationBuilder.DropTable(
                name: "archivo");

            migrationBuilder.DropTable(
                name: "cat_tipo_utilizacion");

            migrationBuilder.DropTable(
                name: "celo_registro");

            migrationBuilder.DropTable(
                name: "responsable");

            migrationBuilder.DropTable(
                name: "cat_resultado_fecundacion");

            migrationBuilder.DropTable(
                name: "cat_tipo_fecundacion");

            migrationBuilder.DropTable(
                name: "cat_tipo_archivo");

            migrationBuilder.DropTable(
                name: "cat_modulo");

            migrationBuilder.DropTable(
                name: "cat_estado_registro");

            migrationBuilder.DropTable(
                name: "vacuno");

            migrationBuilder.DropTable(
                name: "cat_tipo_responsable");

            migrationBuilder.DropTable(
                name: "cat_tipo_adquisicion");

            migrationBuilder.DropTable(
                name: "cat_color");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "granja");

            migrationBuilder.DropTable(
                name: "cat_raza");

            migrationBuilder.DropTable(
                name: "cat_sexo");

            migrationBuilder.DropTable(
                name: "geo_distrito");

            migrationBuilder.DropTable(
                name: "geo_provincia");

            migrationBuilder.DropTable(
                name: "geo_departamento");
        }
    }
}
