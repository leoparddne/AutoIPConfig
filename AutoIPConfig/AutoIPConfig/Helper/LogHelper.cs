using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace AutoIPConfig.Helper
{
    public static class LogHelper
    {
        private static ILogger log;

        private static readonly object _lock;

        private static LoggerConfiguration GenLog(this LoggerConfiguration configuration, LogEventLevel logEventLevel, int retainedFileCount = 0, long fileSizeBytes = 5242880L)
        {
            string SerilogOutputTemplate = "{NewLine}{NewLine}Date：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}LogLevel：{Level}{NewLine}Message：{Message}{NewLine}{Exception}" + new string('-', 50);
            configuration.WriteTo.Logger(delegate (LoggerConfiguration lg)
            {
                LoggerSinkConfiguration writeTo = lg.Filter.ByIncludingOnly((LogEvent p) => p.Level == logEventLevel).WriteTo;
                string path = LogFilePath(logEventLevel);
                string outputTemplate = SerilogOutputTemplate;
                long? fileSizeLimitBytes = fileSizeBytes;
                int? retainedFileCountLimit = ((retainedFileCount == 0) ? null : new int?(retainedFileCount));
                writeTo.File(path, LogEventLevel.Verbose, outputTemplate, null, fileSizeLimitBytes, null, buffered: false, shared: true, null, RollingInterval.Day, rollOnFileSizeLimit: true, retainedFileCountLimit);
            });
            return configuration;
            static string LogFilePath(LogEventLevel LogEvent)
            {
                return $"{AppContext.BaseDirectory}00_Logs/{LogEvent}/log.log";
            }
        }

        static LogHelper()
        {
            _lock = new object();
            int.TryParse(AppSettingsHelper.GetSetting("LogSetting", "RetainedFileCount"), out var result);
            if (result == 0)
            {
                result = 10;
            }

            long.TryParse(AppSettingsHelper.GetSetting("LogSetting", "FileSizeLimitBytes"), out var result2);
            result2 = ((result2 == 0L) ? 5242880 : result2);
            log = (Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().Enrich.FromLogContext().GenLog(LogEventLevel.Debug, result, result2).GenLog(LogEventLevel.Information, result, result2)
                .GenLog(LogEventLevel.Warning, result, result2)
                .GenLog(LogEventLevel.Error, result, result2)
                .GenLog(LogEventLevel.Fatal, result, result2)
                .CreateLogger());
        }

        public static void Info(string data)
        {
            log.Information("{data}", data);
        }

        public static void Debug(string data)
        {
            log.Debug("{data}", data);
        }

        public static void Error(string data)
        {
            log.Error("{data}", data);
        }

        public static void Warning(string data)
        {
            log.Warning("{data}", data);
        }

        public static void Fatal(string data)
        {
            log.Fatal("{data}", data);
        }

        public static void WriteLog(string fileName, string[] datas, string defaultFolder = "")
        {
            string logContent = string.Join("\r\n", datas);
            WriteLog(fileName, logContent, defaultFolder);
        }

        public static void WriteLog(string fileName, string logContent, string defaultFolder = "")
        {
            int result = 0;
            int.TryParse(AppSettingsHelper.GetSetting("LogSetting", "RetainedFileCount"), out result);
            if (result == 0)
            {
                result = 10;
            }

            long.TryParse(AppSettingsHelper.GetSetting("LogSetting", "FileSizeLimitBytes"), out var result2);
            result2 = ((result2 == 0L) ? 5242880 : result2);
            string path = Path.Combine(AppContext.BaseDirectory, "Log");
            lock (_lock)
            {
                Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File(Path.Combine(path, "", fileName + ".log"), LogEventLevel.Verbose, "{Message}{NewLine}{Exception}", null, retainedFileCountLimit: (result == 0) ? null : new int?(result), fileSizeLimitBytes: result2, levelSwitch: null, buffered: false, shared: true, flushToDiskInterval: null, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true).CreateLogger();
                Log.Information(DateTime.Now.ToString("HH:mm:ss.ffffff") + ": " + logContent);
                Log.CloseAndFlush();
            }
        }
    }
}