using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Plugin.Reports.CustomReports.Models.CustomerReports.DiscountModels;
using Nop.Plugin.Reports.CustomReports.Models.OrderDetails;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Services
{
    /// <summary>
    /// A CustomReportsService osztály felelős különböző riportokhoz tartozó adatok elkészítéséért.
    /// </summary>
    public class CustomReportService
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderReportService _orderReportService;

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<DiscountUsageHistory> _discountUsageHistoryRepository;
        private readonly IRepository<Discount> _discountRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Shipment> _shipmentRepository;

        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public CustomReportService(

                ICategoryService categoryService,
                IDateTimeHelper dateTimeHelper,
                ILocalizationService localizationService,
                IOrderReportService orderReportService,

                IRepository<Customer> customerRepository,
                IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
                IRepository<Discount> discountRepository,
                IRepository<Order> orderRepository,
                IRepository<OrderItem> orderItemRepository,
                IRepository<OrderNote> orderNoteRepository,
                IRepository<Shipment> shipmentRepository,

                IWorkContext workContext,
                ILogger logger)
        {
            _categoryService = categoryService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _orderReportService = orderReportService;

            _customerRepository = customerRepository;
            _discountUsageHistoryRepository = discountUsageHistoryRepository;
            _discountRepository = discountRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderNoteRepository = orderNoteRepository;
            _shipmentRepository = shipmentRepository;

            _workContext = workContext;
            _logger = logger;
        }

        #endregion

        #region Methods

        #region Ügyfélszolgálati riportokhoz tartozó metódusok
        /// <summary>
        /// Ellenőrzi, hogy a vásárló visszatérő-e, azaz van-e más rendelése a megadott rendeléstől eltérően.
        /// </summary>
        /// <param name="customerId">A vásárló egyedi azonosítója.</param>
        /// <param name="orderGuid">Az aktuális rendelés azonosítója.</param>
        /// <returns>Igaz, ha van másik rendelése a vásárlónak, különben hamis.</returns>
        public async Task<bool> IsReturningCustomerAsync(int customerId, Guid orderGuid)
        {
            // Lekérdezés
            var query = from o in _orderRepository.Table
                        where o.CustomerId == customerId && o.OrderGuid != orderGuid
                        select o;

            // Van-e volt rendelése, vagy nem
            var hasOtherOrders = await query.AnyAsync();

            return hasOtherOrders;
        }

        /// <summary>
        /// Visszaadja a kupon riport modellt a megadott időszakra vonatkozóan.
        /// Csoportosítja a kuponokat típus alapján, és összesíti a felhasználások számát és a teljes kedvezmény összegét.
        /// </summary>
        /// <param name="createdFromUtc">Az intervallum kezdő dátuma UTC idő szerint.</param>
        /// <param name="createdToUtc">Az intervallum záró dátuma UTC idő szerint.</param>
        /// <returns>Lista a kupon típusokról, felhasználások számáról és a teljes kedvezmény összegéről.</returns>
        public async Task<IList<DiscountReportModel>> GetDiscountReportModelListByDateAsync(DateTime? createdFromUtc, DateTime? createdToUtc)
        {
            var query = from duh in _discountUsageHistoryRepository.Table
                        join o in _orderRepository.Table on duh.OrderId equals o.Id
                        join d in _discountRepository.Table on duh.DiscountId equals d.Id
                        join oi in _orderItemRepository.Table on duh.OrderId equals oi.OrderId
                        where (!createdFromUtc.HasValue || duh.CreatedOnUtc >= createdFromUtc.Value) &&
                              (!createdToUtc.HasValue || duh.CreatedOnUtc <= createdToUtc.Value)
                        group new { duh, o, d, oi } by d.DiscountTypeId into g
                        select new DiscountReportModel
                        {
                            DiscountTypeName = ((DiscountType)g.Key).ToString(), 
                            UsageCount = g.Select(x => x.o.Id).Distinct().Count(),  
                            TotalDiscountAmount =  g.Sum(x => x.o.OrderSubTotalDiscountInclTax) + 
                                                   g.Sum(x => x.d.DiscountAmount) + 
                                                   g.Sum(x => x.oi.DiscountAmountExclTax)
                        };

            var result = await query.ToListAsync();

            return result;
        }

        /// <summary>
        /// Visszaadja az összes regisztrált vásárló számát és az időszakban regisztrált vásárlók számát.
        /// </summary>
        /// <param name="createdFromUtc">Az intervallum kezdő dátuma  UTC idő szerint.</param>
        /// <param name="createdToUtc">Az intervallum záró dátuma UTC idő szerint.</param>
        /// <returns>Tuple, amely tartalmazza az összes vásárló és az időszak alatt regisztrált vásárlók számát.</returns>
        public async Task<(int TotalRegisteredCustomers, int RegisteredCustomersInPeriod)> GetRegisteredCustomersCountsAsync(DateTime? createdFromUtc, DateTime? createdToUtc)
        {
            // Az összes regisztrált customer száma
            var totalRegisteredCustomers = await _customerRepository.Table
                .CountAsync(c => true);  // Minden regisztrált vásárlót lekérünk

            // Az időszakban regisztrált vásárlók száma (CreatedOnUtc alapján)
            var registeredCustomersInPeriod = await _customerRepository.Table
                .CountAsync(c =>
                                 (!createdFromUtc.HasValue || c.CreatedOnUtc >= createdFromUtc.Value) &&
                                 (!createdToUtc.HasValue || c.CreatedOnUtc <= createdToUtc.Value));

            // Visszaadunk egy tuple-t, ami tartalmazza a teljes számot és az időszakon belüliek számát
            return (totalRegisteredCustomers, registeredCustomersInPeriod);
        }

        /// <summary>
        /// Visszaadja a Shoperia Plus előfizetések számát összesen és az adott intervallumban.
        /// </summary>
        /// <param name="createdFromUtc">Az intervallum kezdő dátuma UTC idő szerint.</param>
        /// <param name="createdToUtc">Az intervallum záró dátuma UTC idő szerint.</param>
        /// <returns>Tuple, amely tartalmazza az összes Shoperia Plus előfizetőt és az időszak alatt feliratkozók számát.</returns>
        public async Task<(int TotalShoperiaPlusSubscriptions, int ShoperiaPlusSubscriptionsInPeriod)> GetShoperiaSubscriptionsCountsAsync(DateTime? createdFromUtc, DateTime? createdToUtc)
        {
            // Az összes customer száma, akiknél a Husegprogram mező igaz
            var totalShoperiaPlusSubscriptions = await _customerRepository.Table
                .CountAsync(c => c.Husegprogram);

            // Az időszakban feliratkozott hűségprogramos vásárlók száma (HusegCsatlakozasDatum alapján)
            var shoperiaPlusSubscriptionsInPeriod = await _customerRepository.Table
                .CountAsync(c => c.Husegprogram &&
                                 c.HusegCsatlakozasDatum.HasValue &&
                                 (!createdFromUtc.HasValue || c.HusegCsatlakozasDatum >= createdFromUtc.Value) &&
                                 (!createdToUtc.HasValue || c.HusegCsatlakozasDatum <= createdToUtc.Value));

            // Visszaadunk egy tuple-t, ami tartalmazza a teljes számot és az időszakon belüliek számát
            return (totalShoperiaPlusSubscriptions, shoperiaPlusSubscriptionsInPeriod);
        }

        /// <summary>
        /// Visszaadja az időszak alatt visszaküldött rendelések számát.
        /// </summary>
        /// <param name="createdFromUtc">Az intervallum kezdő dátuma UTC idő szerint.</param>
        /// <param name="createdToUtc">Az intervallum záró dátuma UTC idő szerint.</param>
        /// <returns>Az időszak alatt visszaküldött rendelések száma.</returns>
        public async Task<int> GetReturnedOrdersCountsAsync(DateTime? createdFromUtc, DateTime? createdToUtc)
        {
            var query = from order in _orderRepository.Table
                        join shipment in _shipmentRepository.Table on order.Id equals shipment.OrderId
                        where order.OrderStatusId == 40 // Cancelled
                              && shipment.DeliveryDateUtc != null // Csak a szállítással rendelkező rendelések
                              && (!createdFromUtc.HasValue || shipment.DeliveryDateUtc >= createdFromUtc.Value)
                              && (!createdToUtc.HasValue || shipment.DeliveryDateUtc <= createdToUtc.Value)
                        select order.Id; // Csak az érintett rendelés ID-k

            // Visszaadjuk az érintett rendelések számát
            return await query.Distinct().CountAsync();
        }

        #endregion

        #region Bestseller riporthoz tartozó metódusok
        
        public async Task<string> TermekKategoriaProblemaSzoveg(Product product)
        {

            string uzenet = await _localizationService.GetResourceAsync("Admin.Reports.Problemas.Problema.Kategoria");
            var termekKategoria = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, showHidden: true);
            if (termekKategoria.Count == 0)
            {
                uzenet = uzenet + " NINCS";
            }
            else
            {
                foreach (var category in termekKategoria)
                {

                    var kategoria = await _categoryService.GetCategoryByIdAsync(category.CategoryId);
                    uzenet = uzenet + kategoria.Name + ", ";
                }
            }

            return uzenet;
        }
        public async Task<IPagedList<BestsellersReportLine>> GetBestsellersReportAsync(BestsellerSearchModel searchModel)
        {
            //get parameters to filter bestsellers
            var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
            var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.VendorId = currentVendor.Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //get bestsellers
            var bestsellers = await _orderReportService.BestSellersReportAsync(showHidden: true,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                billingCountryId: searchModel.BillingCountryId,
                orderBy: OrderByEnum.OrderByTotalAmount,
                vendorId: searchModel.VendorId,
                categoryId: searchModel.CategoryId,
                manufacturerId: searchModel.ManufacturerId,
                storeId: searchModel.StoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            return bestsellers;
        }

        #endregion

        #region OrderDetails
        public async Task<IList<OrderDetailsReportModel>> GetOrderDetailsReportModelListByDateAsync(OrderDetailsSearchModel searchModel)
        {
            // Base query with the required fields
            var query = from order in _orderRepository.Table
                        join shipment in _shipmentRepository.Table on order.Id equals shipment.OrderId into shipmentGroup
                        from shipment in shipmentGroup.DefaultIfEmpty()
                        join note in _orderNoteRepository.Table on order.Id equals note.OrderId
                        where note.Note.Contains("Order status has been changed to Feldolgozás alatt") &&
                              (!searchModel.StartDate.HasValue || note.CreatedOnUtc >= searchModel.StartDate.Value) &&
                              (!searchModel.EndDate.HasValue || note.CreatedOnUtc <= searchModel.EndDate.Value)
                        select new OrderDetailsReportModel
                        {
                            OrderId = order.Id,
                            Status = order.TesztRendeles
                                ? "Teszt"
                                : order.Deleted
                                    ? "Törölve"
                                    : order.OrderStatusId == (int)OrderStatus.Cancelled
                                        ? order.ShippingStatusId == (int)ShippingStatus.VisszajottEllenorzesreVar
                                            ? "Visszajött"
                                            : ((OrderStatus)order.OrderStatusId).ToString()
                                        : order.OrderStatusId == (int)OrderStatus.Complete
                                            ? "Teljesítve"
                                            : ((OrderStatus)order.OrderStatusId).ToString(),
                            ProcessingAvailableDate = note.CreatedOnUtc,
                            PackageCompletedDate = shipment.CreatedOnUtc,
                            ShippedDate = shipment.ShippedDateUtc,
                            DeliveredDate = shipment.DeliveryDateUtc,
                            ShippingMethod = !string.IsNullOrEmpty(shipment.SzallitasiMod) 
                                    ? string.Equals(shipment.SzallitasiMod, "GLS")
                                        ? "GLS Futár"
                                        : shipment.SzallitasiMod
                                    : string.Equals(order.ShippingMethod, "GLS")
                                        ? "GLS Futár"
                                        : order.ShippingMethod,
                            PaymentMethod = order.PaymentMethodSystemName == "Payments.CashOnDelivery"
                                    ? "Utánvét"
                                    : order.PaymentMethodSystemName == "Payments.SimplePay"
                                        ? "SimplePay"
                                        : !string.IsNullOrEmpty(order.PaymentMethodSystemName)
                                            ? "order.PaymentMethodSystemName"
                                            : "MISSING",
                            PackagingTime = shipment.CreatedOnUtc - note.CreatedOnUtc
                        };

            
            var data = await query.ToListAsync();

            foreach (var item in data)
            {
                item.ProcessingAvailableDate = item.ProcessingAvailableDate.HasValue 
                    ? await _dateTimeHelper.ConvertToUserTimeAsync(item.ProcessingAvailableDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null;
                item.PackageCompletedDate = item.PackageCompletedDate.HasValue
                    ? await _dateTimeHelper.ConvertToUserTimeAsync(item.PackageCompletedDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null;
                item.ShippedDate = item.ShippedDate.HasValue
                    ? await _dateTimeHelper.ConvertToUserTimeAsync(item.ShippedDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null;
                item.DeliveredDate = item.DeliveredDate.HasValue
                    ? await _dateTimeHelper.ConvertToUserTimeAsync(item.DeliveredDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null;
                item.PackagingTime = item.PackageCompletedDate.HasValue && item.ProcessingAvailableDate.HasValue 
                    ? item.PackageCompletedDate - item.ProcessingAvailableDate
                    : null;
            }

            return data;
        }

        #endregion
        
        #endregion
    }
}
