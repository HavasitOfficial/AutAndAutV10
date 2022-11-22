using AutAndAutV10.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace AutAndAutV10.Services
{
    public class LogService : ILogService
    {
        private readonly ILogger _logger;
        public LogService(ILogger logger)
        {
            _logger = logger;
        }
        public void Error(Exception exception, string message)
        {
            _logger.Error(exception,$" Message : { message}");
        }

        public void Info(string message)
        {
            _logger.Information($"Message : {message}");
        }
    }
}
