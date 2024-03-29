﻿using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.HAL.BlockDevice;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace TangerineOS.GUI
{
    public class GUI
    {
        //We are setting the OS width by height
        public static Mode displayMode { get; private set; } = new(1280, 720, ColorDepth.ColorDepth32);
        public static int screenX { get { return displayMode.Columns; } }
        public static int screenY { get { return displayMode.Rows; } }

        //We are just creating variable. You can ignore
        public static int fps;
        private static int fps2;
        private static int frames;
        private static bool displayFPS = true;

        public static bool wasClicked;

        public static bool Lock = true;
        public static bool screenSaver;
        public static bool debug;
        public static byte ExecuteError;

        public static bool Pressed;
        private static bool OneClick;
        private static bool StartOneClick;
        public static bool StartClick;
        public static bool LongPress;
        public static bool Loading;
        public static bool DisplayCursor;

        public static Canvas canvas;

        public static readonly Pen WhitePen = new(Color.White);
        public static readonly Pen GrayPen = new(Color.Gray);
        public static readonly Pen DarkPen = new(Color.Black);
        public static readonly Pen DarkGrayPen = new(Color.FromArgb(40, 40, 40));
        public static readonly Pen RedPen = new(Color.DarkRed);
        public static readonly Pen Red2Pen = new(Color.Red);
        public static readonly Pen GreenPen = new(Color.Green);
        public static readonly Pen YellowPen = new(Color.Goldenrod);
        public static readonly Pen BluePen = new(Color.SteelBlue);
        public static readonly Pen DarkBluePen = new(Color.MidnightBlue);
        public static Pen SystemPen = BluePen;

        public static PCScreenFont font;
        private static Menu menu;

        public static int wallpapernum; //We will use this later when changing the wallpaper

        // End of variable creation
        public static string Init(string overrideRes = null)
        {
            try
            {
                if (overrideRes != null) { displayMode = ResParse(overrideRes); }

                else if (Kernel.safeMode) { displayMode = new(800, 600, ColorDepth.ColorDepth32); } //display as size 800 x 600

                else if (Kernel.useDisks) { displayMode = Profiles.LoadSystem(); } //load the OS system

                SetRes(displayMode, true, true);
                Lock = true;
                MouseManager.X = (uint)GUI.screenX / 2;
                MouseManager.Y = (uint)GUI.screenY / 2;
              
                canvas.Display();

                if(font == null) //if font cant be found
                {
                    font = PCScreenFont.Default;
                    Resources.InitResources(); // We will load all the resources as initialised in our resources file
                    Font.Main();
                }

                if (!Kernel.safeMode) //if NOT in safemode, which means in useDisks mode. Focus on the exclamation mark the !Kernal.safeMode, means NOT in safe mode
                {
                    Settings.wallpapernum = Profiles.LoadUser();
                    switch (Settings.wallpapernum)
                    {
                        case 1:
                            ApplyRes(new Bitmap(Resources.WallpaperOld)); break;
                        case 2:
                            ApplyRes(new Bitmap(Resources.WallpaperLock)); break;
                        case 3:
                            ApplyRes(new Bitmap(Resources.WallpaperOrigami)); break;
                        case 4:
                            ApplyRes(new Bitmap(Resources.Wallpaper2005s)); break;
                        case 5:
                            ApplyRes(new Bitmap(Resources.WallpaperCosmos)); break;
                        default:
                            ApplyRes(new Bitmap(Resources.Wallpaper)); break; //load default wallpaper. Our default wallpaper called 'Wallpaper' in the resources file
                    }
                    ProcessManager.Run(new ScreenSaverService());
                    ProcessManager.Run(new PerformanceWatchdog());
                    TangerineOS.Net.Start(); //Start the OS function
                }
                else
                {
                    Images.wallpaper = new Bitmap(800, 600, ColorDepth.ColorDepth32);
                    MemoryOperations.Fill(Images.wallpaper.rawData, Color.CadetBlue.ToArgb());
                    Images.wallpaperBlur = new Bitmap(800, 600, ColorDepth.ColorDepth32); //Just blur the image a little
                    menu = new();
                }

                ProcessManager.Run(new GlobalInput());
                DisplayCursor = true;
                return "OK";
            }
            catch (Exception e)
            {
                Kernel.GUIenabled = false;
                try { canvas.Disable(); } catch { }
                return e.Message;
            }
        }
        public static void Refresh()
        {
            if (fps2 != RTC.Second)
            {
                fps = frames;
                frames = 0;
                fps2 = RTC.Second;
            }
            frames++;

            switch (MouseManager.MouseState)
               
            {
                case MouseState.Left or MouseState.Right:
                    StartClick = false;
                    if (!StartOneClick) { StartOneClick = true; StartClick = true; }
                    //enable clicking
                    OneClick = true;
                    LongPress = true;
                    wasClicked = true;
                    break;

                case MouseState.None:
                    Pressed = false; 
                    if (OneClick)
                    {
                        Pressed = true;
                        OneClick = false;
                        if(!screenSaver && !Lock && MouseManager.Y > 30) { ProcessManager.Click((int)MouseManager.X, (int)MouseManager.Y); }
                    }
                    StartOneClick = false; 
                    StartClick = false;
                    LongPress = false;
                    break;
            }

            if (screenSaver) { ScreenSaver.Update(); }
            else
            {
                if (Lock) { LockScreen.Update(); } //when screen is locked (in safemode) , we will use the latest wallpaper the user has set
                else
                {
                    canvas.DrawImage(Images.wallpaper, 0, 0);
                    ProcessManager.Refresh();
                    menu.Update(); //if not locked then just update the wallpaper
                }
            }
            if (debug) //just to catch some errors for debugging...
            {
                if (ExecuteError == 1) { ExecuteError = 0; throw new Exception("Manual crash"); }
                else if (ExecuteError == 2) { throw new Exception("Manual crash"); }
                canvas.DrawString(TangerineOS.Sysinfo.Ram(), font, WhitePen, 10, 0);
            }
            //nothing important just some settings intialisation
            Toast.Update();
            if (DisplayCursor) { if (Loading) { canvas.DrawImageAlpha(Icons.cursorload, (int)MouseManager.X, (int)MouseManager.Y); } else { canvas.DrawImageAlpha(Icons.cursor, (int)MouseManager.X, (int)MouseManager.Y); } }

            //nothing important just some settings intialisation
            if (displayFPS)
            {
                GUI.canvas.DrawFilledRectangle(DarkPen, (int)GUI.screenX - 14, 0, 14, 7);
                Font.DrawNumbers(Convert.ToString(fps), Color.Yellow, (int)GUI.screenX - 13, 1);
            }

            Heap.Collect();
            canvas.Display();
        }
        public static void Wait() 
        {
            canvas.DrawImageAlpha(Icons.cursorload, (int)MouseManager.X, (int)MouseManager.Y);
            Toast.Update();
            canvas.Display();
        }

        //When shutting down
        public static void ShutdownGUI(bool restart = false) //to shutdown pc use Kernel.Shutdown() instead
        {
            Toast.Force("Stopping processes...");
            string str = ProcessManager.StopAll();
            if (str != null) //to do
            {
                Toast.Force(str);
                Thread.Sleep(1000);
            }
            canvas.DrawImage(Images.wallpaperBlur, 0, 0);
            if (Kernel.useDisks && !Kernel.safeMode)
            {
                Toast.Force("Saving user settings...");
                Profiles.Save(); //Save the user setting
                Thread.Sleep(500);
                canvas.DrawImage(Images.wallpaperBlur, 0, 0);
            }
            Toast.Display(restart ? "Restarting..." : "Shutting down..."); //if restart, show restart, if not display message as shut down
            canvas.Display();
            Toast.msg = null;
            Kernel.GUIenabled = false;
        }

        //focus until here... ignore the rest below as they are not working as intended and doesnt show up in the GUI.

        //setting resolution... sorry you guys can ignore this block because it doesnt work...
        public static string SetRes(Mode mode, bool fast = false, bool boot = false)
        {
            try
            {
                //canvas = new VBECanvas(new Mode(1280, 800, ColorDepth.ColorDepth32));
                canvas = (VBECanvas)FullScreenCanvas.GetFullScreenCanvas(mode);
            }
            catch (Exception e)
            {
                if (!boot) { SetRes(displayMode, fast); }
                throw new Exception("Resolution " + mode.Columns + "x" + mode.Rows + " is not available; " + e);
            }
            displayMode = mode;
            MouseManager.ScreenWidth = (uint)mode.Columns - 5;
            MouseManager.ScreenHeight = (uint)mode.Rows;
            if (!fast)
            {
                switch (Settings.wallpapernum)
                {
                    case 1:
                        ApplyRes(new Bitmap(Resources.WallpaperOld)); break;
                    case 2:
                        ApplyRes(new Bitmap(Resources.WallpaperLock)); break;
                    case 3:
                        ApplyRes(new Bitmap(Resources.WallpaperOrigami)); break;
                    case 4:
                        ApplyRes(new Bitmap(Resources.Wallpaper2005s)); break;
                    case 5:
                        ApplyRes(new Bitmap(Resources.WallpaperCosmos)); break;
                    default:
                        ApplyRes(new Bitmap(Resources.Wallpaper)); break;
                }
            }
            Kernel.GUIenabled = true;
            return "Successfully changed resolution to " + mode.Columns + "x" + mode.Rows; // mode.ColorDepth.ToString() not implemented
        }

        //End of block 

        //Doesnt work too
        public static void ApplyRes(Bitmap wallpaper)
        {
            Images.wallpaper = PostProcess.ResizeBitmap(wallpaper, (uint)GUI.screenX, (uint)GUI.screenY);
            Images.wallpaperBlur = PostProcess.ResizeBitmap(PostProcess.DarkenBitmap(PostProcess.ApplyBlur(PostProcess.ResizeBitmap(wallpaper, 640, 360), 20), 0.5f), (uint)GUI.screenX, (uint)GUI.screenY);
            menu = new();
            for (int i = 0; i < ProcessManager.running.Count; i++)
            {
                if (ProcessManager.running[i] is Window w)
                { w.RefreshBorder(); }
            }
        }
        public static Mode ResParse(string res)
        {
            if (res.Contains('x'))
            {
                ColorDepth colorss;
                uint x;
                uint y;
                if (res.Contains('@'))
                {
                    string[] split = res.Split('@');
                    colorss = GetColorDepth(split[1]);
                    x = Convert.ToUInt32(split[0].Split('x')[0]);
                    y = Convert.ToUInt32(split[0].Split('x')[1]);
                }
                else
                {
                    x = Convert.ToUInt32(res.Split('x')[0]);
                    y = Convert.ToUInt32(res.Split('x')[1]);
                    colorss = displayMode.ColorDepth;
                }
                return new Mode((int)x, (int)y, colorss);
            }
            else
            {
                throw new Exception("'x' character expected");
            }
        }
        public static ColorDepth GetColorDepth(string str)
        {
            return str switch
            {
                "32" => ColorDepth.ColorDepth32,
                "24" => ColorDepth.ColorDepth24,
                "16" => ColorDepth.ColorDepth16,
                "8" => ColorDepth.ColorDepth8,
                "4" => ColorDepth.ColorDepth4,
                _ => ColorDepth.ColorDepth32,
            };
        }
    }

    //end of block
    public class PostProcess
    {
        public static Bitmap DarkenBitmap(Bitmap bitmap, float factor)
        {
            int[] originalData = bitmap.rawData;
            int dataSize = originalData.Length;

            // Iterate over each pixel and darken its brightness
            for (int i = 0; i < dataSize; i++)
            {
                int argb = originalData[i];

                // Extract the individual color components
                byte alpha = (byte)((argb >> 24) & 0xFF);
                byte red = (byte)((argb >> 16) & 0xFF);
                byte green = (byte)((argb >> 8) & 0xFF);
                byte blue = (byte)(argb & 0xFF);

                // Darken the color components by applying the specified factor
                red = (byte)(red * factor);
                green = (byte)(green * factor);
                blue = (byte)(blue * factor);

                // Combine the modified color components back into an ARGB value
                int modifiedArgb = (alpha << 24) | (red << 16) | (green << 8) | blue;

                // Update the pixel in the bitmap with the modified color
                originalData[i] = modifiedArgb;
            }

            // Create a new bitmap with the modified data
            bitmap = new Bitmap(bitmap.Width, bitmap.Height, GUI.displayMode.ColorDepth);
            bitmap.rawData = originalData;
            return bitmap;
        }
        public static Bitmap ResizeBitmap(Bitmap bmp, uint nX, uint nY)
        {
            if (bmp.Width == nX && bmp.Height == nY)
            {
                return bmp;
            }
            Bitmap resized = new Bitmap(nX, nY, GUI.displayMode.ColorDepth);
            int[] origIndices = new int[nX * nY];

            for (int i = 0; i < nX; i++)
            {
                for (int j = 0; j < nY; j++)
                {
                    int origX = (int)(i * bmp.Width / nX);
                    int origY = (int)(j * bmp.Height / nY);
                    int origIndex = (int)(origX + origY * bmp.Width);
                    int resizedIndex = (int)(i + j * nX);
                    origIndices[resizedIndex] = origIndex;
                }
            }

            for (int i = 0; i < origIndices.Length; i++)
            {
                resized.rawData[i] = bmp.rawData[origIndices[i]];
            }

            return resized;
        }
        public static Bitmap CropBitmap(Bitmap bitmap, int x, int y, int width, int height)
        {
            int[] originalData = bitmap.rawData;
            int croppedWidth = width;
            int croppedHeight = height;
            int[] croppedData = new int[croppedWidth * croppedHeight];

            for (int row = 0; row < croppedHeight; row++)
            {
                for (int col = 0; col < croppedWidth; col++)
                {
                    int originalX = x + col;
                    int originalY = y + row;
                    int originalIndex = (int)(originalY * bitmap.Width + originalX);
                    int croppedIndex = row * croppedWidth + col;
                    croppedData[croppedIndex] = originalData[originalIndex];
                }
            }
            bitmap = new Bitmap((uint)croppedWidth, (uint)croppedHeight, GUI.displayMode.ColorDepth);
            bitmap.rawData = croppedData;
            return bitmap;
        }
        public static Bitmap ApplyBlur(Bitmap bitmap, int blurRadius)
        {
            uint width = bitmap.Width;
            uint height = bitmap.Height;

            // create a temporary bitmap to store the blurred image
            Bitmap blurredBitmap = new Bitmap(width, height, GUI.displayMode.ColorDepth);
            int[] blurredData = blurredBitmap.rawData;
            int[] originalData = bitmap.rawData;

            // calculate the Gaussian kernel
            double[] kernel = new double[blurRadius * 2 + 1];
            double sigma = blurRadius / 3.0;
            double sum = 0;
            for (int i = 0; i < kernel.Length; i++)
            {
                double x = i - blurRadius;
                kernel[i] = Math.Exp(-(x * x) / (2 * sigma * sigma));
                sum += kernel[i];
            }

            // normalize the kernel
            for (int i = 0; i < kernel.Length; i++)
            {
                kernel[i] /= sum;
            }

            // loop through the pixels and apply the blur effect
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double r = 0, g = 0, b = 0;
                    for (int i = -blurRadius; i <= blurRadius; i++)
                    {
                        int idx = x + i;
                        if (idx < 0 || idx >= width)
                        {
                            idx = x;
                        }
                        int pixel = originalData[y * width + idx];
                        double weight = kernel[i + blurRadius];
                        r += ((pixel >> 16) & 0xFF) * weight;
                        g += ((pixel >> 8) & 0xFF) * weight;
                        b += (pixel & 0xFF) * weight;
                    }
                    long idx2 = y * width + x;
                    int blurredPixel = ((int)r << 16) | ((int)g << 8) | (int)b;
                    blurredData[idx2] = blurredPixel;
                }
            }

            return blurredBitmap;
        }
    }
    public class Font
    {
        public static int fontX = GUI.font.Width;
        public static int fontY = GUI.font.Height;
        public static Dictionary<char, bool[]> charCache = new Dictionary<char, bool[]>();
        public static Dictionary<char, bool[]> numCache = new Dictionary<char, bool[]>();
        public static void Main()
        {
            foreach (char c in "?ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[{]}\\|;:'\",<.>/`~")
            {
                charCache[c] = CreateCharCache(c);
            }

            numCache.Add('0', new bool[] {
    true, true, true,
    true, false, true,
    true, false, true,
    true, false, true,
    true, true, true
});

            numCache.Add('1', new bool[] {
    false, true, false,
    true, true, false,
    false, true, false,
    false, true, false,
    true, true, true
});

            numCache.Add('2', new bool[] {
    true, true, true,
    false, false, true,
    false, true, false,
    true, false, false,
    true, true, true
});

            numCache.Add('3', new bool[] {
    true, true, true,
    false, false, true,
    false, true, false,
    false, false, true,
    true, true, true
});

            numCache.Add('4', new bool[] {
    true, false, true,
    true, false, true,
    true, true, true,
    false, false, true,
    false, false, true
});

            numCache.Add('5', new bool[] {
    true, true, true,
    true, false, false,
    true, true, true,
    false, false, true,
    true, true, true
});

            numCache.Add('6', new bool[] {
    true, true, true,
    true, false, false,
    true, true, true,
    true, false, true,
    true, true, true
});

            numCache.Add('7', new bool[] {
    true, true, true,
    false, false, true,
    false, false, true,
    false, true, false,
    false, true, false
});

            numCache.Add('8', new bool[] {
    true, true, true,
    true, false, true,
    true, true, true,
    true, false, true,
    true, true, true
});

            numCache.Add('9', new bool[] {
    true, true, true,
    true, false, true,
    true, true, true,
    false, false, true,
    true, true, true
});

        }
        public static void DrawChar(char c, Color color, int x, int y)
        {
            if (c == ' ') { return; }
            bool[] cache = charCache[c];
            for (int py = 0; py < fontY; py++)
            {
                for (int px = 0; px < fontX; px++)
                {
                    if (cache[py * fontX + px])
                    {
                        GUI.canvas.DrawPoint(new Pen(color), x + px, y + py);
                    }
                }
            }
        }

        public static void DrawNum(char c, Color color, int x, int y)
        {
            if (!char.IsDigit(c)) { return; }
            bool[] cache = numCache[c];
            for (int py = 0; py < 5; py++)
            {
                for (int px = 0; px < 3; px++)
                {
                    if (cache[py * 3 + px])
                    {
                        GUI.canvas.DrawPoint(new Pen(color), x + px, y + py);
                    }
                }
            }
        }
        public static bool[] CreateCharCache(char c)
        {
            bool[] cache = new bool[fontX * fontY];
            GUI.canvas.Clear();
            GUI.canvas.DrawChar(c, GUI.font, GUI.WhitePen, 0, 0);
            for (int y = 0; y < fontY; y++)
            {
                for (int x = 0; x < fontX; x++)
                {
                    cache[y * fontX + x] = GUI.canvas.GetPointColor(x, y).R != 0;
                }
            }
            return cache;
        }
        public static void DrawString(string str, Color color, int x, int y)
        {
            foreach (char c in str)
            {
                DrawChar(c, color, x, y);
                x += fontX;
            }
        }

        public static void DrawNumbers(string str, Color color, int x, int y)
        {
            foreach (char c in str)
            {
                DrawNum(c, color, x, y);
                x += 4;
            }
        }

        public static void DrawChar(char c, int color, int[] canvas, int canvasWidth, int x2, int y2)
        {
            int fontY = Font.fontY;
            int fontX = Font.fontX;
            if (c == ' ')
            {
                return;
            }
            bool[] cache = charCache[c];
            for (int py = 0; py < fontY; py++)
            {
                for (int px = 0; px < fontX; px++)
                {
                    if (cache[py * fontX + px])
                    {
                        canvas[(y2 + py) * canvasWidth + (x2 + px)] = color;
                    }
                }
            }
        }
        public static void DrawString(string str, int color, int x2, int y2, int[] bitmap, int canvasWidth)
        {
            int ogX = x2;
            foreach (char c in str)
            {
                if (c == '\n') { y2 += 20; x2 = ogX; continue; }
                DrawChar(c, color, bitmap, canvasWidth, x2, y2);
                x2 += Font.fontX;
            }
        }
        public static void DrawImageAlpha(Bitmap image, int x2, int y2, int[] canvas, int canvasWidth)
        {
            for (int py = 0; py < image.Height; py++)
            {
                for (int px = 0; px < image.Width; px++)
                {
                    int temp = image.rawData[py * image.Width + px];
                    if (temp == 0) { continue; }
                    canvas[(int)((y2 + py) * canvasWidth + (x2 + px))] = temp;
                }
            }
        }
    }
}