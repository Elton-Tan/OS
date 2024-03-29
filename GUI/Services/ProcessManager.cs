using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static TangerineOS.GUI.Process;

namespace TangerineOS.GUI
{
    public static class ProcessManager
    {
        internal static List<Process> running = new();
        static short second = -1;
        static char second10;
        public static void Refresh()
        {
            bool lowpriority = false;
            bool highpriority = false;
            if (RTC.Second != second)
            {
                highpriority = true;
                second = RTC.Second;
            }
            if (TangerineOS.Date.CurrentSecond()[0] != second10)
            {
                lowpriority = true;
                second10 = TangerineOS.Date.CurrentSecond()[0];
            }
            for (int i = running.Count; i > 0; i--)
            {
                uint j = GCImplementation.GetUsedRAM();

                running[i - 1].id = i - 1;

                if (running[i - 1] is Window w)
                {
                    if (!w.minimized) { WindowManager.Draw(w); }
                }
                if (running[i - 1].priority == Process.Priority.Realtime) { Update(i - 1); }
                if (highpriority && running[i - 1].priority == Process.Priority.High) { Update(i - 1); }
                if (lowpriority && running[i - 1].priority == Process.Priority.Low) { Update(i - 1); }

                running[i - 1].usageRAM += (GCImplementation.GetUsedRAM() - j) / 1024;
            }
        }
        public static void Update(int id)
        {
            try { running[id].Update(); }
            catch (Exception e) { string name = running[id].name; RemoveAt(id); Msg.Main("Error", "Process '" + name + "' crashed: " + e, Icons.error); }
        }
        internal static Process Run(Process process)
        {
            GUI.Loading = true;
            GUI.Refresh();
            uint j = GCImplementation.GetUsedRAM();
            process.id = 0;
            int code = process.Start();
            if (code == -1) { GUI.Loading = false; return null; }
            else if (code != 0) { Msg.Main("Process Manager", "Process '" + process.name + " launched and then stopped with an unexpected error code: " + code, Icons.warn); GUI.Loading = false; return null; }
            process.usageRAM = (GCImplementation.GetUsedRAM() - j) / 1024;
            GUI.Refresh();
            running.Insert(0, process);
            running[0].id = 0;
            GUI.Loading = false;
            return process;
        }
        public static int RemoveAt(int i, bool force = false)
        {
            GUI.Loading = true;
            int exitCode = running[i].Stop();
            //GCImplementation.Free(running[i]);
            if (force || exitCode == 0) { running.RemoveAt(i); }
            else if (exitCode != -1)
            {
                string name = running[i].name;
                running.RemoveAt(i);
                Msg.Main("Process Manager", "Process '" + name + " stopped with an unexpected error code: " + exitCode, Icons.warn);
            }
            GUI.Loading = false;
            return exitCode;
        }
        public static string StopAll()
        {
            foreach(Process process in running) { process.Stop(); }
            running.Clear();
            return null;
            GUI.Loading = true;
            GUI.Refresh();
            string result = null;
            int j = running.Count;
            for (int i = 0; i < j; i++)
            {
                if (RemoveAt(i) == -1)
                {
                    if (result == null)
                    {
                        result = running[i].name;
                    }
                    else { result += "; " + running[i].name; }
                }
            }
            GUI.Loading = false;
            return result;
        }
        public static void Click(int x, int y)
        {
            for (int i = 0; i < running.Count; i++)
            {
                if (running[i] is Window w)
                {
                    if (w.windowlock) { return; }
                    if (!w.minimized)
                    {
                        if (x > w.StartX && x < w.StartX + w.x && y > w.StartY + 30 && y < w.StartY + w.y + 30)
                        {
                            WindowManager.FocusAtWindow(i);
                            try { w?.OnClicked?.Invoke(x - w.StartX, y - w.StartY - 30); } catch(Exception e) { Msg.Main("Error", "Process '" + w.name + "' encountered an error: " + e, Icons.error); }
                            return;
                        }
                    }
                }
            }
        }
        public static string GetPriority(int id)
        {
            return running[id].priority switch
            {
                Priority.High => "High",
                Priority.Low => "Low",
                Priority.Realtime => "Realtime",
                Priority.None => "None",
                _ => "Unknown",
            };
        }
    }
    public static class WindowManager
    {
        internal static void Draw(Window w)
        {
            if (w.windowlock)
            {
                if (GUI.LongPress)
                {
                    w.StartX = w.StartXOld + (int)MouseManager.X;
                    w.StartY = w.StartYOld + (int)MouseManager.Y;
                    if (w.StartY < 0) { w.StartY = 0; }
                    else if (w.StartY > GUI.screenY - 60) { w.StartY = (int)GUI.screenY - 60; }
                    if (w.StartX < 0) { w.StartX = 0; }
                    else if (w.StartX + w.x > GUI.screenX) { w.StartX = (int)GUI.screenX - w.x; }
                }
                else
                {
                    w.windowlock = false;
                    w.borderCanvas = PostProcess.CropBitmap(Images.wallpaperBlur, w.StartX, w.StartY, w.x, 30);
                    Font.DrawString(w.name, Color.White.ToArgb(), 36, 10, w.borderCanvas.rawData, w.x);
                    Font.DrawImageAlpha(w.icon, 5, 3, w.borderCanvas.rawData, w.x);
                    Font.DrawImageAlpha(Icons.minimize, w.x - 50, 7, w.borderCanvas.rawData, w.x);
                    Font.DrawImageAlpha(Icons.close, w.x - 20, 7, w.borderCanvas.rawData, w.x);
                    w.OnMoved?.Invoke();
                }
                GUI.canvas.DrawImage(w.borderCanvas, w.StartX, w.StartY);
            }
            else
            {
                GUI.canvas.DrawImage(w.borderCanvas, w.StartX, w.StartY);
                if (MouseManager.Y > (w.StartY) && MouseManager.Y < w.StartY + 30)
                {
                    if (MouseManager.X > w.StartX - 23 + w.x && MouseManager.X < w.StartX + 1 + w.x)
                    {
                        GUI.canvas.DrawImageAlpha(Icons.close2, w.StartX - 20 + w.x, w.StartY + 7);
                        if (GUI.Pressed) { ProcessManager.RemoveAt(w.id); return; }
                    }
                    else if (MouseManager.X > w.StartX - 53 + w.x && MouseManager.X < w.StartX - 23 + w.x)
                    {
                        GUI.canvas.DrawImageAlpha(Icons.minimize2, w.StartX - 50 + w.x, w.StartY + 7);
                        if (GUI.Pressed) { w.minimized = true; }
                    }
                    else if (GUI.StartClick && MouseManager.X > w.StartX && MouseManager.X < w.StartX + w.x - 53)
                    {
                        w.StartXOld = w.StartX - (int)MouseManager.X;
                        w.StartYOld = w.StartY - (int)MouseManager.Y;
                        w.windowlock = true;
                        MemoryOperations.Fill(w.borderCanvas.rawData, 0);
                        Font.DrawString(w.name, Color.White.ToArgb(), 36, 10, w.borderCanvas.rawData, w.x);
                        Font.DrawImageAlpha(w.icon, 5, 3, w.borderCanvas.rawData, w.x);
                        Font.DrawImageAlpha(Icons.minimize, w.x - 50, 7, w.borderCanvas.rawData, w.x);
                        Font.DrawImageAlpha(Icons.close, w.x - 20, 7, w.borderCanvas.rawData, w.x);
                        FocusAtWindow(w.id);
                        w.OnStartMoving?.Invoke();
                    }
                }
            }
            GUI.canvas.DrawImage(w.appCanvas, w.StartX, w.StartY + 30);
        }
        public static void FocusAtWindow(int id)
        {
            if (ProcessManager.running[id] is Window w)
            {
                w.minimized = false;
                MoveToTop(ProcessManager.running, id);
            }
        }
        public static void MoveToTop<T>(this List<T> list, int index)
        {
            T item = list[index];
            for (int i = index; i > 0; i--)
                list[i] = list[i - 1];
            list[0] = item;
        }
    }
    internal class TaskManager : Window
    {
        internal TaskManager() : base("Process Manager", 400, 400, new Bitmap(Resources.TaskmngIcon), Priority.High) { OnClicked = Clicked; OnKeyPressed = Key; }
        private int selY = -1;
        private int list = -1;
        internal override int Start()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            return 0;
        }
        internal override void Update()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            DrawFilledRectangle(GUI.SystemPen.ValueARGB, x - 100, y - 50, 90, 20);
            DrawStringAlpha("End task", Color.Gray.ToArgb(), x - 90, y - 45);
            DrawHorizontalLine(Color.White.ToArgb(), 5, 10, x - 10);
            DrawString(" Task ", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 0, 5); DrawString(" RAM ", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, x - 160, 5); DrawString(" Priority ", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, x - 80, 5);
            byte i = 0;
            if (selY != -1)
            {
                if(ProcessManager.running.Count < list) { selY--; list--; }
                else if (ProcessManager.running.Count > list) { selY++; list++; }
            }
            foreach(var task in ProcessManager.running)
            {
                i++;

                if(selY == i - 1)
                {
                    DrawFilledRectangle(Color.Gray.ToArgb(), 10, selY * 20 + 29, this.x - 20, Font.fontY + 2);
                    DrawStringAlpha("End task", Color.White.ToArgb(), x - 90, y - 45);
                }
                if (i < 17) { Print(task, i); }
            }
            int ram = (int)(TangerineOS.Sysinfo.UsedRAM / TangerineOS.Sysinfo.InstalledRAM * 100);
            DrawFilledRectangle(Color.DarkGray.ToArgb(), 0, y - 20, x, 20);
            if (ram > 100) { ram = 100; }
            DrawFilledRectangle(GUI.SystemPen.ValueARGB, 0, y - 20, ram*(x/100), 20);
            DrawStringAlpha("RAM: " + ram + "%", Color.White.ToArgb(), 10, y - 15);
        }
        internal override int Stop() { return 0; }
        private void Print(Process task, int i)
        {
            DrawStringAlpha(task.name, Color.White.ToArgb(), 10, i * 20 + 10); DrawStringAlpha(task.usageRAM + " KB", Color.White.ToArgb(), x - 150, i * 20 + 10); DrawStringAlpha(ProcessManager.GetPriority(task.id), Color.White.ToArgb(), x - 70, i * 20 + 10);
        }
        private void Clicked(int x, int y)
        {
            if (y > 30 && y < ProcessManager.running.Count * 20 + 29 && y < this.y - 60)
            {
                if (selY == (y - 30) / 20)
                {
                    if (ProcessManager.running[selY] is Window) { WindowManager.FocusAtWindow(selY); }
                }
                else if (selY != -1)
                {
                    DrawFilledRectangle(GUI.DarkGrayPen.ValueARGB, 10, selY * 20 + 29, this.x - 20, Font.fontY + 2);
                    Print(ProcessManager.running[selY], selY + 1);
                }
                selY = (y - 30) / 20;
                list = ProcessManager.running.Count;
                DrawFilledRectangle(Color.Gray.ToArgb(), 10, selY * 20 + 29, this.x - 20, Font.fontY + 2);
                Print(ProcessManager.running[selY], selY + 1);
                DrawStringAlpha("End task", Color.White.ToArgb(), this.x - 90,  this.y - 45);
            }
            else if (selY != -1)
            {
                if (x > this.x - 100 && x < this.x - 10 && y > this.y - 50 && y < this.y - 30) { ProcessManager.RemoveAt(selY); selY = -1; return; }
                DrawFilledRectangle(GUI.DarkGrayPen.ValueARGB, 10, selY * 20 + 29, this.x - 20, Font.fontY + 2);
                Print(ProcessManager.running[selY], selY + 1); selY = -1;
            }
        }
        private void Key(KeyEvent key)
        {
            if(selY != -1 && key.Key == ConsoleKeyEx.Delete) { ProcessManager.RemoveAt(selY); selY = -1; }
        }
    }
}