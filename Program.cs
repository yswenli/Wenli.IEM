using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Wenli.IEM.Win32;

namespace Wenli.IEM
{
    static class Program
    {
        public static KeyboardHook keyBordHook = new KeyboardHook();

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);            
            keyBordHook.Start();
            Application.Run(new WordBoard());
            keyBordHook.Stop();
        }        
    }
}
