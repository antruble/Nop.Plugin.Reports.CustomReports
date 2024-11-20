using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DailyOrders;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasManufacturer;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Services.Security;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Reports.CustomReports.Controllers.Problemasak.ProblemasManufacturer
{
    public class ProblemasManufacturerController : BaseReportController<ProblemasManufacturerSearchModel, ProblemasManufacturerListModel, ProblemasManufacturerReportModel>
    {
        public ProblemasManufacturerController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory customerReportsModelFactory,
            IPermissionService permissionService,
            ILogger logger
            )
            : base(baseReportFactory, customerReportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/Problemasak/ProblemasManufacturer/ProblemasManufacturer.cshtml")
        {

        }
    }
}
