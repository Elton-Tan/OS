using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using TangerineOS.Commands;
using TangerineOS.GUI;
using System.Drawing;
using System.Linq;
using System.Dynamic;

namespace TangerineOS
{
    public class Date
    {
        public static string CurrentDate(bool DisplayWeekday, bool DisplayYear)
        {
            string Weekday = "";
            string Month = "";
            if (DisplayWeekday)
            {
                switch (RTC.DayOfTheWeek) //create array of days in a week
                {
                    case 1:
                        Weekday = "Monday, ";
                        break;
                    case 2:
                        Weekday = "Tuesday, ";
                        break;
                    case 3:
                        Weekday = "Wednesday, ";
                        break;
                    case 4:
                        Weekday = "Thursday, ";
                        break;
                    case 5:
                        Weekday = "Friday, ";
                        break;
                    case 6:
                        Weekday = "Saturday, ";
                        break;
                    case 7:
                        Weekday = "Sunday, ";
                        break;
                }
            }
            switch (RTC.Month) //create array of month
            {
                case 1:
                    Month = "January";
                    break;
                case 2:
                    Month = "February";
                    break;
                case 3:
                    Month = "March";
                    break;
                case 4:
                    Month = "April";
                    break;
                case 5:
                    Month = "May";
                    break;
                case 6:
                    Month = "June";
                    break;
                case 7:
                    Month = "July";
                    break;
                case 8:
                    Month = "August";
                    break;
                case 9:
                    Month = "September";
                    break;
                case 10:
                    Month = "October";
                    break;
                case 11:
                    Month = "November";
                    break;
                case 12:
                    Month = "December";
                    break;

            }
            if (DisplayYear)
            {
                if (DisplayWeekday)
                {
                    return Weekday + RTC.DayOfTheMonth + "." + RTC.Month + ".20" + RTC.Year;
                }
                else
                {
                    return RTC.DayOfTheMonth + "." + RTC.Month + ".20" + RTC.Year;
                }
            }
            else
            {
                if (DisplayWeekday)
                {
                    return Weekday + RTC.DayOfTheMonth + " " + Month;
                }
                else
                {
                    return RTC.DayOfTheMonth + " " + Month;
                }
            }
        }
        public static string CurrentTime(bool DisplaySeconds)
        {
            string hour = RTC.Hour.ToString();// convert to string
            if (hour.Length == 1) { hour = "0" + hour; } //get the hour
            string min = RTC.Minute.ToString(); //convert to string
            if (min.Length == 1) { min = "0" + min; } //get the min
            if (DisplaySeconds)
            {
                string sec = RTC.Second.ToString();
                if (sec.Length == 1) { sec = "0" + sec; }
                return hour + ":" + min + ":" + sec; //return the whole string
            }
            else
            {
                return hour + ":" + min;
            }
        }
        public static string CurrentSecond()
        {
            string sec = RTC.Second.ToString();
            if (sec.Length == 1) { sec = "0" + sec; }
            return sec;
        }
    }
}
namespace TangerineOS.GUI
{
    internal class Date : Window
    {
        internal Date() : base("Date", 220, 50, Icons.program, Priority.High) { }
        internal override int Start()
        {
            MemoryOperations.Fill(appCanvas.rawData, Color.Black.ToArgb());
            DrawString(TangerineOS.Date.CurrentTime(true), Color.LimeGreen.ToArgb(), Color.Black.ToArgb(), x - (x / 2) - 35, 10);
            DrawString(TangerineOS.Date.CurrentDate(true, true), Color.White.ToArgb(), Color.Black.ToArgb(), x - (x / 2) - 100, 25);
            return 0;
        }
        internal override void Update()
        {
            DrawString(TangerineOS.Date.CurrentTime(true), Color.LimeGreen.ToArgb(), Color.Black.ToArgb(), x - (x / 2) - 35, 10);
            DrawString(TangerineOS.Date.CurrentDate(true, true), Color.White.ToArgb(), Color.Black.ToArgb(), x - (x / 2) - 100, 25);
        }

        internal override int Stop() { return 0; }
    }
}
namespace TangerineOS.Commands
{
    internal class Date : CommandsTree
    {
        internal Date() : base
            ("Date", "Displays the system time and date.",
            new Command[] {
            new Command(new string[] { "time", "date" }, "Displays the current system time and date.", new string[] {"/t - display only time","/d - display only date"})
            })
        {
        }
        internal override int Execute(string[] args, CommandShell shell)
        {
            if (args[0] == "time" || args[0] == "date")
            {
                bool noDate = false;
                bool noTime = false;
                foreach (string arg in args.Skip(1))
                {
                    if (arg == "/t")
                    {
                        noDate = true;
                    }
                    else if (arg == "/d")
                    {
                        noTime = true;
                    }
                }
                string str = "";
                if (!noDate) { str += TangerineOS.Date.CurrentDate(true, true) + " "; }
                if (!noTime) { str += TangerineOS.Date.CurrentTime(true); }
                shell.print = str;
                return 0;
            }
            return 1;
        }
    }
}