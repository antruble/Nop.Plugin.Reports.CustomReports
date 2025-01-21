using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Reports.CustomReports.Models.PromotionSummary;
using Nop.Plugin.Reports.CustomReports.Models.OrderDetails;
using Nop.Plugin.Reports.CustomReports.Models.OrderSummary;
using Nop.Services.ExportImport;
using Nop.Services.ExportImport.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Reports.CustomReports.Models.CustomerId;

namespace Nop.Plugin.Reports.CustomReports.Services.Export
{
    public class ExportReportService
    {
        private readonly CatalogSettings _catalogSettings;

        public ExportReportService(
            CatalogSettings catalogSettings
            )
        {
            _catalogSettings = catalogSettings;
        }

        public async Task<byte[]> ExportOrderDetailsToXlsAsync(IList<OrderDetailsReportModel> list)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<OrderDetailsReportModel>("Rendelés", p => p.OrderId),
                new PropertyByName<OrderDetailsReportModel>("Rendelés státusza", p => p.Status),
                new PropertyByName<OrderDetailsReportModel>("Feldolgozható", p => p.FormattedProcessingAvailableDate),
                new PropertyByName<OrderDetailsReportModel>("Elkészült a csomag", p => p.FormattedPackageCompletedDate),
                new PropertyByName<OrderDetailsReportModel>("Csomag futárszolgálatnál", p => p.FormattedShippedDate),
                new PropertyByName<OrderDetailsReportModel>("Megkapta a vevő", p => p.FormattedDeliveredDate),
                new PropertyByName<OrderDetailsReportModel>("Szállítási mód", p => p.ShippingMethod),
                new PropertyByName<OrderDetailsReportModel>("Fizetési mód", p => p.PaymentMethod),
                new PropertyByName<OrderDetailsReportModel>("Csomagolási idő (dd:hh:mm)", p => p.FormattedPackagingTime)
            };

            return await new CustomPropertyManager<OrderDetailsReportModel>(properties, _catalogSettings).ExportToXlsxAsync(list);
        }
        public async Task<byte[]> ExportOrderSummaryToXlsAsync(IList<OrderSummaryReportModel> daily, IList<OrderSummaryReportModel> monthly)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<OrderSummaryReportModel>("Dátum", p => p.Period),
                new PropertyByName<OrderSummaryReportModel>("Rendelések száma", p => p.NumOfOrders),
                new PropertyByName<OrderSummaryReportModel>("Rendelt termékek száma", p => p.TotalQuantity),
                new PropertyByName<OrderSummaryReportModel>("Termék/rendelés", p => p.QuantityPerOrder), 
                new PropertyByName<OrderSummaryReportModel>("Végösszeg (nettó)", p => p.OrderTotal), 
                new PropertyByName<OrderSummaryReportModel>("Részösszeg (nettó)", p => p.OrderSubtotalInclTax),
                new PropertyByName<OrderSummaryReportModel>("Részösszeg/rendelés", p => p.SubTotalPerOrder), 
                new PropertyByName<OrderSummaryReportModel>("Részösszeg kedvezmény (nettó)", p => p.OrderSubtotalDiscountInclTax),
                new PropertyByName<OrderSummaryReportModel>("Rendeléshez tartozó kedvezmény (nettó)", p => p.OrderDiscount), 
                new PropertyByName<OrderSummaryReportModel>("Szállítási költség (nettó)", p => p.OrderShippingInclTax), 
                new PropertyByName<OrderSummaryReportModel>("Extra fizetéshez tartozó költség (nettó)", p => p.PaymentMethodAdditionalFeeInclTax), 
                new PropertyByName<OrderSummaryReportModel>("Bruttó részösszeg", p => p.OrderSubtotalExclTax), 
                new PropertyByName<OrderSummaryReportModel>("Rendeléshez tartozó adó", p => p.OrderTax), 
            };


            return await new CustomPropertyManager<OrderSummaryReportModel>(properties, _catalogSettings).ExportToXlsxAsync(daily, monthly, "Daily", "Monthly");
        }
        public async Task<byte[]> ExportPromotionSummaryToXlsAsync(IList<PromotionSummaryReportModel> list)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<PromotionSummaryReportModel>("Név", p => p.Name),
                new PropertyByName<PromotionSummaryReportModel>("Napi (db)", p => p.DailyUsageCount),
                new PropertyByName<PromotionSummaryReportModel>("Napi (nettó)", p => p.DailyTotalDiscountAmount),
                new PropertyByName<PromotionSummaryReportModel>("Napi (%)", p => p.DailyPercentage),
                new PropertyByName<PromotionSummaryReportModel>("Havi (db)", p => p.MonthlyUsageCount),
                new PropertyByName<PromotionSummaryReportModel>("Havi (nettó)", p => p.MonthlyTotalDiscountAmount),
                new PropertyByName<PromotionSummaryReportModel>("Havi (%)", p => p.MonthlyPercentage),
                new PropertyByName<PromotionSummaryReportModel>("Margin", p => p.MarginAmount),
                new PropertyByName<PromotionSummaryReportModel>("Margin (%)", p => p.MarginPercentage),
            };

            return await new CustomPropertyManager<PromotionSummaryReportModel>(properties, _catalogSettings).ExportToXlsxAsync(list);
        }
        public async Task<byte[]> ExportCustomerIdToXlsAsync(IList<CustomerIdReportModel> list)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<CustomerIdReportModel>("Rendelés szám", p => p.OrderNumber),
                new PropertyByName<CustomerIdReportModel>("Vevő ID", p => p.CustomerId),
                new PropertyByName<CustomerIdReportModel>("Simplepay fizetés ID", p => p.SimplePayTransactionId),
                new PropertyByName<CustomerIdReportModel>("Fizetés módja", p => p.PaymentMethod),
                new PropertyByName<CustomerIdReportModel>("Futárcég", p => p.Carrier),
                new PropertyByName<CustomerIdReportModel>("Nyomkövetési szám", p => p.TrackingNumber),
            };

            return await new CustomPropertyManager<CustomerIdReportModel>(properties, _catalogSettings).ExportToXlsxAsync(list);
        }
    }
}
