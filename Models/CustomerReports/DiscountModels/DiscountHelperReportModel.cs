﻿using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DiscountModels
{
    public class DiscountHelperReportModel
    {
        public int DiscountTypeId { get; set; }

        public int UsageCount { get; set; }

        public decimal TotalDiscountAmount { get; set; }
    }
}