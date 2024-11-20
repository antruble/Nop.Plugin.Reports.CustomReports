using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.CustomerReports.ReturnedOrders
{
    public partial record ReturnedOrdersReportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Reports.Other.ReturnedOrdersReportModel.Fields.PartialAmount")]
        public int PartialAmount { get; set; }
    }
}
