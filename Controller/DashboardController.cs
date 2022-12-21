using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace PortalPreview.Controller
{
    public class DefaultDashboardController : DashboardController
    {
        public DefaultDashboardController(DashboardConfigurator configurator, IDataProtectionProvider? dataProtectionProvider = null)
           : base(configurator, dataProtectionProvider)
        {
        }
    }
}
