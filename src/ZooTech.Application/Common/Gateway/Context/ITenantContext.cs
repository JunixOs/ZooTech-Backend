using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Common.Gateway.Context
{
    public interface ITenantContext
    {
        long TenantId { get; }
        string Code { get; }
        string SubDomain { get; }
        string DatabaseName { get; }

        void SetTenant(long id, string code , string subDomain , string databaseName);
    }
}
