using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Reports.CustomReports.Models.PromotionSummary
{
    public partial record PromotionSummaryReportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.DailyUsageCount")]
        public int DailyUsageCount  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.DailyTotalDiscountAmount")]
        public decimal DailyTotalDiscountAmount  {  get; set; }
        
        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.DailyPercentage")]
        public decimal DailyPercentage  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.MonthlyUsageCount")]
        public int MonthlyUsageCount  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.MonthlyTotalDiscountAmount")]
        public decimal MonthlyTotalDiscountAmount  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.MonthlyPercentage")]
        public decimal MonthlyPercentage  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.MarginAmount")]
        public decimal MarginAmount { get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.MarginPercentage")]
        public decimal MarginPercentage { get; set; }

    }
}
