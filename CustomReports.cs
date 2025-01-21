using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nop.Services.Logging;
using System.Runtime.CompilerServices;
using Nop.Services.Localization;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using System;
using Nop.Services.Messages;
using Nop.Core.Domain.Messages;
using Nop.Services.Configuration;

namespace Nop.Plugin.Reports.CustomReports
{
    /// <summary>
    /// Ez az oszt�ly felel�s a plugin telep�t�s��rt, elt�vol�t�s��rt �s a konfigur�l�s��rt.
    /// </summary>
    public class CustomReports : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CustomReports(
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            ILogger logger,
            ISettingService settingService,
            IWebHelper webHelper 
            ) 
        {
            _permissionService = permissionService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _logger = logger;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// A plugin telep�t�se. Ezt a met�dust h�vja meg az alkalmaz�s, amikor a plugint telep�ti.
        /// </summary>
        public override async Task InstallAsync()
        {
            await base.InstallAsync();

            // Ellenőrzés, hogy létezik-e már az üzenet sablon, ha nem létrehozzuk
            var messageTemplate = await _messageTemplateService.GetMessageTemplatesByNameAsync("Reports.ExcelEmail");
            if (messageTemplate == null || messageTemplate.Count == 0)
            {
                // Új sablon létrehozása
                var newTemplate = new MessageTemplate
                {
                    Name = "Reports.ExcelEmail",
                    Subject = "Riport excel: %ReportName% (%EndDate%)",
                    Body = "<p>A mellékletben található a riport az alábbi intervallum adatait tartalmazza: %StartDate% - %EndDate%.</p>",
                    IsActive = true,
                    EmailAccountId = 1,
                    LimitedToStores = false
                };
                await _messageTemplateService.InsertMessageTemplateAsync(newTemplate);
            }

            // Ellenőrzés, hogy létezik-e már riporthoz tartozó kulcs a Settings táblában
            var reportEmailsKey = GetReportEmailSchedulerSettingsKeyAsync();
            var existingSetting = await _settingService.GetSettingByKeyAsync<string>(reportEmailsKey);

            if (string.IsNullOrEmpty(existingSetting))
            {
                // Új beállítás létrehozása üres JSON értékkel
                var emptyJsonValue = "{}";
                await _settingService.SetSettingAsync(reportEmailsKey, emptyJsonValue);
            }

        }

        /// <summary>
        /// A plugin elt�vol�t�sa. Ezt a met�dust h�vja meg az alkalmaz�s, amikor a plugint elt�vol�tja.
        /// </summary>
        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        /// <summary>
        /// Visszaadja a plugin konfigur�ci�s oldal�nak URL-j�t.
        /// </summary>
        /// <returns>A konfigur�ci�s oldal URL-je.</returns>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/CustomReports/Configure";
        }

        /// <summary>
        /// Kezeli az admin oldali menü struktúráját.
        /// </summary>
        /// <param name="rootNode">Az admin oldal főmenüjének gyökér node-ja.</param>
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return;

            // Megkeresi a "Reports" menüt
            var reportsMenu = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Reports"));
            if (reportsMenu == null)
                return;

            // Riportcsoportok és önálló riportok hozzáadása
            var reportsMenuItems = await GetReportsMenuItemsAsync();
            foreach (var item in reportsMenuItems)
            {
                // Csoportos riport menü, vagy egyedüli report menü hozzáadása a "Reports" menühöz
                if (item.IsGroup)
                    AddReportGroup(reportsMenu, item.SystemName, item.Title, item.IconClass, item.ChildReports);
                else
                    AddSingleReport(reportsMenu, item.SystemName, item.Title, item.IconClass, item.ControllerName);
            }

