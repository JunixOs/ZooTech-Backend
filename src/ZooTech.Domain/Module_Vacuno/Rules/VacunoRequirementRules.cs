namespace ZooTech.Domain.Module_Vacuno.Rules;

public static class VacunoRequirementRules
{
    public static class SettingCodes
    {
        public const string DefaultFilterDays = "VACUNOS_DEFAULT_FILTER_DAYS";
        public const string ArbolMinNiveles = "VACUNOS_ARBOL_MIN_NIVELES";
        public const string ArbolMaxNiveles = "VACUNOS_ARBOL_MAX_NIVELES";
        public const string CodigoMaxLength = "VACUNOS_CODIGO_MAX_LENGTH";
        public const string InputMaxLength = "VACUNOS_INPUT_MAX_LENGTH";
        public const string ObservacionesMaxLength = "VACUNOS_OBSERVACIONES_MAX_LENGTH";
        public const string ObservacionesMaxWords = "VACUNOS_OBSERVACIONES_MAX_WORDS";
        public const string FotoFormatosPermitidos = "VACUNOS_FOTO_FORMATOS_PERMITIDOS";
        public const string CodigoUppercaseRequired = "VACUNOS_CODIGO_UPPERCASE_REQUIRED";
        public const string ReporteFormatosDescarga = "VACUNOS_REPORTE_FORMATOS_DESCARGA";
        public const string EliminacionUndoSeconds = "VACUNOS_ELIMINACION_UNDO_SECONDS";
        public const string FecundacionObservacionesMaxLength = "VACUNOS_FECUNDACION_OBSERVACIONES_MAX_LENGTH";
    }

    public static class FeatureCodes
    {
        public const string ModuleVacunos = "MODULE_VACUNOS";
        public const string ModuleVacunosReproduccion = "MODULE_VACUNOS_REPRODUCCION";
        public const string ModuleVacunosReportes = "MODULE_VACUNOS_REPORTES";
    }

    public static class RuleCodes
    {
        public const string EliminacionCondicionada = "VACUNOS_ELIMINACION_CONDICIONADA";
        public const string FecundacionTrazabilidad = "VACUNOS_FECUNDACION_TRAZABILIDAD";
        public const string FecundacionPendienteUnica = "VACUNOS_FECUNDACION_PENDIENTE_UNICA";
    }

    public static class ResourceKeys
    {
        public const string ErrorCampoObligatorio = "Error_CampoObligatorio";
        public const string ErrorCodigoMaxLength = "Error_CodigoMaxLength";
        public const string ErrorInputMaxLength = "Error_InputMaxLength";
        public const string ErrorObservacionesMaxLength = "Error_ObservacionesMaxLength";
        public const string ErrorFotoFormatoInvalido = "Error_FotoFormatoInvalido";
        public const string SuccessRegistroVacunoCreado = "Success_RegistroVacunoCreado";
        public const string SuccessRegistroVacunoActualizado = "Success_RegistroVacunoActualizado";
    }

    public static class Text
    {
        public static readonly string[] WordSeparators = { " " };

        public const int CodigoMaxLength = 15;
        public const int InputMaxLength = 15;
        public const int DefaultFilterDays = 30;
        public const int ArbolMinNiveles = 1;
        public const int ArbolMaxNiveles = 4;
        public const int UbigeoNameMaxLength = 60;
        public const int UtilizacionMaxLength = 30;
        public const int FotoRutaMaxLength = 500;
        public const int ObservacionesMaxLength = 150;
        public const int ObservacionesMaxWords = 30;
    }

    public static class Catalogs
    {
        public const int EstadoVivoId = 1;
        public const int TipoAdquisicionMontaId = 1;
        public const int TipoAdquisicionCompraId = 2;
        public const int SexoMachoId = 1;
        public const int SexoHembraId = 2;
        public const int TipoUtilizacionProduccionLecheId = 1;
        public const int TipoUtilizacionCarneId = 2;
        public const int TipoUtilizacionReproduccionId = 3;
        public const int ColorDefaultId = 1;
        public const int GranjaDefaultId = 1;
        public const int UbigeoHashSeed = 17;
        public const int UbigeoHashMultiplier = 31;
        public const int UbigeoHashModulo = 10000;
        public const int UbigeoHashOffset = 1;

