namespace ZooTech.Domain.Exceptions
{
    public class VacunoYaExisteException : Exception
    {
        public VacunoYaExisteException(string codigo)
            : base($"Ya existe un vacuno con el código '{codigo}'.")
        {
        }
    }
}