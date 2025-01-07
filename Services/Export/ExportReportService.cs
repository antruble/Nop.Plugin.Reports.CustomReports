using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Reports.CustomReports.Models.DiscountSummary;
using Nop.Plugin.Reports.CustomReports.Models.OrderDetails;
using Nop.Plugin.Reports.CustomReports.Models.OrderSummary;
using Nop.Services.ExportImport;
using Nop.Services.ExportImport.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                new PropertyByName<OrderSummaryReportModel>("Date", p => p.Period),
                new PropertyByName<OrderSummaryReportModel>("Orders", p => p.NumOfOrders),
                new PropertyByName<OrderSummaryReportModel>("Total quantity", p => p.TotalQuantity),
                new PropertyByName<OrderSummaryReportModel>("Quantity/order", p => p.QuantityPerOrder), 
                new PropertyByName<OrderSummaryReportModel>("OrderSubTotal (incl tax)", p => p.OrderSubtotalInclTax),
                new PropertyByName<OrderSummaryReportModel>("Subtotal/order", p => p.SubTotalPerOrder), 
                new PropertyByName<OrderSummaryReportModel>("OrderSubTotal (exl tax)", p => p.OrderSubtotalExclTax), 
                new PropertyByName<OrderSummaryReportModel>("OrderSubtotalDiscount (incl tax)", p => p.OrderSubtotalDiscountInclTax),
                new PropertyByName<OrderSummaryReportModel>("OrderShipping (incl tax)", p => p.OrderShippingInclTax), 
                new PropertyByName<OrderSummaryReportModel>("PaymentMethodAdditionalFee (incl tax)", p => p.PaymentMethodAdditionalFeeInclTax), 
                new PropertyByName<OrderSummaryReportModel>("OrderTax", p => p.OrderTax), 
                new PropertyByName<OrderSummaryReportModel>("OrderTotal", p => p.OrderTotal), 
                new PropertyByName<OrderSummaryReportModel>("OrderDiscount", p => p.OrderDiscount), 
            };


            return await new CustomPropertyManager<OrderSummaryReportModel>(properties, _catalogSettings).ExportToXlsxAsync(daily, monthly, "Daily", "Monthly");
        }
        public async Task<byte[]> ExportDiscountSummaryToXlsAsync(IList<DiscountSummaryReportModel> list)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<DiscountSummaryReportModel>("NÉV", p => p.DiscountName),
                new PropertyByName<DiscountSummaryReportModel>("NAPI (db)", p => p.DailyUsageCount),
                new PropertyByName<DiscountSummaryReportModel>("NAPI (nettó)", p => p.DailyTotalDiscountAmount),
                new PropertyByName<DiscountSummaryReportModel>("NAPI (%)", p => p.DailyPercentage),
                new PropertyByName<DiscountSummaryReportModel>("HAVI (db)", p => p.MonthlyUsageCount),
                new PropertyByName<DiscountSummaryReportModel>("HAVI (nettó)", p => p.MonthlyTotalDiscountAmount),
                new PropertyByName<DiscountSummaryReportModel>("HAVI (%)", p => p.MonthlyPercentage),
                new PropertyByName<DiscountSummaryReportModel>("MARGIN", p => p.MarginAmount),
                new PropertyByName<DiscountSummaryReportModel>("MARGIN (%)", p => p.MarginPercentage),
            };

            return await new CustomPropertyManager<DiscountSummaryReportModel>(properties, _catalogSettings).ExportToXlsxAsync(list);
        }
    }
}
