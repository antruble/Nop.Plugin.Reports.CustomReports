using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.CustomerId
{
    public partial record CustomerIdReportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Reports.CustomerId.Fields.OrderNumber")]
        public string OrderNumber { get; set; } // Rendelési szám

        [NopResourceDisplayName("Admin.Reports.CustomerId.Fields.CustomerId")]
        public string CustomerId { get; set; } // Vevő ID (az, amit küld a rendszer az IF-en)

        [NopResourceDisplayName("Admin.Reports.CustomerId.Fields.SimplePayTransactionId")]
        public string SimplePayTransactionId { get; set; } // SimplePay fizetési azonosító

        [NopResourceDisplayName("Admin.Reports.CustomerId.Fields.PaymentMethod")]
        public string PaymentMethod { get; set; } // Kártyás / utánvétes fizetés

        [NopResourceDisplayName("Admin.Reports.CustomerId.Fields.Carrier")]
        public string Carrier { get; set; } // Futárcég

        [NopResourceDisplayName("Admin.Reports.CustomerId.Fields.TrackingNumber")]
        public string TrackingNumber { get; set; } // Nyomkövetési szám
    }
}
