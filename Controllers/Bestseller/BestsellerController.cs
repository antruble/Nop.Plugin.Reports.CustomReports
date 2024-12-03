using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Models.Bestsellers;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DailyOrders;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Services.Catalog;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Reports.CustomReports.Controllers.CustomerReports
{
    public class BestsellerController : BaseReportController<BestsellerSearchModel, BestsellersListModel, BestsellersReportModel>
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExportManager _exportManager;
        private readonly INotificationService _notificationService;
        private readonly IProductService _productService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderReportService _orderReportService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public BestsellerController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory customerReportsModelFactory,
            IPermissionService permissionService,
            ILogger logger,

            IDateTimeHelper dateTimeHelper,
            IExportManager exportManager,
            INotificationService notificationService,
            IProductService productService,
            IPriceFormatter priceFormatter,
            IOrderReportService orderReportService,
            IWorkContext workContext
            )
            : base(baseReportFactory, customerReportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/Bestsellers/Bestsellers.cshtml")
        {
            _dateTimeHelper = dateTimeHelper;
            _exportManager = exportManager;
            _notificationService = notificationService;
            _productService = productService;
            _priceFormatter = priceFormatter;
            _orderReportService = orderReportService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        public async Task<IActionResult> ExportExcelAll(BestsellerSearchModel searchModel)
        {
            if (!await base.PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
            var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.VendorId = currentVendor.Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //get bestsellers
            var bestsellers = await _orderReportService.BestSellersReportAsync(showHidden: true,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                billingCountryId: searchModel.BillingCountryId,
                orderBy: OrderByEnum.OrderByTotalAmount,
                vendorId: searchModel.VendorId,
                categoryId: searchModel.CategoryId,
                manufacturerId: searchModel.ManufacturerId,
                storeId: searchModel.StoreId);

            var bestsellerek = bestsellers.ToList();

            //prepare list model
            var lista = new List<Eladasok>();

            foreach (var item in bestsellerek)
            {
                var egyLista = new Eladasok();
                egyLista.ProductId = item.ProductId;
                egyLista.TotalQuantity = item.TotalQuantity;
                egyLista.TotalAmount = await _priceFormatter.FormatPriceAsync(item.TotalAmount, true, false);
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                egyLista.EAN = product.Sku;
                egyLista.AktualisKeszlet = product.StockQuantity;
                egyLista.ProductName = product.Name;
                lista.Add(egyLista);
            }

            try
            {
                var bytes = await _exportManager.ExportBestsellersToXlsxAsync(lista);
                return File(bytes, MimeTypes.TextXlsx, "eladasok.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }
        #endregion
    }
}
