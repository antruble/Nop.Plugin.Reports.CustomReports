using Nop.Web.Framework.Models;

namespace Nop.Plugin.Reports.CustomReports.Models.Problemasak.ProblemasProduct
{
    /// <summary>
    /// Represents a low stock product list model
    /// </summary>
    public partial record ProblemasProductListModel : BasePagedListModel<ProblemasProductReportModel>
    {
    }
}