using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.ShoperiaPlusSubscriptions;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Services.Security;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Reports.CustomReports.Controllers.CustomerReports
{
    public class ShoperiaPlusSubscriptionsController : BaseReportController<CustomerReportsSearchModel, ShoperiaPlusSubscriptionsListModel, ShoperiaPlusSubscriptionsReportModel>
    {
        public ShoperiaPlusSubscriptionsController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory customerReportsModelFactory,
            IPermissionService permissionService,
            ILogger logger
            )
            : base(baseReportFactory, customerReportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/CustomerReports/ShoperiaPlusSubscriptions/ShoperiaPlusSubscriptions.cshtml")
        {

        }
    }
}
