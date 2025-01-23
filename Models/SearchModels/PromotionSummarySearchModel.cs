using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.SearchModels
{
    public partial record PromotionSummarySearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Reports.PromotionSummary.SearchModel.FilterCategoryId")]
        public int FilterCategoryId { get; set; }
        public IList<SelectListItem> AvailableCategoryOptions { get; set; }

        public PromotionSummarySearchModel() 
        {
            AvailableCategoryOptions = new List<SelectListItem>();
        }
    }

}
