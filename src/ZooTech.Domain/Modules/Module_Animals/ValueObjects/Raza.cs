using ZooTech.Domain.Exceptions;

namespace ZooTech.Domain.Modules.Module_Animals.ValueObjects;

public class Raza
{
    public string Code { get; }
    public string Nombre { get; }

    public Raza(string code, string nombre)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new DomainException("El código de la raza es obligatorio.");
        if (string.IsNullOrWhiteSpace(nombre)) throw new DomainException("El nombre de la raza es obligatorio.");

        Code = code;
        Nombre = nombre;
    }
}
