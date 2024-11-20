using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasOrder;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Services.Security;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Reports.CustomReports.Controllers.Problemasak.ProblemasOrder
{
    public class ProblemasOrderController : BaseReportController<ProblemasOrderSearchModel, ProblemasOrderListModel, ProblemasOrderReportModel>
    {
        public ProblemasOrderController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory customerReportsModelFactory,
            IPermissionService permissionService,
            ILogger logger
            )
            : base(baseReportFactory, customerReportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/Problemasak/ProblemasOrder/ProblemasOrder.cshtml")
        {

        }
    }
}
