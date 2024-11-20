using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System;

namespace Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasOrder
{
    public partial record ProblemasOrderReportModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Orders.Fields.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Orders.Fields.CreationDate")]
        public DateTime CreatedOnUTC { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Orders.Fields.Problema")]
        public string Problema { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Orders.Fields.Szamlazva")]
        public bool Szamlazva { get; set; }

        #endregion
    }
}
