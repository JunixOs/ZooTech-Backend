using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos
{
    public class ListCelosCommand : IAuditableRequest
    {
        public AuditEventType EventType => throw new NotImplementedException();
        public string Action => throw new NotImplementedException();

        public string? Search { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public IReadOnlyDictionary<string, string>? ColumnFilters { get; set; }
    }
}