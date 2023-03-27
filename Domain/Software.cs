namespace AccountManager.Domain
{
    public enum Software
    {
        [SoftwareComponent("Launcher", "launcher", "launcher")]
        Launcher,

        [SoftwareComponent("Site Master", "sitemaster", "sitemaster")]
        SiteMaster,

        [SoftwareComponent("Deployer", "deployer", "deployer")]
        Deployer,

        [SoftwareComponent("PDF/DXF Export", "pdfexport", "pdfexport")]
        PdfExport,

        [SoftwareComponent("Relational Export", "sqlexport", "sqlexport")]
        SqlExport,

        [SoftwareComponent("Populate", "populate", "populate")]
        Populate,

        [SoftwareComponent("Reporting", "reporting", "reporting")]
        Reporting,

        [SoftwareComponent("Web Client", "client", "client")]
        Client,

        [SoftwareComponent("Linkware", "linkware", "linkware")]
        Linkware,

        [SoftwareComponent("Site Master Validator", "smchk", "smchk")]
        Smchk,

        [SoftwareComponent("Discovery", "discovery", "discovery")]
        Discovery,

        [SoftwareComponent("Fiber SenSys", "fibersensys", "fibersensys")]
        FiberSenSys,

        [SoftwareComponent("Fiber Mountain", "fibermountain", "fibermountain")]
        FiberMountain,

        [SoftwareComponent("ServiceNow", "servicenow", "servicenow")]
        ServiceNow,

        [SoftwareComponent("CommScope", "fibermountain", "fibermountain")]
        CommScope
    }
}