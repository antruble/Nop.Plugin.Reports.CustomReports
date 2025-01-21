using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Factories;
using Nop.Plugin.Reports.CustomReports.Models.OrderSummary;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Plugin.Reports.CustomReports.Services.Export;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Reports.CustomReports.Models.CustomerId;
using Nop.Services.Logging;
using Nop.Core;
using Nop.Core.Domain.Messages;
using ClosedXML.Excel;
using System.IO;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.ScheduleTasks;
using Nop.Plugin.Reports.CustomReports.Services;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Nop.Plugin.Reports.CustomReports.Controllers.CustomerId
{
    public class CustomerIdController : BaseReportController<SingleDateSearchModel, CustomerIdListModel, CustomerIdReportModel>
    {
        #region Fields

        private readonly ExportReportService _exportReportService;
        private readonly INotificationService _notificationService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ITaskEmailMappingService _taskEmailMappingService;

        #endregion

        #region Ctor

        public CustomerIdController(
            BaseReportFactory baseReportFactory,
            IReportsModelFactory reportsModelFactory,
            IPermissionService permissionService,
            ILogger logger,

            ExportReportService exportReportService,
            INotificationService notificationService,
            IQueuedEmailService queuedEmailService,
            IEmailAccountService emailAccountService,
            IScheduleTaskService scheduleTaskService,
            ITaskEmailMappingService taskEmailMappingService
            )
            : base(baseReportFactory, reportsModelFactory, permissionService, logger, "~/Plugins/Reports.CustomReports/Views/CustomerId/CustomerId.cshtml")
        {
            _exportReportService = exportReportService;
            _notificationService = notificationService;
            _queuedEmailService = queuedEmailService;
            _emailAccountService = emailAccountService;
            _scheduleTaskService = scheduleTaskService;
            _taskEmailMappingService = taskEmailMappingService;
        }

        #endregion

        #region Methods

        public override async Task<IActionResult> ShowReport()
        {
            // Ellenőrizzük, hogy létezik-e a ScheduledTask
            var isTaskExist = (await _scheduleTaskService.GetAllTasksAsync())
                .Any(t => t.Type == "Nop.Plugin.Reports.CustomReports.Tasks.RegisterEmailTask, Nop.Plugin.Reports.CustomReports");

            ViewBag.TaskExists = isTaskExist;

            // Hívjuk az alapmetódust a model elkészítéséhez
            return await base.ShowReport();
        }

        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        public async Task<IActionResult> ExportExcelAll(SingleDateSearchModel searchModel)
        {
            if (!await base.PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var data = await base.ReportsModelFactory.FetchCustomerIdDataAsync(searchModel);

            try
            {
                var bytes = await _exportReportService.ExportCustomerIdToXlsAsync(data);
                return File(bytes, MimeTypes.TextXlsx, "customerid.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        /// <summary>
        /// Visszaadja az adott feladathoz tartozó összes email címet.
        /// </summary>
        /// <param name="taskId">A feladat azonosítója, amelyhez tartozó emaileket le kell kérni.</param>
        /// <returns>Az email címek listája JSON formátumban.</returns>
        [HttpGet]
        public async Task<IActionResult> GetEmailsByTaskId(int taskId)
        {
            var emails = await _taskEmailMappingService.GetEmailsByTaskIdAsync(taskId);
            return Json(emails);
        }

        /// <summary>
        /// Hozzárendeli az adott email címet az adott feladathoz.
        /// </summary>
        /// <param name="taskId">A feladat azonosítója.</param>
        /// <param name="email">Az email cím, amelyet hozzá kell rendelni.</param>
        /// <returns>HTTP válasz a művelet eredményével.</returns>
        [HttpPost]
        public async Task<IActionResult> AddEmailToTask(int taskId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email cím nem lehet üres!");

            var validator = new EmailValidator();
            var validationResult = validator.Validate(email);

            if (!validationResult.IsValid)
            {
                // Az első hibaüzenet visszaadása
                return BadRequest(validationResult.Errors[0].ErrorMessage);
            }

            try
            {
                await _taskEmailMappingService.AddEmailToTaskAsync(taskId, email);
                return Ok(new { message = "Email cím sikeresen hozzáadva!" });
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync($"Hiba az email hozzáadása közben: {ex.Message}");
                return StatusCode(500, "Hiba történt az email hozzáadása közben.");
            }
        }

        /// <summary>
        /// Eltávolítja a megadott emailt a feladathoz hozzárendelt emailek listájából.
        /// </summary>
        /// <param name="taskId">A feladat azonosítója, amelyből az email cím eltávolításra kerül.</param>
        /// <param name="email">Az eltávolítandó email cím.</param>
        /// <returns>HTTP válasz a művelet eredményével.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveEmailFromTask(int taskId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email nem lehet üres!");

            try
            {
                await _taskEmailMappingService.RemoveEmailFromTaskAsync(taskId, email);
                return Ok(new { message = "Email sikeresen törölve!" });
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync($"Hiba az email törlése közben: {ex.Message}");
                return StatusCode(500, "Hiba történt az email törlése közben.");
            }
        }

        #endregion
    }

    /// <summary>
    /// Egy e-mail cím validálására szolgáló segéd osztály a FluentValidation segítségével.
    /// </summary>
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(email => email)
                .NotEmpty().WithMessage("Email nem lehet üres.") // Ha üres az email cím, akkor hibaüzenet
                .EmailAddress().WithMessage("Helytelen email formátum."); // Ha helytelen email formátum, akkor hibaüzenet
        }
    }
}
