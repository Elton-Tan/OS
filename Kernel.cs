using Cosmos.Core;
using Cosmos.System.Graphics;
using System;
using System.Threading;
using Display = TangerineOS.GUI.GUI;
using Sys = Cosmos.System;

namespace TangerineOS
{
    public class Kernel : Sys.Kernel
    {
        public static readonly string OSVERSION = "TangerineOS Version Alpha 0.5.1";
        public static readonly string OSNAME = "TangerineOS";
        public static string PCNAME = "pc";
        public static readonly string MAINDISK = "0:\\";
        public static readonly string SYSTEMPATH = MAINDISK + "TangerineOS\\";
        public static readonly string USERSPATH = SYSTEMPATH + "Users\\";

        public static readonly string PROGRAMSPATH = SYSTEMPATH + "Programs\\";
        public static readonly string PROGRAMSDATAPATH = SYSTEMPATH + "Config\\";
        public static readonly string SYSTEMCONFIG = PROGRAMSDATAPATH + "systemConfig.cfg";

        public static readonly string CURRENTUSER = USERSPATH + "Admin\\";
        public static readonly string USERPROGRAMSPATH = CURRENTUSER + "Programs\\";
        public static readonly string USERPROGRAMSDATAPATH = CURRENTUSER + "Config\\";
        public static readonly string USERCONFIG = USERPROGRAMSDATAPATH + "userConfig.cfg";

        public static bool GUIenabled = false;
        public static bool useDisks = true;
        public static bool useNetwork = false;
        public static bool safeMode = false;

        //BeforeRun() is the display before the OS start.
        protected override void BeforeRun()
        {
            try
            {
                Console.Clear(); //clear console of all data
                //Console.CursorVisible = false;
                Console.CursorLeft = 35;
                Console.CursorTop = 11;
                Console.ForegroundColor = ConsoleColor.Yellow; // set console to green
                Console.WriteLine("TangerineOS"); //Our OS name
                Console.CursorLeft = 0;
                Console.CursorTop = Console.WindowHeight - 1;
                TextMode.Run();
            }
            catch (Exception e)
            {
                SystemCrash(e.Message);
            }

        }
        protected override void Run()
        {
            try
            {
                if (GUIenabled) { Display.Refresh(); } //because we cleared the display before, we will refresh it
                else { TextMode.ConsoleMode(); }
            }
            //if the system encounter error when starting
            catch (Exception e) 
            {
                try
                {
                    if (GUIenabled)
                    { Display.Refresh(); Msg.Main("Error", "System " + e, GUI.Icons.error); } //Display error
                    else { Console.WriteLine("System " + e, GUI.Icons.error); } //Show error in console
                }
                //if cant even load the GUI
                catch (Exception ex)
                {
                    SystemCrash(ex.ToString()); //then we will crash the system and catch the error
                }
            }
        }

        //happens when system crash
        public static void SystemCrash(string e)
        {
            Display.canvas?.Disable();
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.CursorTop = Console.WindowHeight / 3;
            Console.CursorLeft = (Console.WindowWidth - OSVERSION.Length - 4) / 2;
            Console.Write("  " + OSVERSION + "  ");
            Thread.Sleep(500);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.CursorLeft = (Console.WindowWidth - ("Fatal System " + e).Length) / 2;
            Console.Write("Fatal System " + e);
            Thread.Sleep(500);
            Console.WriteLine();
            Console.WriteLine();
            Console.CursorLeft = (Console.WindowWidth - "Press any key to restart computer ".Length) / 2;
            Console.Write("Press any key to restart computer ");
            Console.ReadKey(true); //awat until a kew is pressed and read
            Sys.Power.Reboot();//then reboot the system
            ACPI.Reboot();
        }

        //Shutting down / Restarting the OS
        public static void Shutdown(bool restart = false, bool force = false)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.CursorVisible = false;
            Console.Clear();
            Console.SetCursorPosition(18, Console.WindowHeight / 2 - 1);
            if (force)
            {
                if (restart) { Sys.Power.Reboot(); }  //if user select restart, OS reboots
                else { Sys.Power.Shutdown(); } //if not it will just shutdown for good
            }
            else
            {
                if (GUIenabled)
                {
                    Display.ShutdownGUI(restart);//we shutdown the GUI
                }
                Console.Beep(700, 100);
                Console.Beep(600, 100);
                Console.Beep(500, 100);
                if (restart)
                {
                    Console.Beep(400, 300);
                    if (GUIenabled) { Display.canvas.Disable(); }
                    Thread.Sleep(100);
                    Sys.Power.Reboot();
                }
                else
                {
                    Console.Beep(400, 100);
                    Console.Beep(300, 200);
                    if (GUIenabled) { Display.canvas.Disable(); }
                    Thread.Sleep(100);
                    Sys.Power.Shutdown();
                }
            }
            Console.Write("It is now safe to exit the virtual machine.");
            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}