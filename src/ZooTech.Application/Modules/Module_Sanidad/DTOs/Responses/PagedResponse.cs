namespace ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;

public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; }
    public int TotalRegistros { get; set; }
    public int Pagina { get; set; }
    public int Tamano { get; set; }
    public int TotalPaginas { get; set; }
}