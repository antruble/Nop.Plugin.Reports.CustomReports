using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.CustomerReports.RegisteredCustomers
{
    public partial record RegisteredCustomersReportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Reports.Other.RegisteredCustomers.Fields.PartialAmount")]
        public int PartialAmount { get; set; }

        [NopResourceDisplayName("Admin.Reports.Other.RegisteredCustomers.Fields.TotalAmount")]
        public int TotalAmount { get; set; }

    }
}
