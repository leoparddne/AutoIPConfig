using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoIPConfig.Helper
{
    public class LogHelperEx
    {
        public static bool ISDebug = true;
        public static void WriteLog(string file, string msg)
        {
            //var x = Task.CurrentId;
            var finalMsg = $"{file},ThreadID:{Thread.GetCurrentProcessorId()},{msg}";
            Console.WriteLine(finalMsg);
            LogHelper.WriteLog(file, new string[] { finalMsg });
        }

        public static void Debug(string msg)
        {
            //var x = Task.CurrentId;
            var finalMsg = $"ThreadID:{Thread.GetCurrentProcessorId()},{msg}";
            if (ISDebug)
            {
                Console.WriteLine(finalMsg);
            }
            Helper.LogHelper.Debug(finalMsg);
        }
    }
}
