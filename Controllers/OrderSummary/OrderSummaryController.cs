using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Models.OrderSummary;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Reports.CustomReports.Models.OrderDetails;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using ILogger = Nop.Services.Logging.ILogger;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Core;
using Nop.Services.Messages;
using Nop.Plugin.Reports.CustomReports.Services.Export;

namespace Nop.Plugin.Reports.CustomReports.Controllers.OrderSummary
{
    public class OrderSummaryController : BaseReportController<OrderSummarySearchModel, OrderSummaryListModel, OrderSummaryReportModel>
    {
        private readonly ExportReportService _exportReportService;
        private readonly INotificationService _notificationService;
        public OrderSummaryController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory reportsModelFactory,
            IPermissionService permissionService,
            ILogger logger,

            ExportReportService exportReportService,
            INotificationService notificationService
            )
            : base(baseReportFactory, reportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/OrderSummary/OrderSummary.cshtml")
        {
            _exportReportService = exportReportService;
            _notificationService = notificationService;
        }
        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        public async Task<IActionResult> ExportExcelAll(OrderSummarySearchModel searchModel)
        {
            if (!await base.PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var dataDaily = await base.ReportsModelFactory.FetchOrderSummaryDataAsync(new OrderSummarySearchModel { PeriodTypeId = 0});
            var dataMonthly = await base.ReportsModelFactory.FetchOrderSummaryDataAsync(new OrderSummarySearchModel { PeriodTypeId = 1});

            try
            {
                var bytes = await _exportReportService.ExportOrderSummaryToXlsAsync(dataDaily, dataMonthly);
                return File(bytes, MimeTypes.TextXlsx, "ordersummary.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }
    }
}
