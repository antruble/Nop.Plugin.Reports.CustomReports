using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;


namespace Nop.Plugin.Reports.CustomReports.Models.SearchModels
{
    public partial record ProblemasManufacturerSearchModel : BaseSearchModel
    {
        #region Ctor

        public ProblemasManufacturerSearchModel()
        {
            AvailableProblemaOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Reports.LowStock.SearchManufacturerProblema")]
        public int SearchProblemaId { get; set; }
        public IList<SelectListItem> AvailableProblemaOptions { get; set; }

        #endregion
    }
    public partial record ProblemasOrderSearchModel : BaseSearchModel
    {
        #region Ctor

        public ProblemasOrderSearchModel()
        {
            AvailableProblemaOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Reports.LowStock.SearchManufacturerProblema")]
        public int SearchProblemaId { get; set; }
        public IList<SelectListItem> AvailableProblemaOptions { get; set; }

        #endregion
    }
    public partial record ProblemasProductSearchModel : BaseSearchModel
    {
        #region Ctor

        public ProblemasProductSearchModel()
        {
            AvailableProblemaOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Reports.LowStock.SearchManufacturerProblema")]
        public int SearchProblemaId { get; set; }
        public IList<SelectListItem> AvailableProblemaOptions { get; set; }

        #endregion
    }
}
