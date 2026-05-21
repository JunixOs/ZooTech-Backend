using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant.Ports
{
    public interface ICreateTenantOutputPort
    {
        Task Ok(CreateTenantOutput output);
    }
}
