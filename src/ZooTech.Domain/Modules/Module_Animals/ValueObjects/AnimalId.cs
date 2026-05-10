namespace ZooTech.Domain.Modules.Module_Animals.ValueObjects;

public class AnimalId
{
    public long Value { get; }

    public AnimalId(long value)
    {
        if (value <= 0)
            throw new ArgumentException("Id inválido: debe ser mayor a cero.");
        Value = value;
    }

    public static AnimalId Of(long value) => new(value);
}
