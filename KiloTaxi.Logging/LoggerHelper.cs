using Serilog.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Serilog.Exceptions;

namespace KiloTaxi.Logging
{
    public sealed class LoggerHelper
    {
        private static LoggerHelper _instance;
        private static readonly object _lock = new object();

        // Private constructor ensures that no other class can instantiate this directly
        private LoggerHelper() { }

        // Singleton instance accessor
        public static LoggerHelper Instance
        {
            get
            {
                // Use double-checked locking for thread safety
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LoggerHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        // Configure the logger (should be called once, on startup)
        public void ConfigureLogging(string seqUrl, string logFilePath)
        {
            //if (Log.Logger != null)
            //{
            //    return; // If logger is already configured, do nothing
            //}

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()  // Capture exception details
                .WriteTo.Console()  // Write logs to the console
                                    // Separate file for Debug logs
                .WriteTo.File(
                    path: $"{logFilePath}/debug-log-.txt",
                    rollingInterval: RollingInterval.Day,
                    //outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message:lj} (at {CallerMemberName} in {CallerFilePath}:{CallerLineNumber}) {NewLine}{Exception}",
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message:lj} {NewLine}{Exception}",
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    retainedFileCountLimit: 7)
                // Separate file for Info logs
                .WriteTo.File(
                    path: $"{logFilePath}/info-log-.txt",
                    rollingInterval: RollingInterval.Day,
                    //outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message:lj} (at {CallerMemberName} in {CallerFilePath}:{CallerLineNumber}) {NewLine}{Exception}",
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message:lj} {NewLine}{Exception}",

                    restrictedToMinimumLevel: LogEventLevel.Information,
                    retainedFileCountLimit: 7)
                // Separate file for Error logs
                .WriteTo.File(
                    path: $"{logFilePath}/error-log-.txt",
                    rollingInterval: RollingInterval.Day,
                    //outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message:lj} (at {CallerMemberName} in {CallerFilePath}:{CallerLineNumber}) {NewLine}{Exception}",
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message:lj} {NewLine}{Exception}",
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    retainedFileCountLimit: 7)
                .WriteTo.Seq(seqUrl)  // Send logs to Seq
                .CreateLogger();
        }

        // Helper method to log Debug level messages
        public void LogDebug(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            //Log.Debug("{Message} (at {MemberName} in {FilePath}:{LineNumber})", message, memberName, filePath, lineNumber);
            Log.Debug("{Message}", message);

        }

        // Helper method to log Information level messages
        public void LogInfo(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Log.Information("{Message} (at {MemberName} in {FilePath}:{LineNumber})", message, memberName, filePath, lineNumber);
            //Log.Information("{Message}", message);
        }

        // Helper method to log Warning level messages
        public void LogWarning(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            //Log.Warning("{Message} (at {MemberName} in {FilePath}:{LineNumber})", message, memberName, filePath, lineNumber);
            Log.Warning("{Message}", message);

        }

        // Helper method to log Error level messages with exception details
        public void LogError(Exception ex, string message = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            //Log.Error(ex, "{Message} (at {MemberName} in {FilePath}:{LineNumber})",
            //          message ?? ex.Message, memberName, filePath, lineNumber);
            Log.Error(ex, "{Message}",
                      message ?? ex.Message);
        }

        // Helper method to log Error level messages without exception
        public void LogError(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            //Log.Error("{Message} (at {MemberName} in {FilePath}:{LineNumber})", message, memberName, filePath, lineNumber);
            Log.Error("{Message}", message);

        }
    }
}
