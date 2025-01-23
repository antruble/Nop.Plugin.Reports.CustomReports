using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Reports.CustomReports.Models.PromotionSummary;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using ILogger = Nop.Services.Logging.ILogger;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Core;
using Nop.Services.Messages;
using Nop.Plugin.Reports.CustomReports.Services.Export;

namespace Nop.Plugin.Reports.CustomReports.Controllers.PromotionSummary
{
    public class PromotionSummaryController : BaseReportController<PromotionSummarySearchModel, PromotionSummaryListModel, PromotionSummaryReportModel>
    {
        private readonly ExportReportService _exportReportService;
        private readonly INotificationService _notificationService;
        public PromotionSummaryController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory reportsModelFactory,
            IPermissionService permissionService,
            ILogger logger,

            ExportReportService exportReportService,
            INotificationService notificationService
            )
            : base(baseReportFactory, reportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/PromotionSummary/PromotionSummary.cshtml")
        {
            _exportReportService = exportReportService;
            _notificationService = notificationService;
        }
        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        public async Task<IActionResult> ExportExcelAll(PromotionSummarySearchModel searchModel)
        {
            if (!await base.PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var data = await base.ReportsModelFactory.FetchPromotionSummaryDataAsync(searchModel);

            try
            {
                var bytes = await _exportReportService.ExportPromotionSummaryToXlsAsync(data);
                return File(bytes, MimeTypes.TextXlsx, "PromotionSummary.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }
    }
}
