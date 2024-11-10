using KiloTaxi.Logging;

namespace KiloTaxi.Realtime.Helper.ServiceExtensions
{
    public class ConfigHelper
    {
        public static void ConfigureService(WebApplicationBuilder builder)
        {
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            // Configure logging(write to both file and Seq)
            LoggerHelper.Instance.ConfigureLogging("http://localhost:5341", logFilePath);
            
        }
    }
}
