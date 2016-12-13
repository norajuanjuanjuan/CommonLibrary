using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InsertEntryPoint
{
    class txtWriteHelper
    {
        public static void CreateTxtFile(string path)
        {
            if (!System.IO.File.Exists(path))
                System.IO.File.Create(path).Close();
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                System.IO.File.Create(path).Close();
            }
        }
        public static void AppendLog(StreamWriter sw, string content)
        {
            sw.WriteLine(content);
            sw.Flush();
        }

        public static void writeLine(StreamWriter sw)
        {
            sw.WriteLine();
            sw.Flush();
        }
    }
}
