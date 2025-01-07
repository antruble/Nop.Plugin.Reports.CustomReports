using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Nop.Plugin.Reports.CustomReports.Models.OrderSummary
{
    public partial record OrderSummaryReportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.Period")]
        public string Period { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.NumOfOrders")]
        public int NumOfOrders { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.TotalQuantity")]
        public int TotalQuantity {  get; set; }
        
        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.QuantityPerOrder")]
        public decimal QuantityPerOrder {  get; set; }
        
        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.OrderSubtotalInclTax")]
        public decimal OrderSubtotalInclTax {  get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.OrderSubtotalExclTax")]
        public decimal OrderSubtotalExclTax { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.SubTotalPerOrder")]
        public decimal SubTotalPerOrder {  get; set; }
        
        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.OrderSubtotalDiscountInclTax")]
        public decimal OrderSubtotalDiscountInclTax { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.OrderShippingInclTax")]
        public decimal OrderShippingInclTax { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.PaymentMethodAdditionalFeeInclTax")]
        public decimal PaymentMethodAdditionalFeeInclTax { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.OrderTax")]
        public decimal OrderTax { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.OrderTotal")]
        public decimal OrderTotal { get; set; }
        [NopResourceDisplayName("Admin.Reports.OrderSummary.Fields.OrderDiscount")]
        public decimal OrderDiscount { get; set; }
    }
}
