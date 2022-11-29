using AutAndAutV10.Models;
using AutAndAutV10.Services.Interfaces;
using Umbraco.Cms.Core.Scoping;

namespace AutAndAutV10.Services
{
    public class ForgottenPasswordService : IForgottenPasswordService
    {
        private readonly IScopeProvider _scopeProvider;

        private const double EXPIRY_DURATION_MINUTES = 10080;

        public ForgottenPasswordService(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public void DeleteForgottenPasswordByEmail(string dataEmail)
        {
            if (string.IsNullOrEmpty(dataEmail))
            {
                return;
            }

            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            db.Delete<ForgottenPasswordDatabaseModel>("WHERE [Email] = @dataEmail", new { dataEmail });
            scope.Complete();
        }

        public string CreateToken(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return string.Empty;
            }

            using var scope = _scopeProvider.CreateScope();
            var token = Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "");
            var record = new ForgottenPasswordDatabaseModel()
            {
                Email = email,
                ForgottenToken = token,
                Expire = DateTime.Now.AddMinutes(EXPIRY_DURATION_MINUTES)
            };

            scope.Database.Insert(record);
            scope.Complete();

            return token;
        }

        public IEnumerable<ForgottenPasswordDatabaseModel> GetForgottenPasswordByEmail(string dataEmail)
        {
            if (string.IsNullOrEmpty(dataEmail))
            {
                return null;
            }

            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            return db.Query<ForgottenPasswordDatabaseModel>(
                "SELECT * FROM ForgottenPasswordToken WHERE [EMAIL]= @dataEmail",
                new { dataEmail });
        }

        public bool ValidateToken(string token, IEnumerable<ForgottenPasswordDatabaseModel> records)
        {
            if (records == null || !records.Any() || string.IsNullOrEmpty(token))
            {
                return false;
            }
            var lastRecord = records
                .Where(x => x.Expire != null && x.ForgottenToken != null)
                .OrderByDescending(x => x.Expire)
                .FirstOrDefault();
            if (lastRecord != null && lastRecord.ForgottenToken == token && lastRecord.Expire > DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
