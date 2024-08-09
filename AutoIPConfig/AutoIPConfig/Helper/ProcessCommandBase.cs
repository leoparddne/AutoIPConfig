using System.Diagnostics;
using System.Text;

namespace AutoIPConfig.Helper
{
    public class ProcessCommandBase : IDisposable
    {
        //程序名
        public string programe;
        //参数
        StringBuilder parameter = new StringBuilder();
        Process process = null;

        public ProcessCommandBase(string programe)
        {
            this.programe = programe;
        }

        public ProcessCommandBase AddParameter(string para)
        {
            parameter.Append($" {para} ");

            return this;
        }

        public string Exec(bool waitForExit = false)
        {
            //var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            process = new Process();
            process.StartInfo.FileName = programe;
            process.StartInfo.Arguments = parameter.ToString();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            //重定向标准输输出、标准错误流
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Exited += Process_Exited;
            process.OutputDataReceived += Process_OutputDataReceived;
            Console.WriteLine($"Exe:{process.StartInfo.FileName}");
            Console.WriteLine($"Parameter:{process.StartInfo.Arguments}");
            process.Start();
            process.BeginErrorReadLine();
            //process.BeginOutputReadLine();
            if (waitForExit)
            {
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return result;
            }

            return string.Empty;
        }

        public void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data ?? string.Empty);
        }

        public void Process_Exited(object sender, EventArgs e)
        {
        }

        public void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data ?? string.Empty);
        }

        public void ClearParameter()
        {
            parameter.Clear();
        }

        public void Close()
        {
            process?.Close();
            process = null;
        }

        public void Kill()
        {
            process?.Kill();
            process?.Close();
            process = null;
        }

        public void Dispose()
        {
            Kill();
        }
    }
}
