public static CeloItemResponse ToResponse(CeloItemDto item)
{
    return new CeloItemResponse
    {
        Id = item.Id,

        CodigoRegistro = item.CodigoRegistro,
        Fecha = item.Fecha,
        Hora = item.Hora,
        CodigoVacuno = item.CodigoVacuno,
        NombreVacuno = item.NombreVacuno,
        VecesEnCelo = item.VecesEnCelo
    };
}