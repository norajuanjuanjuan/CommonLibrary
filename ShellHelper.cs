using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutoTesterLib
{
   public class ShellHelper
    {
       public static Process GetShellProcess()
       {
           Process process = new Process();
           //设定程序名
           process.StartInfo.FileName = "cmd.exe";
           //关闭Shell的使用
           process.StartInfo.UseShellExecute = false;
           //重新定向标准输入，输入，错误输出
           process.StartInfo.RedirectStandardInput = true;
           process.StartInfo.RedirectStandardOutput = true;
           process.StartInfo.RedirectStandardError = true;
           //设置cmd窗口不显示
           process.StartInfo.CreateNoWindow = true;

           return process;
       }
    }
}
