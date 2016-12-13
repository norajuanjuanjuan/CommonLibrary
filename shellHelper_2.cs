using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsertEntryPoint
{
    class shellHelper
    {
        public static void executeShellCmd(string exePath,string args)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = exePath;
            p.StartInfo.Arguments =args ;
            p.StartInfo.RedirectStandardInput = true;    //重定向标准输入
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;    //重定向错误输出
            //开始
            p.Start();
            p.WaitForExit();
            p.Close();
        }
    }
}
