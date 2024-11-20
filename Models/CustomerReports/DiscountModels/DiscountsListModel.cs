using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DiscountModels
{
    public partial record DiscountsListModel : BasePagedListModel<DiscountReportModel>
    {
    }
}
