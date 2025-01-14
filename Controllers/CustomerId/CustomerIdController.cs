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
        private readonly ExportReportService _exportReportService;
        private readonly INotificationService _notificationService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ITaskEmailMappingService _taskEmailMappingService;

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

        public override async Task<IActionResult> ShowReport()
        {
            // Ellenőrizzük, hogy létezik-e a ScheduledTask
            var existingTask = (await _scheduleTaskService.GetAllTasksAsync())
                .FirstOrDefault(t => t.Type == "Nop.Plugin.Reports.CustomReports.Tasks.SendEmailTask, Nop.Plugin.Reports.CustomReports");

            ViewBag.TaskExists = existingTask != null;

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

        [HttpPost, ActionName("AddScheduledTask")]
        [FormValueRequired("add-task")]
        public async Task<IActionResult> AddScheduledTask(SingleDateSearchModel searchModel)
        {
            await AddSendEmailTaskAsync();

            return RedirectToAction("ShowReport"); // vagy az adott nézet neve
        }



        [HttpGet]
        public async Task<IActionResult> GetEmailsByTaskId(int taskId)
        {
            var emails = await _taskEmailMappingService.GetEmailsByTaskIdAsync(taskId);
            return Json(emails);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmailToTask(int taskId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email cannot be empty.");

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
                return Ok(new { message = "Email added successfully." });
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync($"Error adding email: {ex.Message}");
                return StatusCode(500, "An error occurred while adding the email.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveEmailFromTask(int taskId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email cannot be empty.");

            try
            {
                await _taskEmailMappingService.RemoveEmailFromTaskAsync(taskId, email);
                return Ok(new { message = "Email removed successfully." });
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync($"Error removing email: {ex.Message}");
                return StatusCode(500, "An error occurred while removing the email.");
            }
        }

        public async Task AddSendEmailTaskAsync()
        {
            // Ellenőrizd, hogy létezik-e már a feladat
            var existingTask = (await _scheduleTaskService.GetAllTasksAsync())
                .FirstOrDefault(t => t.Type == "Nop.Plugin.Reports.CustomReports.Tasks.SendEmailTask, Nop.Plugin.Reports.CustomReports");

            if (existingTask != null)
            {
                // Ha már létezik, nem kell újra létrehozni
                await Logger.InformationAsync("The 'Send Email Task' is already registered.");
                return;
            }

            // Ha nem létezik, hozz létre egy új feladatot
            var lastEnabledUtc = DateTime.UtcNow;

            await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
            {
                Name = "Send Email Task",
                Seconds = 60, // Futtatási időköz másodpercben
                Type = "Nop.Plugin.Reports.CustomReports.Tasks.SendEmailTask, Nop.Plugin.Reports.CustomReports",
                Enabled = true,
                LastEnabledUtc = lastEnabledUtc,
                StopOnError = false
            });

            await Logger.InformationAsync("The 'Send Email Task' has been successfully added.");
        }

    }

    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(email => email)
                .NotEmpty().WithMessage("Email nem lehet üres.")
                .EmailAddress().WithMessage("Helytelen email formátum.");
        }
    }
}
