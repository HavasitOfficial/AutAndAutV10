using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace AutAndAutV10.Models
{
    [TableName("ForgottenPasswordToken")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class ForgottenPasswordDatabaseModel
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }
        [Column("Email")]
        public string? Email { get; set; }
        [Column("ForgottenToken")]
        public string? ForgottenToken { get; set; }
        [Column("ExpireTime")]
        public DateTime? Expire { get; set; }
    }
}
