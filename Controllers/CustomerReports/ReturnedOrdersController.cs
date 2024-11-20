using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.ReturnedOrders;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Services.Security;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Reports.CustomReports.Controllers.CustomerReports
{
    public class ReturnedOrdersController : BaseReportController<CustomerReportsSearchModel, ReturnedOrdersListModel, ReturnedOrdersReportModel>
    {
        public ReturnedOrdersController(
                BaseReportFactory baseReportFactory,
            IReportsModelFactory customerReportsModelFactory,
            IPermissionService permissionService,
            ILogger logger
            )
            : base(baseReportFactory, customerReportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/CustomerReports/ReturnedOrders/ReturnedOrders.cshtml")
        {

        }
    }
}
