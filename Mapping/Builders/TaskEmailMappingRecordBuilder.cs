using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Reports.CustomReports.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Mapping.Builders
{
    public class TaskEmailMappingRecordBuilder : NopEntityBuilder<TaskEmailMappingRecord>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(TaskEmailMappingRecord.TaskId))
                    .AsInt32()
            .WithColumn(nameof(TaskEmailMappingRecord.Email))
                    .AsString(255)
            .WithColumn(nameof(TaskEmailMappingRecord.CreatedOnUtc))
                    .AsDateTime();
        }
    }
}
