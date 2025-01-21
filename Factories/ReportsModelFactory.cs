using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Orders;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILogger = Nop.Services.Logging.ILogger;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.ReturnedOrders;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.RegisteredCustomers;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DiscountModels;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.ShoperiaPlusSubscriptions;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DailyOrders;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Plugin.Reports.CustomReports.Models.Bestsellers;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasManufacturer;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasOrder;
using Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasProduct;
using Nop.Services.Localization;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models.Extensions;
using Nop.Plugin.Reports.CustomReports.Services;
using Nop.Plugin.Reports.CustomReports.Models.OrderDetails;
using Nop.Plugin.Reports.CustomReports.Models.OrderSummary;
using Nop.Plugin.Reports.CustomReports.Models.PromotionSummary;
using Nop.Plugin.Reports.CustomReports.Models.CustomerId;

namespace Nop.Plugin.Reports.CustomReports.Factories.CustomerReports
{
    /// <summary>
    /// Ez az osztály felelős a különböző riportokhoz tartozó lista modell előkészítésért.
    /// </summary>
    public class ReportsModelFactory : IReportsModelFactory
    {
        #region Fields

        private readonly ReportDataService _reportDataService;

        private readonly ICategoryService _categoryService;
        private readonly ICustomerReportService _customerReportService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IOrderReportService _orderReportService;
        private readonly IProductService _productService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ReportsModelFactory(
            ReportDataService reportDataService,

            ICategoryService categoryService,
            ICustomerReportService customerReportService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IOrderService orderService,
            IOrderReportService orderReportService,
            IProductService productService,
            IPriceFormatter priceFormatter,
            ILogger logger
            )
        {
            _reportDataService = reportDataService;

            _categoryService = categoryService;
            _customerReportService = customerReportService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _orderService = orderService;
            _orderReportService = orderReportService;
            _productService = productService;
            _priceFormatter = priceFormatter;
            _logger = logger;
        }

        #endregion

        // A konkrét report modell listák (riport adatok) lekérdezésére szolgáló metódusok
        #region Methods

        #region Ügyfélszolgálati riport
        public async Task<List<DailyOrdersReportModel>> FetchDailyOrdersDataAsync(CustomerReportsSearchModel searchModel)
        {
            var startDateValue = searchModel.StartDate.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
                : null;
            var endDateValue = searchModel.EndDate.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1)
                : null;

            // Rendelések lekérdezése a megadott időszakra
            var orders = await _orderService.SearchOrdersShoperiaAsync(
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize
            );

            // Csoportosítás dátum alapján
            var groupedOrders = orders
                .GroupBy(o => o.CreatedOnUtc.Date)
                .Select(g => new DailyOrdersReportModel
                {
                    Date = g.Key,
                    Count = g.Count(),
                    ReturningCustomerCount = g.Count(o => _reportDataService.IsReturningCustomerAsync(o.CustomerId, o.OrderGuid).Result)
                })
                .ToList();

            return groupedOrders;
        }

        public async Task<List<DiscountReportModel>> FetchDiscountsDataAsync(CustomerReportsSearchModel searchModel)
        {
            var startDateValue = !searchModel.StartDate.HasValue ? null
                           : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            // Használt kuponok lekérdezése az időszakra
            var discountReportModelList = await _reportDataService.GetDiscountReportModelListByDateAsync(
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue
            );

            return discountReportModelList.ToList();
        }

        public async Task<List<RegisteredCustomersReportModel>> FetchRegisteredCustomersDataAsync(CustomerReportsSearchModel searchModel)
        {
            var result = new List<RegisteredCustomersReportModel>();
            var startDateValue = !searchModel.StartDate.HasValue ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            // Összes regisztráció, és az intervallumon belüli regisztrációk számának lekérdezése egy tupleként
            var amounts = await _reportDataService.GetRegisteredCustomersCountsAsync(startDateValue, endDateValue);

            // Egy soros report modell előkészítése
            result.Add(new RegisteredCustomersReportModel
            {
                PartialAmount = amounts.RegisteredCustomersInPeriod,  // Időszakban regisztrált vásárlók száma
                TotalAmount = amounts.TotalRegisteredCustomers // Összes vásárló száma
            });

            return result;
        }

        public async Task<List<ShoperiaPlusSubscriptionsReportModel>> FetchShoperiaPlusSubscriptionsDataAsync(CustomerReportsSearchModel searchModel)
        {
            var result = new List<ShoperiaPlusSubscriptionsReportModel>();
            var startDateValue = !searchModel.StartDate.HasValue ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            // Összes feliratkozás, és az intervallumon belüli feliratkózások számának lekérdezése egy tupleként
            var amounts = await _reportDataService.GetShoperiaSubscriptionsCountsAsync(startDateValue, endDateValue);

            // Egy soros report modell előkészítése
            result.Add(new ShoperiaPlusSubscriptionsReportModel
            {
                PartialAmount = amounts.ShoperiaPlusSubscriptionsInPeriod,  // Időszakban feliratkozott vásárlók száma
                TotalAmount = amounts.TotalShoperiaPlusSubscriptions// Összes feliratkózó száma
            });

            return result;
        }

