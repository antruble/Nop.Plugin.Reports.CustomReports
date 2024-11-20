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
    public partial record CustomerReportsSearchModel : BaseSearchModel
    {
        #region Ctor

        public CustomerReportsSearchModel()
        {
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Reports.Custom.CustomReports.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Reports.Custom.CustomReports.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        #endregion
    }
}
