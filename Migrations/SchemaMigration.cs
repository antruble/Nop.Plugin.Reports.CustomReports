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
    [Migration(202501070930)]
    public class SchemaMigration : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.TableFor<TaskEmailMappingRecord>();
        }
    }
}
