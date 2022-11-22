using AutAndAutV10.Models;

namespace AutAndAutV10.Services.Interfaces
{
    public interface IForgottenPasswordService
    {
        string CreateToken(string email);
        IEnumerable<ForgottenPasswordDatabaseModel>? GetForgottenPasswordByEmail(string dataEmail);
        void DeleteForgottenPasswordByEmail(string dataEmail);
        bool ValidateToken(string token, IEnumerable<ForgottenPasswordDatabaseModel> records);
    }
}
