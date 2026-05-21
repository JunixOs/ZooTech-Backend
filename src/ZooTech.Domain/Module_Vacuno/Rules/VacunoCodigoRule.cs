namespace ZooTech.Domain.Module_Vacuno.Rules;

/// <summary>
/// Regla: el código del vacuno no puede exceder 10 caracteres y debe estar en mayúsculas.
/// </summary>
public static class VacunoCodigoRule
{
    public const int LongitudMaxima = 10;

    public static bool EsValido(string codigo)
        => !string.IsNullOrWhiteSpace(codigo)
           && codigo.Length <= LongitudMaxima
           && codigo == codigo.ToUpperInvariant();
}