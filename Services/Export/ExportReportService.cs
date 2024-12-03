using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Reports.CustomReports.Models.OrderDetails;
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
    }
}
