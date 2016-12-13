using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InsertEntryPoint
{
    class bcpHelper
    {
        public static StreamWriter SW_log { get; set; }
        static string bcpExePath = Environment.GetEnvironmentVariable("programfiles")+
            @"\Microsoft SQL Server\100\Tools\Binn\bcp.exe";

        public static bool BatchInsertProcess(string dbName,string svName,string sourceFile,string targetTable,
            string errorFile,string outBcpFile)
        {
            bool isSucceeded = false;
            //-c 文件需为Encoding.ASCII ; -w 写入文件需为Unicode 
           string args = "["+dbName+"]" + ".." +"["+ targetTable+"]" + " in \"" + sourceFile + "\"" +
                    " -S " + svName + " -T -w -t \",\" -e \""+errorFile+"\" -o \""+outBcpFile+"\"";
           shellHelper.executeShellExeCmd(bcpExePath,args);
         
            //判断bcp是否成功
           FileInfo fi = new FileInfo(errorFile);
           if (fi.Length!= 0)
           {
               Console.WriteLine("Error:Fail to process bcp!");
               txtWriteHelper.AppendLog(SW_log, "Error:Fail to process bcp!");
           }
            else if (fi.Length==0)
           {
               isSucceeded = true;
            }
           return isSucceeded;
        }
    }
}
