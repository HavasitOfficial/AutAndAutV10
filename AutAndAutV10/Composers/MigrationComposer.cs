using AutAndAutV10.Migrations;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace AutAndAutV10.Composers
{
    public class MigrationComposer : IComponent
    {
        private readonly IScopeProvider scopeProvider;
        private readonly IKeyValueService keyValueService;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;

        public MigrationComposer(IScopeProvider scopeProvider, IKeyValueService keyValueService, IMigrationPlanExecutor migrationPlanExecutor)
        {
            this.scopeProvider = scopeProvider;
            this.keyValueService = keyValueService;
            _migrationPlanExecutor = migrationPlanExecutor;
        }

        public void Initialize()
        {
            new Upgrader(new ForgottenPasswordMigration()).Execute(_migrationPlanExecutor, scopeProvider, keyValueService);
        }

        public void Terminate()
        {
            throw new System.NotImplementedException();
        }
    }
}
