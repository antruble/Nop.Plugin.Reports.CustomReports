using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.OrderDetails
{
    public partial record OrderDetailsReportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.OrderId")]
        public int OrderId { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.Status")]
        public string Status {  get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.ProcessingAvailableDate")]
        public DateTime? ProcessingAvailableDate { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.FormattedProcessingAvailableDate")]
        public string FormattedProcessingAvailableDate => ProcessingAvailableDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "MISSING";

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.PackageCompletedDate")]
        public DateTime? PackageCompletedDate { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.FormattedPackageCompletedDate")]
        public string FormattedPackageCompletedDate => PackageCompletedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "MISSING";

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.ShippedDate")]
        public DateTime? ShippedDate { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.FormattedShippedDate")]
        public string FormattedShippedDate => ShippedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "MISSING";
        
        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.DeliveredDate")]
        public DateTime? DeliveredDate { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.FormattedDeliveredDate")]
        public string FormattedDeliveredDate => DeliveredDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "MISSING";

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.ShippingMethod")]
        public string ShippingMethod { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.PaymentMethod")]
        public string PaymentMethod { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.PackagingTime")]
        public TimeSpan? PackagingTime { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderDetails.Fields.FormattedPackagingTime")]
        public string FormattedPackagingTime => PackagingTime?.ToString(@"dd\:hh\:mm") ?? "Nem készült el";
    }
}
