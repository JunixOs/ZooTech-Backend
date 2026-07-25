
namespace ZooTech.Domain.Module_Celo.Entities
{
    public sealed class CeloListItem
    {
    private CeloListItem(

        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string vacunoCodigo,
        string nombreVacuno,
        string? observaciones,
        List<string> caracteristicaCodes
            ) {

            Id = id;
            Codigo = codigo;
            FechaHora = fechaHora;
            VacunoId = vacunoId;
            VacunoCodigo = vacunoCodigo;
            NombreVacuno = nombreVacuno;
            Observaciones = observaciones;
            CaracteristicaCodes = caracteristicaCodes;
        }


        public long Id { get; }
        public string Codigo { get; private set; }
        public DateTime FechaHora { get; private set; }
        public long VacunoId { get; private set; }
        public string VacunoCodigo { get; private set; }
        public string NombreVacuno { get; private set; }
        public string? Observaciones { get; private set; }
        public IReadOnlyList<string> CaracteristicaCodes { get; private set; }



        public static CeloListItem Rehydrate(
        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string vacunoCodigo,
        string nombreVacuno,
        string? observaciones,
        List<string> caracteristicaCodes

        )
        {
            var celoListItem = new CeloListItem(
            id,
            codigo.Trim(),
            fechaHora,
            vacunoId,
            vacunoCodigo,
            nombreVacuno,
            observaciones,
            caracteristicaCodes
            );


            return celoListItem;

        }

    }

}
