using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InsertEntryPoint
{
    class shellHelper
    {
        public static StreamWriter SW_log { get; set; }
        public static void executeShellExeCmd(string exePath,string args)
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = exePath;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.Arguments = args;
                p.StartInfo.RedirectStandardInput = true;    //重定向标准输入
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;    //重定向错误输出
                //开始
                p.Start();
                p.WaitForExit();
                p.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                txtWriteHelper.AppendLog(SW_log, ex.Message);
                throw ex;
            }
        }

        public static void executeCmd(string args)
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;    //重定向标准输入
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;    //重定向错误输出
                //开始
                p.Start();
                p.StandardInput.WriteLine(args);
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();
                p.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                txtWriteHelper.AppendLog(SW_log, ex.Message);
                throw ex;
            }
        }
    }
}
