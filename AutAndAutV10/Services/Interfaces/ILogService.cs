namespace AutAndAutV10.Services.Interfaces
{
    public interface ILogService
    {
        void Error(Exception exception, string message);
        void Info(string message);
    }
}
