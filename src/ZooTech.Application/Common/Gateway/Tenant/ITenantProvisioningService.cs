using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateTenant;

namespace ZooTech.Application.Common.Gateway.Tenant
{
    public interface ITenantProvisioningService
    {
        public Task ProvisionAsync(CreateTenantCommand cmd);
    }
}
