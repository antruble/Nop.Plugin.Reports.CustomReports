using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var logger = endpointRouteBuilder.ServiceProvider.GetRequiredService<ILogger<RouteProvider>>();
            logger.LogInformation("Regisztrálva: ExtendedBestsellersReport");

            endpointRouteBuilder.MapControllerRoute(
                name: "TestExtendedReport",
                pattern: "Admin/Test/ExtendedReport",
                defaults: new { controller = "ExtendedReport", action = "Bestsellers", area = "Admin" });


            // Felülírjuk az eredeti útvonalat az új controller-re
            endpointRouteBuilder.MapControllerRoute(
                name: "ExtendedBestsellersReport",
                pattern: "Admin/Report/Bestsellers",
                defaults: new { controller = "ExtendedReport", action = "Bestsellers", area = "Admin" });

            // Új Configure útvonal
            endpointRouteBuilder.MapControllerRoute(
                name: "ConfigureCustomReports",
                pattern: "Admin/CustomReports/Configure",
                defaults: new { controller = "ReportHelper", action = "Configure", area = "Admin" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1;
    }
}
