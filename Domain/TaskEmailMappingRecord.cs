using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Domain
{
    public class TaskEmailMappingRecord : BaseEntity
    {
        /// <summary>
        /// Az ütemezett feladat azonosítója (ScheduleTask.Id).
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Az email cím, amelyre a riportot el kell küldeni.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Az email cím hozzáadásának dátuma és időpontja.
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
