using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Reports.CustomReports.Domain;

namespace Nop.Plugin.Reports.CustomReports.Migrations
{
    [Migration(20250117, "CreateTaskEmailMappingTable")]
    public class SchemaMigration : Migration
    {
        public override void Up()
        {
            Create.TableFor<TaskEmailMappingRecord>();
        }

        public override void Down()
        {
            Delete.Table("TaskEmailMappingRecord");
        }
    }
}
