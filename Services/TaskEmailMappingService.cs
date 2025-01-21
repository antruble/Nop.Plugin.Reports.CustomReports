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

        #region Fields

        private readonly ISettingService _settingService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor
        public TaskEmailMappingService(
            ISettingService settingService,
            ILogger logger)
        {
            _settingService = settingService;
            _logger = logger;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Az adott task ID-hoz tartozó beállított email címek lekérdezése az adatbázisból.
        /// </summary>
        /// <param name="taskId">A task azonosítója, amelyhez az email címeket le szeretnénk kérdezni.</param>
        /// <returns>
        /// Egy lista az adott task ID-hoz tartozó email címekről.
        /// </returns>
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

        /// <summary>
        /// A megadott email cím hozzáadása az adott task ID-hoz tartozó emailek listájához.
        /// Ha a task ID még nem létezik, automatikusan létrehozza azt.
        /// </summary>
        /// <param name="taskId">A task azonosítója, amelyhez az email címet hozzá szeretnénk adni.</param>
        /// <param name="email">Az email cím, amelyet hozzá szeretnénk adni.</param>
        /// <exception cref="ArgumentException">A paraméterek nem megfelelőek, például az email cím üres.</exception>

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

        /// <summary>
        /// Egy email cím eltávolítása az adott task ID-hoz tartozó email listából.
        /// Ha az email cím az utolsó elem, akkor a task ID is törlésre kerül.
        /// </summary>
        /// <param name="taskId">A task azonosítója, amelyből az email címet el szeretnénk távolítani.</param>
        /// <param name="email">Az email cím, amelyet el szeretnénk távolítani.</param>
        /// <exception cref="ArgumentException">A paraméterek nem megfelelőek, például az email cím üres.</exception>

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

        #endregion

    }
}