        public async Task<List<ReturnedOrdersReportModel>> FetchReturnedOrdersDataAsync(CustomerReportsSearchModel searchModel)
        {
            var result = new List<ReturnedOrdersReportModel>();
            var startDateValue = !searchModel.StartDate.HasValue ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            // Visszaküldött rendelések számának lekérdezése az időszakban
            var amount = await _reportDataService.GetReturnedOrdersCountsAsync(startDateValue, endDateValue);

            // Egy soros report modell előkészítése
            result.Add(new ReturnedOrdersReportModel
            {
                PartialAmount = amount,  // Időszakban regisztrált vásárlók száma
            });


            return result;
        }

        #endregion
        
        #region Bestsellers
        public async Task<List<BestsellersReportModel>> FetchBestsellersDataAsync(BestsellerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var bestsellers = await _reportDataService.GetBestsellersReportAsync(searchModel);

            //prepare list model
            var productIds = bestsellers.Select(b => b.ProductId).Distinct().ToList();
            var products = await _productService.GetProductsByIdsAsync(productIds.ToArray());
            var productDict = products.ToDictionary(p => p.Id);

            var result = bestsellers.Select(bestseller =>
            {
                var product = productDict[bestseller.ProductId];
                return new BestsellersReportModel
                {
                    ProductId = bestseller.ProductId,
                    ProductName = product.Name,
                    EAN = product.Sku,
                    AktualisKeszlet = product.StockQuantity,
                    TotalQuantity = bestseller.TotalQuantity,
                    TotalAmount = _priceFormatter.FormatPriceAsync(bestseller.TotalAmount, true, false).Result
                };
            }).ToList();

            return result;

        }

        #endregion

