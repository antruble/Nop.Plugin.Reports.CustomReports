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
    /// <summary>
    /// Represents a daily order search model
    /// </summary>
    public partial record OrderSummarySearchModel : BaseSearchModel
    {
        #region Ctor

        public OrderSummarySearchModel()
        {
           PeriodTypeOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Reports.OrderSummary.SearchModel.PeriodTypeId")]
        public int PeriodTypeId { get; set; }

        public IList<SelectListItem> PeriodTypeOptions { get; set; }

        #endregion
    }
}
