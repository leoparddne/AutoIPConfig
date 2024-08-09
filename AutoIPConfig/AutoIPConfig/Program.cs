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

            //���ݲ���ϵͳ���ͼ���
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
