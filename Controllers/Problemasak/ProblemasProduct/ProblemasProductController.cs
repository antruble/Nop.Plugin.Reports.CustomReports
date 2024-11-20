using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasProduct;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Services.Security;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Reports.CustomReports.Controllers.Problemasak.ProblemasProduct
{
    public class ProblemasProductController : BaseReportController<ProblemasProductSearchModel, ProblemasProductListModel, ProblemasProductReportModel>
    {
        public ProblemasProductController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory customerReportsModelFactory,
            IPermissionService permissionService,
            ILogger logger
            )
            : base(baseReportFactory, customerReportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/Problemasak/ProblemasProduct/ProblemasProduct.cshtml")
        {

        }
    }
}