            // Az alapértelmezett "Bestsellers" riport controllerének átállítása a plugin által definiált "Bestseller" controller-re
            await BiasDefaultNopReport(reportsMenu, "Bestsellers", "Bestseller");


        }

        /// <summary>
        /// Egy riportcsoport hozzáadása a menühöz.
        /// </summary>
        /// <param name="parentNode">A szülő node, amelyhez a csoportot hozzáadjuk.</param>
        /// <param name="systemName">A csoport egyedi rendszer neve.</param>
        /// <param name="title">A csoport megjelenítendő címe.</param>
        /// <param name="iconClass">A csoport ikon CSS osztálya.</param>
        /// <param name="childReports">A csoporthoz tartozó riportok.</param>
        private void AddReportGroup(SiteMapNode parentNode, string systemName, string title, string iconClass, dynamic[] childReports)
        {
            var groupMenuItem = new SiteMapNode
            {
                SystemName = systemName,
                Title = title,
                IconClass = iconClass,
                Visible = true,
                RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } },
                ChildNodes = new List<SiteMapNode>()
            };

            foreach (var childReport in childReports)
            {
                groupMenuItem.ChildNodes.Add(new SiteMapNode
                {
                    SystemName = childReport.SystemName,
                    Title = childReport.Title,
                    IconClass = childReport.IconClass,
                    ControllerName = childReport.ControllerName,
                    ActionName = "ShowReport",
                    Visible = true,
                    RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
                });
            }

            parentNode.ChildNodes.Add(groupMenuItem);
        }

        /// <summary>
        /// Egy önálló riport hozzáadása a menühöz.
        /// </summary>
        /// <param name="parentNode">A szülő csomópont, amelyhez a riportot hozzáadjuk.</param>
        /// <param name="systemName">A riport egyedi rendszer neve.</param>
        /// <param name="title">A riport megjelenítendő címe.</param>
        /// <param name="iconClass">A riport ikon CSS osztálya.</param>
        /// <param name="controllerName">A riport controller neve.</param>
        private void AddSingleReport(SiteMapNode parentNode, string systemName, string title, string iconClass, string controllerName)
        {
            parentNode.ChildNodes.Add(new SiteMapNode
            {
                SystemName = systemName,
                Title = title,
                IconClass = iconClass,
                ControllerName = controllerName,
                ActionName = "ShowReport",
                Visible = true,
                RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
            });
        }

        /// <summary>
        /// Visszaadja a konfigurált riportcsoportokat és riportokat.
        /// </summary>
        /// <returns>A riportokat tartalmazó lista.</returns>
        private async Task<List<dynamic>> GetReportsMenuItemsAsync()
        {
            return new List<dynamic>
            {
                new
                {
                    IsGroup = true,
                    SystemName = "Reports.CustomerReports",
                    Title = await _localizationService.GetResourceAsync("Admin.Reports.CustomerServiceReports.Title"),
                    IconClass = "far fa-dot-circle",
                    ChildReports = new[]
                    {
                        new { SystemName = "Reports.CustomerReports.DailyOrders", Title = await _localizationService.GetResourceAsync("Admin.Reports.CustomerServiceReports.DailyOrders.Title"), ControllerName = "DailyOrders", IconClass = "far fa-dot-circle" },
                        new { SystemName = "Reports.CustomerReports.Discounts", Title = await _localizationService.GetResourceAsync("Admin.Reports.CustomerServiceReports.Discounts.Title"), ControllerName = "Discounts", IconClass = "far fa-dot-circle" },
                        new { SystemName = "Reports.CustomerReports.RegisteredCustomers", Title = await _localizationService.GetResourceAsync("Admin.Reports.CustomerServiceReports.RegisteredCustomers.Title"), ControllerName = "RegisteredCustomers", IconClass = "far fa-dot-circle" },
                        new { SystemName = "Reports.CustomerReports.ShoperiaPlusSubscriptions", Title = await _localizationService.GetResourceAsync("Admin.Reports.CustomerServiceReports.ShoperiaPlusSubscriptions.Title"), ControllerName = "ShoperiaPlusSubscriptions", IconClass = "far fa-dot-circle" },
                        new { SystemName = "Reports.CustomerReports.ReturnedOrders", Title = await _localizationService.GetResourceAsync("Admin.Reports.CustomerServiceReports.ReturnedOrders.Title"), ControllerName = "ReturnedOrders", IconClass = "far fa-dot-circle" }
                    }
                },
                new
                {
                    IsGroup = false,
                    SystemName = "Reports.OrderDetails",
                    Title = await _localizationService.GetResourceAsync("Admin.Reports.OrderDetails.Title"),
                    IconClass = "far fa-dot-circle",
                    ControllerName = "OrderDetails"
                },
                new
                {
                    IsGroup = false,
                    SystemName = "Reports.OrderSummary",
                    Title = await _localizationService.GetResourceAsync("Admin.Reports.OrderSummary.Title"),
                    IconClass = "far fa-dot-circle",
                    ControllerName = "OrderSummary"
                },
                //new
                //{
                //    IsGroup = false,
                //    SystemName = "Reports.PromotionSummary",
                //    Title = await _localizationService.GetResourceAsync("Admin.Reports.PromotionSummary.Title"),
                //    IconClass = "far fa-dot-circle",
                //    ControllerName = "PromotionSummary"
                //},
                new
                {
                    IsGroup = false,
                    SystemName = "Reports.CustomerId",
                    Title = await _localizationService.GetResourceAsync("Admin.Reports.CustomerId.Title"),
                    IconClass = "far fa-dot-circle",
                    ControllerName = "CustomerId"
                },
                new
                {
                    IsGroup = true,
                    SystemName = "Reports.Problemasak",
                    Title = await _localizationService.GetResourceAsync("Admin.Reports.Problemasak.Title"),
                    IconClass = "far fa-dot-circle",
                    ChildReports = new[]
                    {
                        new { SystemName = "Reports.Problemasak.ProblemasManufacturer", Title = await _localizationService.GetResourceAsync("Admin.Reports.Problemasak.Markak.Title"), ControllerName = "ProblemasManufacturer", IconClass = "far fa-dot-circle" },
                        new { SystemName = "Reports.Problemasak.ProblemasOrder", Title = await _localizationService.GetResourceAsync("Admin.Reports.Problemasak.Rendelesek.Title"), ControllerName = "ProblemasOrder", IconClass = "far fa-dot-circle" },
                        new { SystemName = "Reports.Problemasak.ProblemasProduct", Title = await _localizationService.GetResourceAsync("Admin.Reports.Problemasak.Termekek.Title"), ControllerName = "ProblemasProduct", IconClass = "far fa-dot-circle" },
                    }
                },

                // További riportok hozzáadása itt...
            };
        }

        /// <summary>
        /// Egy alapértelmezett riport átirányítása egy, a pluginban definiált controllerre.
        /// </summary>
        /// <param name="reportMenuNode">A "Reports" menü csomópontja.</param>
        /// <param name="nopReportName">Az alapértelmezett riport neve.</param>
        /// <param name="newReportControllerName">A pluginban szereplő új controller neve.</param>
        private async Task BiasDefaultNopReport(SiteMapNode reportMenuNode, string nopReportName, string newReportControllerName) 
        {
            var nopReport = reportMenuNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals(nopReportName));
            if (nopReport == null)
            {
                await _logger.ErrorAsync($"Hiba a BiasDefaultNopReport metódusban: az átirányítani kivánt {nopReportName} nevű alapértelemzett riport nem található!");
                return;
            }
            nopReport.ControllerName = newReportControllerName;
            nopReport.ActionName = "ShowReport";
        }
        public static string GetReportEmailSchedulerSettingsKeyAsync()
        {
            return "ReportEmails";
        }
        #endregion
    }
}
