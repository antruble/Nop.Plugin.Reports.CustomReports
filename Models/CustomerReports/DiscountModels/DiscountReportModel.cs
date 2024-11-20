using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DiscountModels
{
    public partial record DiscountReportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Reports.Other.Discounts.Fields.DiscountType")]
        public string DiscountTypeName { get; set; }

        [NopResourceDisplayName("Admin.Reports.Other.Discounts.Fields.UsageCount")]
        public int UsageCount { get; set; }

        [NopResourceDisplayName("Admin.Reports.Other.Discounts.Fields.TotalDiscountAmount")]
        public decimal TotalDiscountAmount { get; set; }
    }
}
