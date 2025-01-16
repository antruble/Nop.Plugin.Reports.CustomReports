using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Models.Bestsellers;
using Nop.Plugin.Reports.CustomReports.Models.CustomerId;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DailyOrders;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DiscountModels;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.RegisteredCustomers;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.ReturnedOrders;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.ShoperiaPlusSubscriptions;
using Nop.Plugin.Reports.CustomReports.Models.OrderDetails;
using Nop.Plugin.Reports.CustomReports.Models.OrderSummary;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasManufacturer;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasOrder;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasProduct;
using Nop.Plugin.Reports.CustomReports.Models.PromotionSummary;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Services;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Factories;


using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Reports.CustomReports.Factories
{
    public class BaseReportFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;


        private readonly IReportsModelFactory _customerReportsModelFactory;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public BaseReportFactory(
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            ILocalizationService localizationService,
            IWorkContext workContext,

            IReportsModelFactory customerReportsModelFactory,
            ILogger logger
            ) 
        { 
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _localizationService = localizationService;
            _workContext = workContext;

            _customerReportsModelFactory = customerReportsModelFactory;
            _logger = logger;
        }
        #endregion

        #region Methods
        
        /// <summary>
        /// A riportok keresési modelljének előkészítéséhez használatos generikus metódus, amely az összes BaseSearchModel-ból származó keresési modellt előkészít.
        /// </summary>
        /// <typeparam name="TSearchModel">A keresési modell típusa, amely a BaseSearchModel-ből származik.</typeparam>
        /// <returns>Az előkészített keresési modell.</returns>
        public async Task<TSearchModel> PrepareSearchModelAsync<TSearchModel>() where TSearchModel : BaseSearchModel, new()
        {
            // Search model beállítása 
            var searchModel = await BuildSearchModelAsync<TSearchModel>();

            searchModel.SetGridPageSize();
            return searchModel;
        }
        
        /// <summary>
        /// Generikus metódus a ripotokhoz tartozó lista modell előkészítésére.
        /// A ripotban megjelenítendő adatok lekérdezése és a listamodell megjelenésének (grid) előkészítése.
        /// </summary>
        /// <typeparam name="TSearchModel">A keresési modell típusa.</typeparam>
        /// <typeparam name="TListModel">A lista modell típusa.</typeparam>
        /// <typeparam name="TReportModel">A riport modell típusa.</typeparam>
        /// <param name="searchModel">A keresési modell példánya.</param>
        /// <returns>A lista modell.</returns>
        public async Task<TListModel> PrepareListModelAsync<TSearchModel, TListModel, TReportModel>(TSearchModel searchModel)
            where TSearchModel : BaseSearchModel
            where TListModel : BasePagedListModel<TReportModel>, new()
            where TReportModel : BaseNopModel, new()
        {
            // Adatok lekérdezése
            var reportData = await FetchReportDataAsync<TSearchModel, TReportModel>(searchModel);

            // Oldalakra osztás
            var pagesList = reportData.ToPagedList(searchModel);

            // Lista modell előkészítése
            var model = new TListModel().PrepareToGrid(searchModel, pagesList, () => pagesList);
            return model;
        }

        /// <summary>
        /// Dinamikusan építi fel a keresési modelleket az adott típus alapján.
        /// A metódus ellenőrzi a generikus típus paraméterét, és az annak megfelelő 
        /// keresési modellt állítja elő egyéni logika szerint.
        /// </summary>
        /// <typeparam name="TSearchModel">A keresési modell típusa, amelyet fel kell építeni. Ennek öröklődnie kell a BaseSearchModel osztályból.</typeparam>
        /// <returns>A keresési modell az adott típusnak megfelelően felépítve.</returns>
        private async Task<TSearchModel> BuildSearchModelAsync<TSearchModel>() 
            where TSearchModel : BaseSearchModel, new()
        {
            if (typeof(TSearchModel) == typeof(BestsellerSearchModel))
            {
                var result = await BuildBestsellerSearchModelAsync(new BestsellerSearchModel());
                return result as TSearchModel;
            }
            else if (typeof(TSearchModel) == typeof(ProblemasManufacturerSearchModel))
            {
                var result = await BuildProblemasManufacturerSearchModelAsync(new ProblemasManufacturerSearchModel());
                return result as TSearchModel;
            }
            else if (typeof(TSearchModel) == typeof(ProblemasOrderSearchModel))
            {
                var result = await BuildProblemasOrderSearchModelAsync(new ProblemasOrderSearchModel());
                return result as TSearchModel;
            }
            else if (typeof(TSearchModel) == typeof(ProblemasProductSearchModel))
            {
                var result = await BuildProblemasProductSearchModelAsync(new ProblemasProductSearchModel());
                return result as TSearchModel;
            }
            else if (typeof(TSearchModel) == typeof(OrderDetailsSearchModel))
            {
                var result = await BuildOrderDetailsSearchModelAsync(new OrderDetailsSearchModel());
                return result as TSearchModel;
            }
            else if (typeof(TSearchModel) == typeof(OrderSummarySearchModel))
            {
                var result = await BuildOrderSummarySearchModelAsync(new OrderSummarySearchModel());
                return result as TSearchModel;
            }
            else if (typeof(TSearchModel) == typeof(PromotionSummarySearchModel))
            {
                var result = await BuildPromotionSummarySearchModelAsync(new PromotionSummarySearchModel());
                return result as TSearchModel;
            }
            else if (typeof(TSearchModel) == typeof(SingleDateSearchModel))
            {
                return new SingleDateSearchModel() as TSearchModel;
            }
            // További search model építési metódusok helye...
            //else if (typeof(TSearchModel) == typeof(ProblemasProductSearchModel))
            //{
            //    var result = await BuildProblemasProductSearchModelAsync(new ProblemasProductSearchModel());
            //    return result as TSearchModel;
            //}
            else if (typeof(TSearchModel) == typeof(EmptySearchModel))
            {
                return new EmptySearchModel() as TSearchModel;
            }
            else
            {
                return new TSearchModel();
            }
        }

        /// <summary>
        /// A riport adatok lekérdezésére szolgáló generikus metódus.
        /// A riport modell típusának megfelelően hívja meg a konkrét riport adatlekérdezési metódust.
        /// </summary>
        /// <typeparam name="TSearchModel">A keresési modell típusa.</typeparam>
        /// <typeparam name="TReportModel">A riport modell típusa.</typeparam>
        /// <param name="searchModel">A keresési modell példánya.</param>
        /// <returns>A riport adatai listaként.</returns>
        private async Task<List<TReportModel>> FetchReportDataAsync<TSearchModel, TReportModel>(TSearchModel searchModel)
        {
            switch (typeof(TReportModel))
            {
                #region ÜGYFÉLSZOLGÁLATI RIPORTOK

                case Type reportType when reportType == typeof(DailyOrdersReportModel) && searchModel is CustomerReportsSearchModel customReportsSearchModel:
                    var dailyOrdersResult = await _customerReportsModelFactory.FetchDailyOrdersDataAsync(customReportsSearchModel);
                    return dailyOrdersResult.Cast<TReportModel>().ToList();

                case Type reportType when reportType == typeof(DiscountReportModel) && searchModel is CustomerReportsSearchModel customReportsSearchModel:
                    var discountsResult = await _customerReportsModelFactory.FetchDiscountsDataAsync(customReportsSearchModel);
                    return discountsResult.Cast<TReportModel>().ToList();

                case Type reportType when reportType == typeof(RegisteredCustomersReportModel) && searchModel is CustomerReportsSearchModel customReportsSearchModel:
                    var registeredCustomersResult = await _customerReportsModelFactory.FetchRegisteredCustomersDataAsync(customReportsSearchModel);
                    return registeredCustomersResult.Cast<TReportModel>().ToList();

                case Type reportType when reportType == typeof(ReturnedOrdersReportModel) && searchModel is CustomerReportsSearchModel customReportsSearchModel:
                    var returnedOrdersResult = await _customerReportsModelFactory.FetchReturnedOrdersDataAsync(customReportsSearchModel);
                    return returnedOrdersResult.Cast<TReportModel>().ToList();

                case Type reportType when reportType == typeof(ShoperiaPlusSubscriptionsReportModel) && searchModel is CustomerReportsSearchModel customReportsSearchModel:
                    var shoperiaPlusSubscriptionsResult = await _customerReportsModelFactory.FetchShoperiaPlusSubscriptionsDataAsync(customReportsSearchModel);
                    return shoperiaPlusSubscriptionsResult.Cast<TReportModel>().ToList();


                #endregion

                #region Bestsellers

                case Type reportType when reportType == typeof(BestsellersReportModel) && searchModel is BestsellerSearchModel bestsellerSearchModel:
                    var bestsellersResult = await _customerReportsModelFactory.FetchBestsellersDataAsync(bestsellerSearchModel);
                    return bestsellersResult.Cast<TReportModel>().ToList();


                #endregion

                #region Problémások

                case Type reportType when reportType == typeof(ProblemasManufacturerReportModel) && searchModel is ProblemasManufacturerSearchModel problemasManufacturerSearchModel:
                    var problemasManufacturerResult = await _customerReportsModelFactory.FetchProblemasManufacturerDataAsync(problemasManufacturerSearchModel);
                    return problemasManufacturerResult.Cast<TReportModel>().ToList();

                case Type reportType when reportType == typeof(ProblemasOrderReportModel) && searchModel is ProblemasOrderSearchModel problemasOrderSearchModel:
                    var problemasOrderResult = await _customerReportsModelFactory.FetchProblemasOrderDataAsync(problemasOrderSearchModel);
                    return problemasOrderResult.Cast<TReportModel>().ToList();

                case Type reportType when reportType == typeof(ProblemasProductReportModel) && searchModel is ProblemasProductSearchModel problemasProductSearchModel:
                    var problemasProductResult = await _customerReportsModelFactory.FetchProblemasProductDataAsync(problemasProductSearchModel);
                    return problemasProductResult.Cast<TReportModel>().ToList();


                #endregion

                #region OrderDetails
                
                case Type reportType when reportType == typeof(OrderDetailsReportModel) && searchModel is OrderDetailsSearchModel orderDetailsSearchModel:
                    var orderDetailsResult = await _customerReportsModelFactory.FetchOrderDetailsDataAsync(orderDetailsSearchModel);
                    return orderDetailsResult.Cast<TReportModel>().ToList();
                
                #endregion

                #region OrderSummary
                
                case Type reportType when reportType == typeof(OrderSummaryReportModel) && searchModel is OrderSummarySearchModel orderSummarySearchModel:
                    var orderSummaryResult = await _customerReportsModelFactory.FetchOrderSummaryDataAsync(orderSummarySearchModel);
                    return orderSummaryResult.Cast<TReportModel>().ToList();

                #endregion

                #region PromotionSummary

                case Type reportType when reportType == typeof(PromotionSummaryReportModel) && searchModel is PromotionSummarySearchModel promotionSummarySearchModel:
                    var promotionSummaryResult = await _customerReportsModelFactory.FetchPromotionSummaryDataAsync(promotionSummarySearchModel);
                    return promotionSummaryResult.Cast<TReportModel>().ToList();

                #endregion

                #region CustomerId

                case Type reportType when reportType == typeof(CustomerIdReportModel) && searchModel is SingleDateSearchModel customerIdSearchModel:
                    var customerIdResult = await _customerReportsModelFactory.FetchCustomerIdDataAsync(customerIdSearchModel);
                    return customerIdResult.Cast<TReportModel>().ToList();

                #endregion

                default:
                    throw new InvalidOperationException($"Invalid search model type for {typeof(TReportModel).Name}.");
            }

        }

        // Search model builds: A konkrét keresési modellek felépítéséhez használt metódusok
        #region Search model builds

        private async Task<BestsellerSearchModel> BuildBestsellerSearchModelAsync(BestsellerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);
            //prepare available order statuses
            await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);

            //prepare available payment statuses
            await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available billing countries
            searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
                .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
            searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);


            return searchModel;
        }
        private async Task<ProblemasManufacturerSearchModel> BuildProblemasManufacturerSearchModelAsync(ProblemasManufacturerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Manufacturer.Search.Valassz")
            });
            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Manufacturer.Search.NincsKep")
            });

            return searchModel;
        }
        private async Task<ProblemasOrderSearchModel> BuildProblemasOrderSearchModelAsync(ProblemasOrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Order.Search.Valassz")
            });
            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Order.Search.NincsSzamla")
            });
            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Order.Search.RegotaLezaratlan")
            });

            return searchModel;
        }
        private async Task<ProblemasProductSearchModel> BuildProblemasProductSearchModelAsync(ProblemasProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Search.Valassz")
            });
            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Search.NincsKep")
            });
            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Search.NincsKategoria")
            });
            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "3",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Search.KeszletDeNemAktiv")
            });
            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "4",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Search.NincsGyarto")
            });
            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "5",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Search.NincsLeiras")
            });
            searchModel.AvailableProblemaOptions.Add(new SelectListItem
            {
                Value = "6",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Search.NincsTermekSpec")
            });


            return searchModel;
        }
        private async Task<OrderDetailsSearchModel> BuildOrderDetailsSearchModelAsync(OrderDetailsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare OrderStatusOptions
            //await PrepareOrderStatusesAsync(searchModel.OrderStatusOptions);

            //prepare ShippingOptions
            //await PrepareShippingMethodsAsync(searchModel.ShippingOptions);

            //prepare PaymentOptions
            //await PreparePaymentsAsync(searchModel.PaymentOptions);

            return searchModel;
        }
        private async Task<OrderSummarySearchModel> BuildOrderSummarySearchModelAsync(OrderSummarySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare OrderStatusOptions
            await PreparePeriodTypesAsync(searchModel.PeriodTypeOptions);

            return searchModel;
        }

        private async Task<PromotionSummarySearchModel> BuildPromotionSummarySearchModelAsync(PromotionSummarySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailableCategoryOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.PromotionSummary.Search.OnPromotions")
            });
            searchModel.AvailableCategoryOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.PromotionSummary.Search.OnBrands")
            });
            searchModel.AvailableCategoryOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.PromotionSummary.Search.OnCategories")
            });


            return searchModel;
        }
        #endregion

        #region Utilities

        /// <summary>
        /// Prepare default item
        /// </summary>
        /// <param name="items">Available items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use "All" text</param>
        /// <param name="defaultItemValue">Default item value; defaults 0</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task PrepareDefaultItemAsync(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null, string defaultItemValue = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //prepare item text
            defaultItemText ??= await _localizationService.GetResourceAsync("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = string.Empty });
        }

        /// <summary>
        /// Prepare available shipping methods
        /// </summary>
        /// <param name="items">Shipping method items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        private async Task PrepareShippingMethodsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available order statuses
            items.Add(new SelectListItem
            {
                Value = "GLS",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.Shipping.Search.GLS")
            });
            items.Add(new SelectListItem
            {
                Value = "ExpressOne",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.Shipping.Search.ExpressOne")
            });
            items.Add(new SelectListItem
            {
                Value = "PersonalReceipt",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.Shipping.Search.PersonalReceipt")
            });

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }
        
        /// <summary>
        /// Prepare available order statuses
        /// </summary>
        private async Task PrepareOrderStatusesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available order statuses
            items.Add(new SelectListItem
            {
                Value = "Completed",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.OrderStatus.Search.Completed")
            });
            items.Add(new SelectListItem
            {
                Value = "Deleted",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.OrderStatus.Search.Deleted")
            });
            items.Add(new SelectListItem
            {
                Value = "Returned",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.OrderStatus.Search.Returned")
            });
            items.Add(new SelectListItem
            {
                Value = "OnTheWay",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.OrderStatus.Search.OnTheWay")
            });
            items.Add(new SelectListItem
            {
                Value = "Processing",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.OrderStatus.Search.Processing")
            });
            items.Add(new SelectListItem
            {
                Value = "Test",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.OrderStatus.Search.Test")
            });

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available payment methods
        /// </summary>
        private async Task PreparePaymentsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available order statuses
            items.Add(new SelectListItem
            {
                Value = "Payments.CashOnDelivery",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.Payment.Search.CashOnDelivery")
            });
            items.Add(new SelectListItem
            {
                Value = "Payments.SimplePay",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.Payment.Search.SimplePay")
            });

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }
        private async Task PreparePeriodTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available order statuses
            items.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderSummary.Search.Daily")
            });
            items.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.OrderSummary.Search.Monthly")
            });

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }
        
        #endregion

        #endregion

    }
}
