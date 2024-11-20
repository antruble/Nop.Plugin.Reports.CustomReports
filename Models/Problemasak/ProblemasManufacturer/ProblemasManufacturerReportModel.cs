using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;


namespace Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasManufacturer
{
    /// <summary>
    /// Probl
    /// </summary>
    public partial record ProblemasManufacturerReportModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Manufacturer.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Manufacturer.Fields.Problema")]
        public string Problema { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Manufacturer.Fields.NumberOfProducts")]
        public int NumberOfProducts { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Manufacturer.Fields.Published")]
        public bool Published { get; set; }

        #endregion
    }
}
