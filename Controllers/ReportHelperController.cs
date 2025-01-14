using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Logging;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Plugin.Reports.CustomReports.Models.OrderDetails;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Reports.CustomReports.Controllers
{
    public class ReportHelperController : BaseReportController<EmptySearchModel, OrderDetailsListModel, OrderDetailsReportModel>
    {
        public ReportHelperController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory reportsModelFactory,
            IPermissionService permissionService,
            ILogger logger)
            : base(baseReportFactory, reportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/CustomReport.cshtml")
        {

        }

        /// <summary>
        /// Visszaadja a plugin konfigurációs nézetét.
        /// </summary>
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            return View("~/Plugins/Reports.CustomReports/Views/Configure.cshtml");
        }

        [HttpPost]
        public IActionResult RunMigration()
        {
            try
            {
                using (var scope = EngineContext.Current.Resolve<IServiceScopeFactory>().CreateScope())
                {
                    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    runner.MigrateUp(202501070930);
                }

                TempData["SuccessMessage"] = "A migráció sikeresen lefutott.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hiba történt a migráció során: {ex.Message}";
            }

            return RedirectToAction("Configure");
        }
    }

}
