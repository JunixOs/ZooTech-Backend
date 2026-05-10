using ZooTech.Domain.Exceptions;

namespace ZooTech.Domain.Modules.Module_Animals.ValueObjects;

public class Procedencia
{
    public string Granja { get; }
    public string Distrito { get; }
    public string Provincia { get; }
    public string Departamento { get; }

    public Procedencia(string granja, string distrito, string provincia, string departamento)
    {
        if (string.IsNullOrWhiteSpace(granja)) throw new DomainException("La granja es obligatoria.");
        if (string.IsNullOrWhiteSpace(distrito)) throw new DomainException("El distrito es obligatorio.");
        if (string.IsNullOrWhiteSpace(provincia)) throw new DomainException("La provincia es obligatoria.");
        if (string.IsNullOrWhiteSpace(departamento)) throw new DomainException("El departamento es obligatorio.");

        Granja = granja;
        Distrito = distrito;
        Provincia = provincia;
        Departamento = departamento;
    }

    public override string ToString()
    {
        return $"{Granja}, {Distrito}, {Provincia}, {Departamento}";
    }
}
