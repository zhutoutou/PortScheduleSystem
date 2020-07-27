using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace PortShipScheduleSystem
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string strExecutePath = System.Windows.Forms.Application.ExecutablePath;
            int dotPos = strExecutePath.LastIndexOf('\\');
            strExecutePath = strExecutePath.Substring(0, dotPos);   //exe所在目录

            string strSdkPath = strExecutePath + "\\YimaEnc.ocx";   //ocx放exe同目录，如不一样自行修改
            Process p = new Process();
            p.StartInfo.FileName = "Regsvr32.exe";
            p.StartInfo.Arguments = @"/s """ + strSdkPath + @"""";  //注册ocx,路径为中文、空格可兼容
            p.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FromMain());
        }
    }
}
