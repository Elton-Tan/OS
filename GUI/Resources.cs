using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System.Threading;

namespace TangerineOS.GUI
{
    public static class Resources
    {
        //Cursors
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.Cursors.Cursor.bmp")]
        public static byte[] Cursor;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.Cursors.CursorLoad.bmp")]
        public static byte[] CursorLoad;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.Cursors.CursorWhite.bmp")]
        public static byte[] CursorWhite;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.Cursors.CursorWhiteLoad.bmp")]
        public static byte[] CursorWhiteLoad;
        //UI
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.loading.bmp")]
        public static byte[] Load;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.check.bmp")]
        public static byte[] Check;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.check2.bmp")]
        public static byte[] Check2;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.circle.bmp")]
        public static byte[] Circle;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.circle2.bmp")]
        public static byte[] Circle2;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.warn.bmp")]
        public static byte[] Warn;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.info.bmp")]
        public static byte[] Info;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.error.bmp")]
        public static byte[] Error;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.close.bmp")]
        public static byte[] CloseButton;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.close2.bmp")]
        public static byte[] Close2;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.minimize.bmp")]
        public static byte[] Minimize;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.minimize2.bmp")]
        public static byte[] Minimize2;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.taskmng.bmp")]
        public static byte[] TaskmngIcon;
        //Walpapers
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.Wallpapers.Wallpaper.bmp")]
        public static byte[] Wallpaper;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.Wallpapers.WallpaperLock.bmp")]
        public static byte[] WallpaperLock;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.Wallpapers.WallpaperOld.bmp")]
        public static byte[] WallpaperOld;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.Wallpapers.2005s.bmp")]
        public static byte[] Wallpaper2005s;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.Wallpapers.Origami.bmp")]
        public static byte[] WallpaperOrigami;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.Wallpapers.Cosmos.bmp")]
        public static byte[] WallpaperCosmos;

        //Icons
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Resources.UI.program.bmp")]
        public static byte[] Program;

        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Apps.Console.console.bmp")]
        public static byte[] ConsoleIcon;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Apps.SysInfo.sysinfo.bmp")]
        public static byte[] InfoSystemIcon;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Apps.Settings.settings.bmp")]
        public static byte[] Settings;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Apps.Files.files.bmp")]
        public static byte[] Files;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Apps.Notepad.notepad.bmp")]
        public static byte[] Notepad;

        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Apps.Files.disk.bmp")]
        public static byte[] DiskIcon;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Apps.Files.file.bmp")]
        public static byte[] FileIcon;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.Apps.Files.refresh.bmp")]
        public static byte[] RefreshIcon;

        //Menu
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.UI.Menu.reboot.bmp")]
        public static byte[] Reboot;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.UI.Menu.shutdown.bmp")]
        public static byte[] Shutdown;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.UI.Menu.lock.bmp")]
        public static byte[] LockIcon;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.UI.Menu.start.bmp")]
        public static byte[] StartButton;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.UI.Menu.start2.bmp")]
        public static byte[] Start2;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.UI.Menu.start3.bmp")]
        public static byte[] Start3;
        [ManifestResourceStream(ResourceName = "TangerineOS.GUI.UI.Menu.connected.bmp")]
        public static byte[] Connected;

        public static void InitResources()
        {
            //Cursors
            Icons.cursor = new Bitmap(Cursor);
            Icons.cursorload = new Bitmap(CursorLoad);
            //UI
            Icons.warn = new Bitmap(Warn);
            Icons.info = new Bitmap(Info);
            Icons.error = new Bitmap(Error);
            Icons.load = new Bitmap(Load);

            Icons.close = new Bitmap(CloseButton);
            Icons.close2 = new Bitmap(Close2);
            Icons.minimize = new Bitmap(Minimize);
            Icons.minimize2 = new Bitmap(Minimize2);

            Icons.program = new Bitmap(Program);

            Icons.lockicon = new Bitmap(LockIcon);
            Icons.reboot = new Bitmap(Reboot);
            Icons.shutdown = new Bitmap(Shutdown);
            Icons.connected = new Bitmap(Connected);

            Menu.start = new Bitmap(StartButton);
            Menu.start2 = new Bitmap(Start2);
            Menu.start3 = new Bitmap(Start3);
        }
    }
}