        #region Problémások
        public async Task<List<ProblemasManufacturerReportModel>> FetchProblemasManufacturerDataAsync(ProblemasManufacturerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            // problémás márkák kigyűjtése
            var manufacturersKepNelkul = await _productService.GetProblemasManufacturerKepNelkuliAsync();

            //prepare low stock product models
            var problemasManufacturerModels = new List<ProblemasManufacturerReportModel>();

            //Kép nélkül
            if (searchModel.SearchProblemaId == 0 || searchModel.SearchProblemaId == 1)
            {
                problemasManufacturerModels.AddRange(await manufacturersKepNelkul.SelectAwait(async manufacturer => new ProblemasManufacturerReportModel
                {
                    Id = manufacturer.Id,
                    Name = manufacturer.Name,
                    NumberOfProducts = manufacturer.NumberOfProducts,
                    Published = manufacturer.Published,
                    Problema = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Manufacturer.Problema.NincsKep")
                }).ToListAsync());
            }

            return problemasManufacturerModels;
        }
        public async Task<List<ProblemasOrderReportModel>> FetchProblemasOrderDataAsync(ProblemasOrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            // problémás rendelések kigyüjtése
            var ordersSzamlaNelkul = await _orderReportService.GetProblemasOrdersSzamlaNelkuliAsync();
            var ordersLezaratlan = await _orderReportService.GetProblemasOrdersLezaratlanAsync();

            //prepare low stock product models
            var problemasOrderModels = new List<ProblemasOrderReportModel>();

            //Számla nélkül
            if (searchModel.SearchProblemaId == 0 || searchModel.SearchProblemaId == 1)
            {
                problemasOrderModels.AddRange(await ordersSzamlaNelkul.SelectAwait(async order => new ProblemasOrderReportModel
                {
                    Id = order.Id,
                    CustomOrderNumber = order.CustomOrderNumber,
                    CreatedOnUTC = order.CreatedOnUtc,
                    Szamlazva = order.Szamlazva,
                    Problema = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Order.Problema.NincsSzamla")
                }).ToListAsync());
            }
            // Régóta (legalább 2 hete) lezáratlan/kézbesítetlen rendelések
            if (searchModel.SearchProblemaId == 0 || searchModel.SearchProblemaId == 2)
            {
                problemasOrderModels.AddRange(await ordersSzamlaNelkul.SelectAwait(async order => new ProblemasOrderReportModel
                {
                    Id = order.Id,
                    CustomOrderNumber = order.CustomOrderNumber,
                    CreatedOnUTC = order.CreatedOnUtc,
                    Szamlazva = order.Szamlazva,
                    Problema = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Order.Problema.RegotaLezaratlan")
                }).ToListAsync());
            }

            return problemasOrderModels;
        }
        public async Task<List<ProblemasProductReportModel>> FetchProblemasProductDataAsync(ProblemasProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get low stock product and product combinations
            var productsKepnelkul = await _productService.GetProblemasProductsKepNelkulAsync();
            var productsKeszletDeNemAktiv = await _productService.GetProblemasProductsKeszletDeNemAktivAsync();
            var productsNincsBrand = await _productService.GetProblemasProductsBrandNelkuliAsync();
            var productsNincsLeiras = await _productService.GetProblemasProductsNincsLeirasAsync();
            var productsNincsTermekSpec = await _productService.GetProblemasProductsNincsTermekSpecAsync();

            var osszesKategoria = await _categoryService.GetAllCategoriesAsync(showHidden: true);
            var osszesTermek = await _productService.GetOsszesAktivProductsAsync();

            var productsProblemasKategoriaval = new List<Product>();
            foreach (var termek in osszesTermek)
            {
                var termekKategoriak = await _categoryService.GetProductCategoriesByProductIdAsync(termek.Id);
                if (termekKategoriak.Count == 0)
                {
                    productsProblemasKategoriaval.Add(termek);
                }
                else
                {
                    foreach (var kat in termekKategoriak)
                    {
                        var egykat = osszesKategoria.Where(k => k.Id == kat.CategoryId).FirstOrDefault();
                        if (egykat != null)
                        {
                            if (egykat.Published == false)
                            {
                                productsProblemasKategoriaval.Add(termek);
                            }
                        }
                        else
                        {
                            productsProblemasKategoriaval.Add(termek);
                        }
                    }
                }
            }

            //prepare low stock product models
            var problemasProductModels = new List<ProblemasProductReportModel>();

            //Kép nélkül
            if (searchModel.SearchProblemaId == 0 || searchModel.SearchProblemaId == 1)
            {
                problemasProductModels.AddRange(await productsKepnelkul.SelectAwait(async product => new ProblemasProductReportModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    StockQuantity = await _productService.GetTotalStockQuantityAsync(product),
                    Published = product.Published,
                    Problema = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Problema.NincsKep")
                }).ToListAsync());
            }
            //Problémás kategória
            if (searchModel.SearchProblemaId == 0 || searchModel.SearchProblemaId == 2)
            {
                problemasProductModels.AddRange(await productsProblemasKategoriaval.SelectAwait(async product => new ProblemasProductReportModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    StockQuantity = await _productService.GetTotalStockQuantityAsync(product),
                    Published = product.Published,
                    Problema = await _reportDataService.TermekKategoriaProblemaSzoveg(product)
                }).ToListAsync());
            }
            //Problémás termékek => Nem aktív de van készleten 
            if (searchModel.SearchProblemaId == 0 || searchModel.SearchProblemaId == 3)
            {
                problemasProductModels.AddRange(await productsKeszletDeNemAktiv.SelectAwait(async product => new ProblemasProductReportModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    StockQuantity = await _productService.GetTotalStockQuantityAsync(product),
                    Published = product.Published,
                    Problema = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Problema.KeszletDeNemAktiv")
                }).ToListAsync());
            }
            //Problémás termékek => Gyártó nélküli termékek
            if (searchModel.SearchProblemaId == 0 || searchModel.SearchProblemaId == 4)
            {
                problemasProductModels.AddRange(await productsNincsBrand.SelectAwait(async product => new ProblemasProductReportModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    StockQuantity = await _productService.GetTotalStockQuantityAsync(product),
                    Published = product.Published,
                    Problema = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Problema.NincsGyarto")
                }).ToListAsync());
            }
            //Problémás termékek => Leírás / Összetevők / Biztonsági előírások nélküli aktív termékek
            if (searchModel.SearchProblemaId == 0 || searchModel.SearchProblemaId == 5)
            {
                problemasProductModels.AddRange(await productsNincsLeiras.SelectAwait(async product => new ProblemasProductReportModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    StockQuantity = await _productService.GetTotalStockQuantityAsync(product),
                    Published = product.Published,
                    Problema = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Problema.NincsLeiras")
                }).ToListAsync());
            }
            //Problémás termékek => Termékspecifikáció nélküli termékek
            if (searchModel.SearchProblemaId == 0 || searchModel.SearchProblemaId == 6)
            {
                problemasProductModels.AddRange(await productsNincsTermekSpec.SelectAwait(async product => new ProblemasProductReportModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    StockQuantity = await _productService.GetTotalStockQuantityAsync(product),
                    Published = product.Published,
                    Problema = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Problema.NincsTermekSpec")
                }).ToListAsync());
            }

            return problemasProductModels;
        }
        #endregion

        #region OrderDetails
        public async Task<List<OrderDetailsReportModel>> FetchOrderDetailsDataAsync(OrderDetailsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var result = await _reportDataService.GetOrderDetailsReportModelListByDateAsync(searchModel);
            return result.ToList();
        }

        #endregion

        #region OrderSummary
        public async Task<List<OrderSummaryReportModel>> FetchOrderSummaryDataAsync(OrderSummarySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var result = await _reportDataService.GetOrderSummaryReportModelListAsync(searchModel);
            return result.ToList();
        }

        #endregion
        
        #region PromotionSummary
        public async Task<List<PromotionSummaryReportModel>> FetchPromotionSummaryDataAsync(PromotionSummarySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var result = await _reportDataService.GetPromotionSummaryReportModelListAsync(searchModel);
            return result.ToList();
        }

        #endregion

        #region CustomerId
        public async Task<List<CustomerIdReportModel>> FetchCustomerIdDataAsync(SingleDateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var result = await _reportDataService.GetCustomerIdReportModelListAsync(searchModel);
            return result.ToList();
        }
        #endregion

        #endregion
    }
}
