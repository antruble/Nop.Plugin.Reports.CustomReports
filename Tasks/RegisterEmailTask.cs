using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Plugin.Reports.CustomReports.Factories.CustomerReports;
using Nop.Plugin.Reports.CustomReports.Models.SearchModels;
using Nop.Plugin.Reports.CustomReports.Services;
using Nop.Plugin.Reports.CustomReports.Services.Export;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Tasks
{
    public class RegisterEmailTask : IScheduleTask
    {

        #region Fields

        private readonly ExportReportService _exportReportService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly INopFileProvider _fileProvider;
        private readonly IReportsModelFactory _reportsModelFactory;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly INotificationService _notificationService;
        private readonly ITaskEmailMappingService _taskEmailMappingService;
        private readonly CustomWorkflowMessageService _customWorkflowMessageService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public RegisterEmailTask(
            CustomWorkflowMessageService customWorkflowMessageService,
            ExportReportService exportReportService,
            EmailAccountSettings emailAccountSettings,
            IQueuedEmailService queuedEmailService, 
            IEmailAccountService emailAccountService, 
            IReportsModelFactory reportsModelFactory,
            INopFileProvider fileProvider,
            INotificationService notificationService,
            ITaskEmailMappingService taskEmailMappingService,
            ILogger logger)
        {
            _customWorkflowMessageService = customWorkflowMessageService;
            _fileProvider = fileProvider;
            _emailAccountSettings = emailAccountSettings;
            _exportReportService = exportReportService;
            _reportsModelFactory = reportsModelFactory;
            _queuedEmailService = queuedEmailService;
            _emailAccountService = emailAccountService;
            _notificationService = notificationService;
            _taskEmailMappingService = taskEmailMappingService;
            _logger = logger;
        }

        #endregion

        /// <summary>
        /// Feladat végrehajtásáért felelős metódus, amely egy Excel riportot generál, és elküldi azt emailben a hozzárendelt címzetteknek.
        /// </summary>
        public async Task ExecuteAsync()
        {
            try
            {
                // 1. Excel riport generálása
                byte[] fileBytes;
                string filePath = string.Empty;
                try
                {
                    // Riport adatok lekérése és exportálása Excel formátumba
                    var excelData = await _reportsModelFactory.FetchCustomerIdDataAsync(new SingleDateSearchModel { Date = new DateTime(2024, 01, 01) });
                    fileBytes = await _exportReportService.ExportCustomerIdToXlsAsync(excelData);

                    //// Fájl mentése a wwwrootba mappába
                    var folderPath = _fileProvider.MapPath("~/wwwroot/files/excels");
                    await _logger.InformationAsync($"Checking directory: {folderPath}");

                    if (!_fileProvider.DirectoryExists(folderPath))
                    {
                        await _logger.InformationAsync($"Directory does not exist. Creating: {folderPath}");
                        _fileProvider.CreateDirectory(folderPath);
                    }
                    else
                    {
                        await _logger.InformationAsync($"Directory already exists: {folderPath}");
                    }

                    filePath = _fileProvider.Combine(_fileProvider.MapPath(folderPath), "riportdata.xlsx");
                    await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync($"Hiba az Excel generálása során: {ex.Message}");
                    return;
                }
                var emails = await _taskEmailMappingService.GetEmailsByTaskIdAsync(1);
                if (emails.Any())
                {
                    foreach (var email in emails)
                    {
                        await _customWorkflowMessageService.SendCustomReportEmailAsync(email, filePath);
                    }
                }
                else 
                {
                    await _logger.WarningAsync("Nem található email a reporthoz");
                }

            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Hiba a SendEmailTask végrehajtása során: {ex.Message}");
            }
        }

    }
}
