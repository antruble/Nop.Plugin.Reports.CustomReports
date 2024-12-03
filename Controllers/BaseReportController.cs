using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Reports.CustomReports.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public abstract class BaseReportController<TSearchModel, TListModel, TReportModel> : BasePluginController
        where TSearchModel : BaseSearchModel, new()
        where TListModel : BasePagedListModel<TReportModel>, new()
        where TReportModel : BaseNopModel, new()
    {
        private readonly BaseReportFactory _baseReportFactory;
        private readonly IReportsModelFactory _reportsModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly ILogger _logger;
        private readonly string _viewPath;

        protected IPermissionService PermissionService => _permissionService;
        protected IReportsModelFactory ReportsModelFactory => _reportsModelFactory;
        protected ILogger Logger => _logger;

        protected BaseReportController(
            BaseReportFactory baseReportFactory,

            IReportsModelFactory reportsModelFactory,
            IPermissionService permissionService,
            ILogger logger,
            string viewPath)
        {
            _baseReportFactory = baseReportFactory;
            _reportsModelFactory = reportsModelFactory;
            _permissionService = permissionService;
            _logger = logger;
            _viewPath = viewPath;
        }

        /// <summary>
        /// Megjeleníti a riporthoz tartozó nézetet az előkészített keresési modellel.
        /// </summary>
        /// <returns>Hozzá tartozó nézet.</returns>
        public async Task<IActionResult> ShowReport()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            // Keresési modell elkészítése
            var model = await _baseReportFactory.PrepareSearchModelAsync<TSearchModel>();

            return View(_viewPath, model);
        }

        /// <summary>
        /// A riport adatainak lekérdezéséért felelős metódus.
        /// A metódus POST kéréssel működik, ellenőrzi a felhasználói jogosultságokat,
        /// majd a megadott keresési paraméterek alapján visszaadja az adatokat JSON formátumban.
        /// </summary>
        /// <param name="searchModel">A felparaméterezett keresési modell.</param>
        /// <returns>A riport adatai JSON formátumban.</returns>
        [HttpPost]
        public async Task<IActionResult> FetchReport(TSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            var model = await _baseReportFactory.PrepareListModelAsync<TSearchModel, TListModel, TReportModel>(searchModel);

            return Json(model);
        }

        /// <summary>
        /// Visszaadja a plugin konfigurációs nézetét.
        /// </summary>
        public IActionResult Configure()
        {
            return View("~/Plugins/Reports.CustomReports/Views/Configure.cshtml");
        }
    }
}
