
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
        string nombreVacuno
            ) {

            Id = id;
            Codigo = codigo;
            FechaHora = fechaHora;
            VacunoId = vacunoId;
            VacunoCodigo = vacunoCodigo;
            NombreVacuno = nombreVacuno;
        }


        public long Id { get; }
        public string Codigo { get; private set; }
        public DateTime FechaHora { get; private set; }
        public long VacunoId { get; private set; }
        public string VacunoCodigo { get; private set; }
        public string NombreVacuno { get; private set; }



        public static CeloListItem Rehydrate(
        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string vacunoCodigo,
        string nombreVacuno

        )
        {
            var celoListItem = new CeloListItem(
            id,
            codigo.Trim(),
            fechaHora,
            vacunoId,
            vacunoCodigo,
            nombreVacuno
            );


            return celoListItem;

        }

    }

}
