using AutAndAutV10.Models;
using Umbraco.Cms.Infrastructure.Migrations;

namespace AutAndAutV10.Migrations
{
    public class ForgottenPasswordMigration : MigrationPlan
    {
        public ForgottenPasswordMigration() : base("ForgottenPasswordToken")
        {
            From(string.Empty)
                .To<ForgottenPasswordCreate>("init-migration")
                .To<ForgottenPasswordExpireTime>("expire-time-migration");
        }

        public class ForgottenPasswordCreate : MigrationBase
        {
            public ForgottenPasswordCreate(IMigrationContext context)
                : base(context)
            {
            }
            protected override void Migrate()
            {
                if (!TableExists("ForgottenPasswordToken"))
                {
                    Create.Table<ForgottenPasswordDatabaseModel>().Do();
                }
            }
        }

        public class ForgottenPasswordExpireTime : MigrationBase
        {
            public ForgottenPasswordExpireTime(IMigrationContext context)
                : base(context)
            {

            }

            protected override void Migrate()
            {
                if (TableExists("ForgottenPasswordToken") && !ColumnExists("ForgottenPasswordToken", "ExpireTime"))
                {
                    Create.Column("ExpireTime").OnTable("ForgottenPasswordToken").AsDateTime().Nullable().Do();
                }
            }
        }
    }
}
