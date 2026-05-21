namespace ZooTech.Application.Common.Exceptions;

/// <summary>
/// Se lanza cuando se intenta registrar un vacuno con un código que ya existe.
/// El middleware la intercepta y devuelve 409 Conflict.
/// </summary>
public sealed class VacunoYaExisteException : Exception
{
    public VacunoYaExisteException(string message) : base(message) { }
}