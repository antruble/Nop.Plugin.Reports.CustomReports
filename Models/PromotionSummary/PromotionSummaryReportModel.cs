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
        public int DailyTotalDiscountAmount  {  get; set; }
        
        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.DailyPercentage")]
        public int DailyPercentage  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.MonthlyUsageCount")]
        public int MonthlyUsageCount  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.MonthlyTotalDiscountAmount")]
        public int MonthlyTotalDiscountAmount  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.MonthlyPercentage")]
        public int MonthlyPercentage  {  get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.MarginAmount")]
        public double MarginAmount { get; set; }

        [NopResourceDisplayName("Admin.Reports.PromotionSummary.Fields.MarginPercentage")]
        public double MarginPercentage { get; set; }

    }
}
