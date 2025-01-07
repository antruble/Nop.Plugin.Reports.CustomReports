using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Reports.CustomReports.Models.DiscountSummary
{
    public partial record DiscountSummaryReportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Reports.DiscountSummary.Fields.OrderId")]
        public string DiscountName { get; set; }

        [NopResourceDisplayName("Admin.Reports.DiscountSummary.Fields.DailyUsageCount")]
        public int DailyUsageCount  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.DiscountSummary.Fields.DailyTotalDiscountAmount")]
        public int DailyTotalDiscountAmount  {  get; set; }
        
        [NopResourceDisplayName("Admin.Reports.DiscountSummary.Fields.DailyPercentage")]
        public int DailyPercentage  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.DiscountSummary.Fields.MonthlyUsageCount")]
        public int MonthlyUsageCount  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.DiscountSummary.Fields.MonthlyTotalDiscountAmount")]
        public int MonthlyTotalDiscountAmount  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.DiscountSummary.Fields.MonthlyPercentage")]
        public int MonthlyPercentage  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.DiscountSummary.Fields.MarginAmount")]
        public double MarginAmount  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.DiscountSummary.Fields.MarginPercentage")]
        public double MarginPercentage  {  get; set; }
        
    }
}
