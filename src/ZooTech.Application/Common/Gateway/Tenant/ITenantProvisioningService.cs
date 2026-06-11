using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;

namespace ZooTech.Application.Common.Gateway.Tenant
{
    public interface ITenantProvisioningService
    {
        public Task<bool> ProvisionAsync(CreateTenantCommand cmd);
    }
}
