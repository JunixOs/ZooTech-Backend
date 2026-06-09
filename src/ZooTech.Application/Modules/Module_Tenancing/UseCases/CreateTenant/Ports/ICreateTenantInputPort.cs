using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant.Ports
{
    public interface ICreateTenantInputPort
    {
        Task Handle(CreateTenantCommand cmd);
    }
}
