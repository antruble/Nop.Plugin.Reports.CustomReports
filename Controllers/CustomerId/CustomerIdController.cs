using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Models.OrderSummary;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Plugin.Reports.CustomReports.Services.Export;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Reports.CustomReports.Models.CustomerId;
using Nop.Services.Logging;
using Nop.Core;

namespace Nop.Plugin.Reports.CustomReports.Controllers.CustomerId
{
    public class CustomerIdController : BaseReportController<EmptySearchModel, CustomerIdListModel, CustomerIdReportModel>
    {
        private readonly ExportReportService _exportReportService;
        private readonly INotificationService _notificationService;
        public CustomerIdController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory reportsModelFactory,
            IPermissionService permissionService,
            ILogger logger,

            ExportReportService exportReportService,
            INotificationService notificationService
            )
            : base(baseReportFactory, reportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/CustomerId/CustomerId.cshtml")
        {
            _exportReportService = exportReportService;
            _notificationService = notificationService;
        }
        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        public async Task<IActionResult> ExportExcelAll(EmptySearchModel searchModel)
        {
            //if (!await base.PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            //    return AccessDeniedView();

            //var dataDaily = await base.ReportsModelFactory.FetchOrderSummaryDataAsync(new OrderSummarySearchModel { PeriodTypeId = 0 });
            //var dataMonthly = await base.ReportsModelFactory.FetchOrderSummaryDataAsync(new OrderSummarySearchModel { PeriodTypeId = 1 });

            //try
            //{
            //    var bytes = await _exportReportService.ExportCustomerIdToXlsAsync(dataDaily, dataMonthly);
            //    return File(bytes, MimeTypes.TextXlsx, "customerid.xlsx");
            //}
            //catch (Exception exc)
            //{
            //    await _notificationService.ErrorNotificationAsync(exc);
            //    return RedirectToAction("List");
            //}
            return null; //TODO
        }
    }
}
