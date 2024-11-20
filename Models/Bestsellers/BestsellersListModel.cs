using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DailyOrders;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.Bestsellers
{
    public partial record BestsellersListModel : BasePagedListModel<BestsellersReportModel>
    {
    }
}
