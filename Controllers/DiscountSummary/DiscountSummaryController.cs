using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Reports.CustomReports.Models.DiscountSummary;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using ILogger = Nop.Services.Logging.ILogger;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Core;
using Nop.Services.Messages;
using Nop.Plugin.Reports.CustomReports.Services.Export;

namespace Nop.Plugin.Reports.CustomReports.Controllers.DiscountSummary
{
    public class DiscountSummaryController : BaseReportController<EmptySearchModel, DiscountSummaryListModel, DiscountSummaryReportModel>
    {
        private readonly ExportReportService _exportReportService;
        private readonly INotificationService _notificationService;
        public DiscountSummaryController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory reportsModelFactory,
            IPermissionService permissionService,
            ILogger logger,

            ExportReportService exportReportService,
            INotificationService notificationService
            )
            : base(baseReportFactory, reportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/DiscountSummary/DiscountSummary.cshtml")
        {
            _exportReportService = exportReportService;
            _notificationService = notificationService;
        }
        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        public async Task<IActionResult> ExportExcelAll(EmptySearchModel searchModel)
        {
            if (!await base.PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var data = await base.ReportsModelFactory.FetchDiscountSummaryDataAsync(searchModel);

            try
            {
                var bytes = await _exportReportService.ExportDiscountSummaryToXlsAsync(data);
                return File(bytes, MimeTypes.TextXlsx, "discountsummary.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }
    }
}
