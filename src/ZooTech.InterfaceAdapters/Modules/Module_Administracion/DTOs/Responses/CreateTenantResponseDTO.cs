namespace ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Responses
{
    public class CreateTenantResponseDto
    {
        public string Code { get; set; }
            = default!;

        public string SubDomain { get; set; }
            = default!;

        public string DisplayName { get; set; }
            = default!;

        public string LegalName { get; set; }
            = default!;
    }
}