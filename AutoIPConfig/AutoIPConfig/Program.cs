namespace AutoIPConfig
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });

            //根据操作系统类型加载
            if (System.OperatingSystem.IsWindows())
            {
                host.UseWindowsService();
            }
            if (System.OperatingSystem.IsLinux())
            {
                host.UseSystemd();
            }

            return host;
        }
    }
}
