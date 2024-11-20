using Nop.Plugin.Reports.CustomReports.Models.Bestsellers;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DailyOrders;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DiscountModels;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.RegisteredCustomers;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.ReturnedOrders;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.ShoperiaPlusSubscriptions;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasManufacturer;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasOrder;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasProduct;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Factories.CustomerReports
{
    public partial interface IReportsModelFactory
    {



        #region Ügyfélszolgálati riportok

        Task<List<DailyOrdersReportModel>> FetchDailyOrdersDataAsync(CustomerReportsSearchModel searchModel);
        Task<List<DiscountReportModel>> FetchDiscountsDataAsync(CustomerReportsSearchModel searchModel);
        Task<List<RegisteredCustomersReportModel>> FetchRegisteredCustomersDataAsync(CustomerReportsSearchModel searchModel);
        Task<List<ShoperiaPlusSubscriptionsReportModel>> FetchShoperiaPlusSubscriptionsDataAsync(CustomerReportsSearchModel searchModel);
        Task<List<ReturnedOrdersReportModel>> FetchReturnedOrdersDataAsync(CustomerReportsSearchModel searchModel);

        #endregion

        #region Bestseller

        Task<List<BestsellersReportModel>> FetchBestsellersDataAsync(BestsellerSearchModel searchModel);

        #endregion

        #region Problémások

        Task<List<ProblemasManufacturerReportModel>> FetchProblemasManufacturerDataAsync(ProblemasManufacturerSearchModel searchModel);
        Task<List<ProblemasOrderReportModel>> FetchProblemasOrderDataAsync(ProblemasOrderSearchModel searchModel);
        Task<List<ProblemasProductReportModel>> FetchProblemasProductDataAsync(ProblemasProductSearchModel searchModel);

        #endregion
    }
}
