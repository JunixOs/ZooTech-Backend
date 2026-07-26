using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos
{
    public class ListCelosQuery : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Read;
        public string Action => "List celos";

        public string? Search { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public IReadOnlyDictionary<string, string>? ColumnFilters { get; set; }
    }
}