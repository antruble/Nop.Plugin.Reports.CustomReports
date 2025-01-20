using Nop.Data;
using Nop.Plugin.Reports.CustomReports.Domain;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Services
{
    /// <summary>
    /// Egy olyan service, amivel kezelhetjük az adott riporthoz hozzárendelt email címeket, amikre elküldi a rendszer a beállított riportot.
    /// </summary>
    public interface ITaskEmailMappingService
    {
        /// <summary>
        /// Email címek lekérdezése egy adott feladat azonosító alapján.
        /// </summary>
        /// <param name="taskId">Feladat azonosító.</param>
        /// <returns>A társított email címek listája.</returns>
        Task<IList<string>> GetEmailsByTaskIdAsync(int taskId);

        /// <summary>
        /// Email cím hozzáadása egy feladathoz.
        /// </summary>
        /// <param name="taskId">Feladat azonosító.</param>
        /// <param name="email">Email cím.</param>
        Task AddEmailToTaskAsync(int taskId, string email);

        /// <summary>
        /// Email cím eltávolítása egy feladathoz társított címek közül.
        /// </summary>
        /// <param name="taskId">Feladat azonosító.</param>
        /// <param name="email">Email cím.</param>
        Task RemoveEmailFromTaskAsync(int taskId, string email);
    }

    public class TaskEmailMappingService : ITaskEmailMappingService
    {
        private readonly IRepository<TaskEmailMappingRecord> _taskEmailMappingRepository;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;

        public TaskEmailMappingService(
            IRepository<TaskEmailMappingRecord> taskEmailMappingRepository, 
            ISettingService settingService,
            ILogger logger)
        {
            _settingService = settingService;
            _taskEmailMappingRepository = taskEmailMappingRepository;
            _logger = logger;
        }

        public async Task<IList<string>> GetEmailsByTaskIdAsync(int taskId)
        {
            try
            {
                // Ellenőrizd a ReportEmails beállítást
                var reportEmailsKey = CustomReports.GetReportEmailSchedulerSettingsKeyAsync();
                var existingSetting = await _settingService.GetSettingByKeyAsync<string>(reportEmailsKey) ?? "{}";

                // Parse the existing JSON
                var emailMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(existingSetting) ?? new Dictionary<int, string>();

                // Ellenőrizd, hogy a taskId létezik-e
                if (emailMapping.TryGetValue(taskId, out var emails))
                {
                    return emails.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                return new List<string>();
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error while retrieving emails for task ID {taskId}: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task AddEmailToTaskAsync(int taskId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("The email cannot be empty.", nameof(email));

            try
            {
                // Ellenőrizd a ReportEmails beállítást
                var reportEmailsKey = CustomReports.GetReportEmailSchedulerSettingsKeyAsync();
                var existingSetting = await _settingService.GetSettingByKeyAsync<string>(reportEmailsKey) ?? "{}";

                // Parse the existing JSON
                var emailMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(existingSetting) ?? new Dictionary<int, string>();

                // Frissítsd az adott taskId-hez tartozó email-eket
                if (emailMapping.ContainsKey(taskId))
                {
                    emailMapping[taskId] += $"{email};";
                }
                else
                {
                    emailMapping[taskId] = $"{email};";
                }

                // Mentsd vissza a frissített JSON-t
                var updatedJson = Newtonsoft.Json.JsonConvert.SerializeObject(emailMapping);
                await _settingService.SetSettingAsync(reportEmailsKey, updatedJson);

                await _logger.InformationAsync($"Email '{email}' added to task ID {taskId}.");
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error while adding email '{email}' to task ID {taskId}: {ex.Message}");
            }
        }

        public async Task RemoveEmailFromTaskAsync(int taskId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("The email cannot be empty.", nameof(email));

            try
            {
                // Ellenőrizd a ReportEmails beállítást
                var reportEmailsKey = CustomReports.GetReportEmailSchedulerSettingsKeyAsync();
                var existingSetting = await _settingService.GetSettingByKeyAsync<string>(reportEmailsKey) ?? "{}";

                // Parse the existing JSON
                var emailMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(existingSetting) ?? new Dictionary<int, string>();

                // Ellenőrizd, hogy a taskId létezik-e
                if (emailMapping.ContainsKey(taskId))
                {
                    // Távolítsd el az emailt a listából
                    var emails = emailMapping[taskId].Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (emails.Remove(email))
                    {
                        // Ha maradtak email-ek, frissítsd az értéket, különben töröld a kulcsot
                        if (emails.Any())
                        {
                            emailMapping[taskId] = string.Join(';', emails) + ";";
                        }
                        else
                        {
                            emailMapping.Remove(taskId);
                        }

                        // Mentsd vissza a frissített JSON-t
                        var updatedJson = Newtonsoft.Json.JsonConvert.SerializeObject(emailMapping);
                        await _settingService.SetSettingAsync(reportEmailsKey, updatedJson);

                        await _logger.InformationAsync($"Email '{email}' removed from task ID {taskId}.");
                    }
                    else
                    {
                        await _logger.WarningAsync($"Email '{email}' not found for task ID {taskId}.");
                    }
                }
                else
                {
                    await _logger.WarningAsync($"Task ID {taskId} not found in ReportEmails.");
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error while removing email '{email}' from task ID {taskId}: {ex.Message}");
            }
        }

        
    }
}
