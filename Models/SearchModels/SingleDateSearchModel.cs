using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Models.SearchModels
{
    public partial record SingleDateSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Reports.SingleDateSearchModel.Date")]
        [UIHint("DateNullable")]
        public DateTime? Date { get; set; }

        public SingleDateSearchModel() 
        { 
        }
    }
}
