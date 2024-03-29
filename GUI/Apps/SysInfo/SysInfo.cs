using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using TangerineOS.Commands;
using System;
using System.Collections;
using System.Drawing;

namespace TangerineOS
{
    public static class Sysinfo
    {
        public static string CPUname;
        public static uint InstalledRAM
        {
            get { return CPU.GetAmountOfRAM(); }
        }
        public static uint ReservedRAM
        {
            get { return InstalledRAM - (uint)GCImplementation.GetAvailableRAM(); }
        }
        public static double UsedRAM
        {
            get { return (GCImplementation.GetUsedRAM() / (1024.0 * 1024.0)) + ReservedRAM; }
        }
        public static uint AvailableRAM
        {
            get { return InstalledRAM - (uint)UsedRAM; }
        }
        public static string CPUuptime
        {
            get { return TimeSpan.FromSeconds(CPU.GetCPUUptime() / 3200000000).ToString(@"hh\:mm\:ss"); }
        }
        public static string DisplayRes
        {
            get { return Kernel.GUIenabled ? "Display: " + GUI.GUI.canvas.Mode : "Console Display: " + System.Console.WindowWidth + "x" + System.Console.WindowHeight; }
        }
        public static string Main()
        {
            return Kernel.OSVERSION + "\n" + DisplayRes +"\nCPU: " + CPUname + "\nCPU Uptime: " + CPUuptime;
        }
        public static string Ram()
        {
            return "RAM: " + UsedRAM.ToString("0.00") + "/" + InstalledRAM.ToString("0.00") + " MB (" + (int)((UsedRAM / InstalledRAM) * 100) + "%)";
        }
    }
}
namespace TangerineOS.GUI
{
    internal class InfoSystem: Window
    {
        internal InfoSystem() : base("System Info", 500, 200, new Bitmap(Resources.InfoSystemIcon), Priority.High) { }
        internal override int Start()
        {
            MemoryOperations.Fill(appCanvas.rawData, GUI.DarkGrayPen.ValueARGB);
            return 0;
        }
        internal override void Update()
        {
            MemoryOperations.Fill(appCanvas.rawData, GUI.DarkGrayPen.ValueARGB);
            DrawString(TangerineOS.Sysinfo.Main(), Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 10);
            DrawString("FPS: " + GUI.fps, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 90);
            DrawString(TangerineOS.Sysinfo.Ram(), Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 110);
        }

        internal override int Stop() { return 0; }
    }
}
namespace TangerineOS.Commands
{
    internal class SystemInfo : CommandsTree
    {
        internal SystemInfo() : base
            ("System Info", "Provides info about system",
            new Command[] {
            new Command(new string[] { "sysinfo", "systeminfo", "sys", "system"}, "Display info about system")
            })
        {
        }
        internal override int Execute(string[] args, CommandShell shell)
        {
            if (args[0] == "sysinfo" || args[0] == "systeminfo" || args[0] == "sys" || args[0] == "system")
            {
                shell.print = TangerineOS.Sysinfo.Main() + "\n" + TangerineOS.Sysinfo.Ram();
                return 0;
            }
            return 1;
        }
    }
}