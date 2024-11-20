using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.Bestsellers
{
    public partial record BestsellersReportModel : BaseNopModel
    {
        #region Properties

        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.Name")]
        public string ProductName { get; set; }

        [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.TotalAmount")]
        public string TotalAmount { get; set; }

        [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.TotalQuantity")]
        public decimal TotalQuantity { get; set; }

        [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.EAN")]
        public string EAN { get; set; }

        [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.AktualisKeszlet")]
        public int AktualisKeszlet { get; set; }

        [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.ArParitas")]
        public decimal ArParitas { get; set; }

        #endregion
    }
}
