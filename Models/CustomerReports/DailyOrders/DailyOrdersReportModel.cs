using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DailyOrders
{
    public partial record DailyOrdersReportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Reports.Other.DailyOrders.Fields.Date")]
        public DateTime Date { get; set; }

        [NopResourceDisplayName("Admin.Reports.Other.DailyOrders.Fields.FormattedDate")]
        public string FormattedDate => Date.ToString("yyyy-MM-dd");

        [NopResourceDisplayName("Admin.Reports.Other.DailyOrders.Fields.Count")]
        public int Count { get; set; }

        [NopResourceDisplayName("Admin.Reports.Other.DailyOrders.Fields.ReturningCustomerCount")]
        public int ReturningCustomerCount { get; set; }
    }
}