        public const string EstadoVivo = "vivo";
        public const string EstadoMuerto = "muerto";
        public const string TipoAdquisicionMonta = "monta";
        public const string TipoAdquisicionCompra = "compra";
        public const string SexoMacho = "macho";
        public const string SexoHembra = "hembra";
        public const string TipoUtilizacionProduccionLeche = "produccion_leche";
        public const string TipoUtilizacionCarne = "carne";
        public const string TipoUtilizacionReproduccion = "reproduccion";

        public static readonly IReadOnlyDictionary<string, int> TiposAdquisicion =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                [TipoAdquisicionMonta] = TipoAdquisicionMontaId,
                [TipoAdquisicionCompra] = TipoAdquisicionCompraId
            };

        public static readonly IReadOnlyDictionary<string, int> Razas =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["Jersey"] = 1,
                ["Holstein"] = 2,
                ["Angus"] = 3,
                ["Hereford"] = 4,
                ["Simmental"] = 5,
                ["Brown Swiss"] = 6,
                ["Brahman"] = 7,
                ["Charolais"] = 8,
                ["Aberdeen Angus"] = 9,
                ["Ayrshire"] = 10,
                ["Beefmaster"] = 11,
                ["Bonsmara"] = 12,
                ["Braford"] = 13,
                ["Brangus"] = 14,
                ["Chianina"] = 15,
                ["Criollo"] = 16,
                ["Dexter"] = 17,
                ["Fleckvieh"] = 18,
                ["Galloway"] = 19,
                ["Gelbvieh"] = 20,
                ["Girolando"] = 21,
                ["Gyr"] = 22,
                ["Limousin"] = 23,
                ["Marchigiana"] = 24,
                ["Normando"] = 25,
                ["Pardo Suizo"] = 26,
                ["Red Angus"] = 27,
                ["Romagnola"] = 28,
                ["Santa Gertrudis"] = 29,
                ["Shorthorn"] = 30,
                ["Wagyu"] = 31
            };

        public static readonly IReadOnlyDictionary<string, int> Sexos =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                [SexoMacho] = SexoMachoId,
                [SexoHembra] = SexoHembraId
            };

        public static readonly IReadOnlyDictionary<string, int> TiposUtilizacion =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                [TipoUtilizacionProduccionLeche] = TipoUtilizacionProduccionLecheId,
                [TipoUtilizacionCarne] = TipoUtilizacionCarneId,
                [TipoUtilizacionReproduccion] = TipoUtilizacionReproduccionId
            };

        public static readonly IReadOnlyDictionary<string, int> Colores =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["negro"] = 1,
                ["blanco"] = 2,
                ["marron"] = 3,
                ["marr\u00f3n"] = 3,
                ["gris"] = 4,
                ["rojizo"] = 5,
                ["rojo"] = 5
            };
    }

    public static class Image
    {
        public const string StorageFolder = "vacunos/fotos";

        public static readonly IReadOnlySet<string> AllowedExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".png",
                ".jpg",
                ".jpeg"
            };
    }

    public static class Api
    {
        public const string Route = "v1/vacunos";
        public const string ByCodeRoute = "codigo/{codigo}";
        public const string FilesPath = "files";
        public const int DefaultPage = 1;
        public const int DefaultLimit = 10;
        public const int MinLimit = 1;
        public const int MaxLimit = 50;
        public const int EmptyTotalPages = 1;
    }

    public static class Messages
    {
        public const string DuplicateMainData = "Ya existe otro vacuno con los mismos datos principales.";
        public const string InvalidPurchasePrice =
            "El precio de compra es obligatorio y debe ser mayor a 0 cuando la adquisicion es compra.";
        public const string ObservationLimit = "Las observaciones no pueden exceder 30 palabras o 150 caracteres.";
        public const string InvalidPhoto = "La foto debe estar en formato JPG, JPEG o PNG.";
        public const string InvalidPhotoShort = "La foto debe ser PNG, JPG o JPEG.";

        public static string Required(string field) => $"El campo {field} es obligatorio.";

        public static string InputMaxLength(string field) =>
            $"El campo {field} debe tener como maximo {Text.InputMaxLength} caracteres.";
    }
}
