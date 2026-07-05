namespace ZooTech.Application.Common.Gateway.Time;

/// <summary>
/// Abstracción del reloj del sistema para facilitar pruebas unitarias.
/// </summary>
public interface ITimeProvider
{
    DateTime UtcNow { get; }
}