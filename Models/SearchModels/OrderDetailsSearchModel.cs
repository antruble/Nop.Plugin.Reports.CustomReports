using Microsoft.AspNetCore.Mvc.Rendering;
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
    /// <summary>
    /// Represents a daily order search model
    /// </summary>
    public partial record OrderDetailsSearchModel : BaseSearchModel
    {
        #region Ctor

        //public OrderDetailsSearchModel()
        //{
        //    OrderStatusOptions = new List<SelectListItem>();
        //    ShippingOptions = new List<SelectListItem>();
        //    PaymentOptions = new List<SelectListItem>();
        //}

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Reports.OrderDetails.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Reports.OrderDetails.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        //[NopResourceDisplayName("Admin.Reports.OrderDetails.OrderStatus")]
        //public string OrderStatus { get; set; }
        //[NopResourceDisplayName("Admin.Reports.OrderDetails.ShippingMethod")]
        //public string ShippingMethod { get; set; }
        //[NopResourceDisplayName("Admin.Reports.OrderDetails.PaymentMethodSystemName")]
        //public string PaymentMethodSystemName { get; set; }


        //public IList<SelectListItem> OrderStatusOptions { get; set; }
        //public IList<SelectListItem> ShippingOptions { get; set; }
        //public IList<SelectListItem> PaymentOptions { get; set; }

        #endregion
    }
}
