namespace ZooTech.Application.Common.Gateway.Context;

public interface ITenantContext
{
    Guid TenantId { get; }
    string DatabaseName { get; }
}
