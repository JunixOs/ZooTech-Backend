namespace ZooTech.Domain.Module_Vacuno.Rules;

/// <summary>
/// Regla: las observaciones no deben superar 150 caracteres ni 30 palabras.
/// </summary>
public static class VacunoObservacionesRule
{
    public const int MaxCaracteres = 150;
    public const int MaxPalabras = 30;

    public static bool EsValido(string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(observaciones)) return true;

        if (observaciones.Length > MaxCaracteres) return false;

        var palabras = observaciones
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Length;

        return palabras <= MaxPalabras;
    }
}