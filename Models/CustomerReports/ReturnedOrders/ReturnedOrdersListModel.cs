﻿using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.CustomerReports.ReturnedOrders
{
    public partial record ReturnedOrdersListModel : BasePagedListModel<ReturnedOrdersReportModel>
    {
    }
}