using Nop.Core.Domain.Messages;
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
    public class SendEmailTask : IScheduleTask
    {
        private readonly ExportReportService _exportReportService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IReportsModelFactory _reportsModelFactory;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly INotificationService _notificationService;
        private readonly ITaskEmailMappingService _taskEmailMappingService;
        private readonly ILogger _logger;

        public SendEmailTask(
            ExportReportService exportReportService,
            EmailAccountSettings emailAccountSettings,
            IQueuedEmailService queuedEmailService, 
            IEmailAccountService emailAccountService, 
            IReportsModelFactory reportsModelFactory,
            INotificationService notificationService,
            ITaskEmailMappingService taskEmailMappingService,
            ILogger logger)
        {
            _emailAccountSettings = emailAccountSettings;
            _exportReportService = exportReportService;
            _reportsModelFactory = reportsModelFactory;
            _queuedEmailService = queuedEmailService;
            _emailAccountService = emailAccountService;
            _notificationService = notificationService;
            _taskEmailMappingService = taskEmailMappingService;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                // Excel generálása
                string filePath = string.Empty;
                try
                {
                    var excelData = await _reportsModelFactory.FetchCustomerIdDataAsync(new SingleDateSearchModel());
                    var bytes = await _exportReportService.ExportCustomerIdToXlsAsync(excelData);

                    filePath = Path.Combine(Path.GetTempPath(), "CustomerIdReport.xlsx");

                    await System.IO.File.WriteAllBytesAsync(filePath, bytes);
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync($"Hiba az Excel generálása során: {ex.Message}");
                    return;
                }

                // Email címek lekérdezése
                var emailRecords = await _taskEmailMappingService.GetEmailsByTaskIdAsync(1);

                if (!emailRecords.Any())
                {
                    await _logger.WarningAsync("Nincsenek email címek az adott TaskId-hoz.");
                    return;
                }

                var defaultEmailAccount = await _emailAccountService.GetEmailAccountByIdAsync(3);
                //var defaultEmailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId);

                if (defaultEmailAccount == null)
                {
                    await _logger.ErrorAsync("Alapértelmezett email fiók nem található.");
                    return;
                }

                // Email küldése minden címzettnek
                foreach (var record in emailRecords)
                {
                    var email = new QueuedEmail
                    {
                        Priority = QueuedEmailPriority.High,
                        From = defaultEmailAccount.Email,
                        FromName = defaultEmailAccount.DisplayName,
                        To = record.Email,
                        Subject = "Customer ID report",
                        Body = $"{DateTime.UtcNow.AddDays(-1)}. Customer ID riport",
                        CreatedOnUtc = DateTime.UtcNow,
                        EmailAccountId = defaultEmailAccount.Id,
                        AttachmentFilePath = filePath
                    };

                    try
                    {
                        await _queuedEmailService.InsertQueuedEmailAsync(email);
                        await _logger.InformationAsync($"Email küldés a {record.Email} címre sikeresen rögzítve.");
                    }
                    catch (Exception ex)
                    {
                        await _logger.ErrorAsync($"Hiba az email küldése során a {record.Email} címre: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Hiba a SendEmailTask végrehajtása során: {ex.Message}");
            }
        }

    }
}
