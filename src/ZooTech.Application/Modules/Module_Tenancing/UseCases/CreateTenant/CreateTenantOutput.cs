using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public class CreateTenantOutput
    {
        public string Code { get; set; } = default!;
        public string SubDomain { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string LegalName { get; set; } = default!;
    }
}
