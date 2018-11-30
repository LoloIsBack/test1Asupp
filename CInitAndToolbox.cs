using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7SiemensManager
{
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    static class LoloExtension
    {
        public static bool LoloRegex(this string text, string pattern) { return Regex.IsMatch(text, pattern); } //lolok1AC{TIPS: les extensions permettent la surcharge des STRING par exemple}
        public static string LoloRegex(this string text, string pattern, string replace) { return Regex.Replace(text, pattern, replace); } //lolok1AC{TIPS: les extensions permettent la surcharge des STRING par exemple}
        public static string LoloRegexMatch(this string text, string pattern) { return Regex.Match(text, pattern).ToString(); } //lolok1AC{TIPS: les extensions permettent la surcharge des STRING par exemple}
        public static MatchCollection LoloRegexMatches(this string text, string pattern) { return Regex.Matches(text, pattern); } //lolok1AC{TIPS: les extensions permettent la surcharge des STRING par exemple}
        public static string LoloCapsule(this string text, string format) { return String.Format(format, text); }
        public static string LoloEncrypt(this string text) { return Convert.ToBase64String(Encoding.Default.GetBytes(text)); }
    }
    static class LoloVar
    {
    }

    public static class Tb //lolokey{toolbox} 
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr FindWindow(IntPtr classname, string title); // extern method: FindWindow

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern void MoveWindow(IntPtr hwnd, int X, int Y,
            int nWidth, int nHeight, bool rePaint); // extern method: MoveWindow

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool GetWindowRect
            (IntPtr hwnd, out System.Drawing.Rectangle rect); // extern method: GetWindowRect

        //https://www.codeproject.com/Tips/472294/Position-a-Windows-Forms-MessageBox-in-Csharp
        public static void FindAndMoveMsgBox(int x, int y, bool repaint, string title)
        { 
            System.Threading.Thread thr = new System.Threading.Thread(() => // create a new thread
            {
                IntPtr msgBox = IntPtr.Zero;
                while ((msgBox = FindWindow(IntPtr.Zero, title)) == IntPtr.Zero) ;
                // after the while loop, msgBox is the handle of your MessageBox
                System.Drawing.Rectangle r = new System.Drawing.Rectangle();
                GetWindowRect(msgBox, out r); // Gets the rectangle of the message box
                MoveWindow(msgBox /* handle of the message box */, x, y,
                   r.Width - r.X /* width of originally message box */,
                   r.Height - r.Y /* height of originally message box */,
                   repaint /* if true, the message box repaints */);
            });
            thr.Start(); // starts the thread
        }

        public static string ClipboardGetModal(string message = "Copy cliboard, please", string title = "")
        {
			var keyname = "Clipboar";
            var keypath = @"Software\Sidel\loloS7Manager";
            var clipboard = "";
            var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keypath, true);
            if (reg == null) { reg = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(keypath); }
            if (reg.GetValue(keyname) != null)
            {
                var msg = MessageBox.Show(new Form { TopMost = true }, "Use last data?", title, MessageBoxButtons.YesNo);
                if (msg == DialogResult.Yes) { clipboard = reg.GetValue(keyname).ToString(); }
            }

            if (clipboard == "")
            {          
            while (true)
            {
                if (true)
                {
                    Clipboard.Clear();
                    MessageBox.Show(new Form() { TopMost = true }, message);
                    
                    if (Clipboard.GetText() != "")
                    {
                        clipboard = Clipboard.GetText();
                        Console.WriteLine("While loop break");
                        break;
                    }
                    else { Console.WriteLine("!!!No data inside clipboard, so relooping!!!"); }
                }
            }
            reg.SetValue(keyname, clipboard);
            }
            return clipboard;
        }
        public static List<int> Range(string toConvert = "202, 209, 245     - 249, 301") //lolokey{range 201M1 202M1.... listing} 
        {
            List<int> mList = new List<int>();
            var myString = toConvert.LoloRegex(@"\s+", "");
            var matches = Regex.Matches(myString, @"[^,;]+");
            foreach (Match match in matches)
            {
                foreach (Capture capture in match.Captures)
                {
                    Console.WriteLine("Index={0}, Value={1}", capture.Index, capture.Value);
                    var mVals = Regex.Matches(capture.Value, @"\d+");
                    if (mVals.Count > 0)
                    {
                        var start = Convert.ToInt16(mVals[0].Value); var stop = start;
                        if (mVals.Count > 1) { stop = Convert.ToInt16(mVals[1].Value); }
                        for (int i = start; i <= stop; i++) { mList.Add(i); }
                    }
                }
            }
            return mList;
        }

        public static string Range(List<int> _values, string prefix = "", string suffix = "M1")
        {
            StringBuilder Range = new StringBuilder();
            _values.Sort();
            var low = _values[0];
            var up = _values[0];
            // Range.Append($"{_values[0]}");
            for (int ii = 1; ii < _values.Count; ii++)
            {
                if (_values[ii] != up + 1)
                { if (up > low)
                        Range.Append($"{low}-{up},");
                    else
                        Range.Append($"{low}, ");
                     low = _values[ii];
                     up = _values[ii];
                }
                up = _values[ii];
            }
            if (up > low)
                Range.Append($"{low}-{up}");
            else
                Range.Append($"{low}");
            Range.Append($", ");
            var ret= Range.ToString();
            //prefix = "Mot"; suffix = "";
            if (prefix != "" && suffix != "")
                ret = Regex.Replace(ret,@"(\d+)(\D+)",$"{prefix}$1{suffix}$2");
            else if(prefix == "")
                ret = Regex.Replace(ret, @"(\d+)([, ]+)", $"$1{suffix}$2");
            else if (suffix == "")
                ret = Regex.Replace(ret, @"([, ]+|\A *)(\d+)", $"$1{prefix}$2");


            return Regex.Replace(ret, @"[, ]+$", "");
        }

        public static string RegexPattern(List<int> myList, string prefix = "", string suffix = "M1")
        {
            StringBuilder myStr = new StringBuilder();
            myStr.Append("(?i)(");
            foreach (var e in myList)
            {
                myStr.Append(prefix).Append("0*").Append(e).Append(suffix).Append("|");
            }
            myStr.Append("xxloloisbackxx)");
            return myStr.ToString();
        }
        public static string RegexPattern(List<string> _list)
        {
            StringBuilder myStr = new StringBuilder();
            myStr.Append("(?i)(");
            foreach (var name in _list)
            {
                myStr.Append(name).Append("|");
            }
            myStr.Append("xxloloisbackxx)");
            return myStr.ToString();
        }
    }
    public class Gentool
    {
        public static Gentool MyProject = new Gentool();
        private readonly List<Motor> Motors = new List<Motor>();
        private class Motor
        {
            public int id;
            public string Name = "";
            public string Comment = "";
        }

        public void MotorAdd(int _name)
        {
            this.MotorAdd(_name.ToString());
        }
        public void MotorAdd(string _name, string _comment = "")
        {
            var motor = new Motor
            {
                id = Convert.ToInt16(_name.LoloRegexMatch(@"\d+")),
                Name = _name
            };
            if (motor.Name.LoloRegex(@"^\d+$")) { motor.Name = motor.Name.LoloCapsule("{0}M1"); }
            motor.Comment = _comment;
            if (motor.Comment == "") { motor.Comment = motor.Name.LoloCapsule("Motor {0}"); }
            this.Motors.Add(motor);
        }
        public List<string> MotorsList() { return ListMotorName(this.Motors); }
        public string MotorsRegexPattern() { return Tb.RegexPattern(ListMotorName(this.Motors)); }

        private static List<string> ListMotorName(List<Motor> _oMotors)
        {
            List<string> listNameMotors = new List<string>();
            foreach (var motor in _oMotors)
            {
                listNameMotors.Add(motor.Name);
            }
            return listNameMotors;
        }
            public int Infos_MotorsTotal() { return Gentool.MyProject.Motors.Count; }

        public static class Macros
        { public static void InitProject()
            {
                var sTmp = "202,203,204,205,206,207,208,209,211,212,213,214,215,216,217,218,219,221,222,223,226,227,229,230,231,232,233,241,243,244,245,246,249,253,254,255,256,261,262,263,265,266,267,269,272,273,274,275,276,278,279,281,282,283,284,285,286,287,288,289,290,291,293,294,295,296,301,302,303,304,305,306,307,311,312,313,317,318,319,321,322,323,331,332,333,334,335,336,337,338,341,342,343,151,152,153,154,155,156";
                var Motors = S7SiemensManager.Tb.Range(sTmp);
                MessageBox.Show(S7SiemensManager.Tb.Range(Motors));
                foreach (var motor in Motors)
                {
                    Gentool.MyProject.MotorAdd(motor);
                }
                MessageBox.Show(Gentool.MyProject.MotorsRegexPattern());
            } }
    }

 



 }
