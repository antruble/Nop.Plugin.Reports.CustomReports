using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Services;
using Nop.Plugin.Reports.CustomReports.Services.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Infrastructure
{
    /// <summary>
    /// Ez az osztály felelős a plugin inicializálásáért, amely során beállítja a szükséges service-eket.
    /// </summary>
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Service-ek hozzáadása és konfigurálása az alkalmazás indításakor.
        /// </summary>
        /// <param name="services">A service-ek gyűjteménye.</param>
        /// <param name="configuration">Az alkalmazás konfigurációja.</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<BaseReportFactory>();
            services.AddScoped<IReportsModelFactory, ReportsModelFactory>();
            services.AddScoped<CustomReportService>();
            services.AddScoped<ExportReportService>();
        }

        /// <summary>
        /// Az alkalmazás által használt middleware-ek konfigurálása.
        /// </summary>
        /// <param name="application">Az alkalmazás konfigurálására szolgáló objektum.</param>
        public void Configure(IApplicationBuilder application)
        {
        }


        /// <summary>
        /// Megadja az indítási konfiguráció sorrendjét.
        /// Az érték segítségével szabályozható, hogy a konfiguráció melyik fázisban történik az alkalmazás indításakor.
        /// </summary>
        public int Order => 3000;
    }
}
