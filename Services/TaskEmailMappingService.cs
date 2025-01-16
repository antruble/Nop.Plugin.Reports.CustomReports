using Nop.Data;
using Nop.Plugin.Reports.CustomReports.Domain;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Services
{
    /// <summary>
    /// Service a riportokhoz tartozó scheduled task email adataihoz.
    /// </summary>
    public interface ITaskEmailMappingService
    {
        /// <summary>
        /// Email címek lekérdezése egy adott feladat azonosító alapján.
        /// </summary>
        /// <param name="taskId">Feladat azonosító.</param>
        /// <returns>A társított email címek listája.</returns>
        Task<IList<TaskEmailMappingRecord>> GetEmailsByTaskIdAsync(int taskId);

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
        private readonly ILogger _logger;

        public TaskEmailMappingService(IRepository<TaskEmailMappingRecord> taskEmailMappingRepository, ILogger logger)
        {
            _taskEmailMappingRepository = taskEmailMappingRepository;
            _logger = logger;
        }

        public async Task<IList<TaskEmailMappingRecord>> GetEmailsByTaskIdAsync(int taskId)
        {
            var emails = await _taskEmailMappingRepository.Table
                .Where(x => x.TaskId == taskId)
                .ToListAsync(); ;

            if (!emails.Any())
            {
                await _logger.WarningAsync($"No emails found for task ID {taskId}.");
                return new List<TaskEmailMappingRecord>();
            }

            return emails;
        }

        public async Task AddEmailToTaskAsync(int taskId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("The email cannot be empty.", nameof(email));

            var record = new TaskEmailMappingRecord
            {
                TaskId = taskId,
                Email = email,
                CreatedOnUtc = DateTime.UtcNow
            };

            try
            {
                await _taskEmailMappingRepository.InsertAsync(record);
                await _logger.InformationAsync($"Email '{email}' added to task ID {taskId}.");
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error while adding email '{email}' to task ID {taskId}: {ex.Message}");
            }
        }

        public async Task RemoveEmailFromTaskAsync(int taskId, string email)
        {
            var record = await _taskEmailMappingRepository.Table
                .FirstOrDefaultAsync(x => x.TaskId == taskId && x.Email == email);

            if (record != null)
            {
                try
                {
                    await _taskEmailMappingRepository.DeleteAsync(record);
                    await _logger.InformationAsync($"Email '{email}' removed from task ID {taskId}.");
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync($"Error while removing email '{email}' from task ID {taskId}: {ex.Message}");
                    return;
                }
            }
            else
            {
                await _logger.WarningAsync($"Email '{email}' not found for task ID {taskId}.");
            }
        }
    }
}
