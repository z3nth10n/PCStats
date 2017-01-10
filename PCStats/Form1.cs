using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace PCStats
{
    public partial class Form1 : Form
    {
        private static IKeyboardMouseEvents m_GlobalHook;
        private static long keyPressed, total, overticks, cheatedTicks, lastcTick;
        private static Point mousePos, lastPos, realLastPos;
        private const int interval = 20;
        private const double warnTime = .5f;
        private static double totalDist, dpi;
        private static Timer Timer1 = new Timer();
        private static bool counting, isMoving;
        private static string aPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static int[,] mouse;
        private static string testFile = Path.Combine(aPath, "test.txt");
        private static Dictionary<Keys, ulong> keysDB = new Dictionary<Keys, ulong>();
        private static ulong maxKeyPress;

        private static Rectangle ScreenSize
        {
            get
            {
                return Screen.PrimaryScreen.WorkingArea;
            }
        }

        //Hooks
        public const uint SPI_GETMOUSESPEED = 0x0070;
        public const int SRCCOPY = 0xcc0020;

        [DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            IntPtr pvParam,
            UInt32 fWinIni);

        [DllImport("user32", EntryPoint = "GetDC", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GetDC(IntPtr hwnd);
        [DllImport("user32", EntryPoint = "ReleaseDC", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int ReleaseDC(IntPtr hwnd, int hdc);
        [DllImport("gdi32", EntryPoint = "BitBlt", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int BitBlt(int hDestDC, int x, int y, int nWidth, int nHeight, int hSrcDC, int xSrc, int ySrc, int dwRop);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.MouseMoveExt += ExtMouseMoved;
            //m_GlobalHook.KeyPress += ExtKeyPress;
            m_GlobalHook.KeyDown += ExtKeyPress;

            mouse = new int[ScreenSize.Width, ScreenSize.Height];

            Timer1.Tick += Timer1_Tick;
            Timer1.Interval = interval;
            Timer1.Start();

            using (Graphics g = CreateGraphics())
                dpi = g.DpiX;

            Console.WriteLine(dpi);
            Console.WriteLine(GetMouseSpeed());

            counting = true;
        }

        private void FormKey_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Image.FromFile(Path.Combine(aPath, "keyboard.png")), new Rectangle(0, 0, 950, 268));
            //e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(128, Color.Black)), new Rectangle(0, 0, 150, 150));
            foreach (KeyValuePair<Keys, ulong> key in keysDB)
            { //if(keysDB.ContainsKey(key))
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int)(key.Value / maxKeyPress * 240), Color.Red)), KeysManager.GetRectangle(key.Key));
                //MessageBox.Show(key.Key.ToString() + ": " + KeysManager.GetRectangle(key.Key).ToString() + " (Used: " + key.Value + " times)");
            }
        }

        private void FormMouse_Load(object sender, EventArgs e)
        {
            Form main = (Form)sender;

            Rectangle position = new Rectangle(0, 0, ScreenSize.Width, ScreenSize.Height);

            var pb = new PictureBox();
            //pb.Image = GetMouseActivity(position); //Image.FromFile(Path.Combine(aPath, "test.png"));
            //pb.Size = new Size(ScreenSize.Width, ScreenSize.Height);
            pb.BackColor = Color.Transparent;

            // Determine the Width and Height of the splash form
            int FormWidth = ScreenSize.Width,
                FormHeight = ScreenSize.Height;

            // Create a bitmap buffer to draw things into
            Bitmap BufferBitmap = new Bitmap(FormWidth, FormHeight), newPic;
            using (Graphics BufferGraphics = Graphics.FromImage(BufferBitmap))
            {
                // Get a screenshot of the desktop area where the splash form will show
                int DesktopDC = GetDC(default(IntPtr));
                IntPtr BufferGraphicsDC = BufferGraphics.GetHdc();
                BitBlt(BufferGraphicsDC.ToInt32(), 0, 0, FormWidth, FormHeight, DesktopDC, 0, 0, SRCCOPY);
                ReleaseDC(default(IntPtr), DesktopDC);
                BufferGraphics.ReleaseHdc(BufferGraphicsDC);

                newPic = GetMouseActivity(position, BufferBitmap);

                // Draw the PNG image over the desktop screenshot
                //BufferGraphics.DrawImage(pb.Image, position.X, position.Y, position.Width, position.Height);

                // Draw some text -- this is where some product license info could be drawn on the splash picture
                //using (SolidBrush TextBrush = new SolidBrush(ForeColor))
                //    BufferGraphics.DrawString(Application.ProductVersion, Font, TextBrush, 25, 40);
            }

            // Put the final result into the PictureBox_SplashImage which will cover the entire splash form
            pb.Size = new Size(FormWidth, FormHeight);
            pb.Image = newPic;

            main.Width = FormWidth;
            main.Height = FormHeight;
            main.WindowState = FormWindowState.Normal;

            main.Controls.Add(pb);

            if (!File.Exists(testFile))
                File.Create(testFile);
        }

        private void FormKey_Load(object sender, EventArgs e)
        {
            Form main = (Form)sender;

            /*var pb = new PictureBox();
            
            pb.Size = new Size(main.Size.Width, main.Size.Height);
            pb.Image = Image.FromFile(Path.Combine(aPath, "keyboard.png"));

            main.Controls.Add(pb);*/
        }

        private Bitmap GetMouseActivity(Rectangle r, Bitmap bo)
        {
            StringBuilder sb = new StringBuilder();
            Bitmap b = new Bitmap(r.Width, r.Height);
            int maxNum = mouse.Cast<int>().Max(),
                used = mouse.Cast<int>().Count(x => x > 0);
            MessageBox.Show(r.Width + " " + r.Height + "; Max Num = " + maxNum + string.Format("; % ({0} used of {1}) = " + (used / (double)(r.Width * r.Height)).ToString("F10") + "%", used, r.Width * r.Height));
            for (int i = 0; i < r.Width; ++i)
                for (int j = 0; j < r.Height; ++j)
                {
                    b.SetPixel(i, j, Interpolate(bo.GetPixel(i, j), Color.Red, Clamp(mouse[i, j] / (double)maxNum, 0, 1))); //new Color(255, 0, 0, mouse[i, j] * 255 / maxNum)
                    //b.SetPixel(i, j, Color.FromArgb((int)Clamp((int)Math.Round(mouse[i, j] * 255 / (double)maxNum), 0, 255), Color.Red));
                    sb.Append(string.Format("{0}x{1}: {2} ({3:F2})", i, j, mouse[i, j], mouse[i, j] / (double)maxNum) + Environment.NewLine);
                }
            File.WriteAllText(testFile, sb.ToString());
            b.Save(Path.Combine(aPath, "map.png"));
            return b;
        }

        public static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private static Color Interpolate(Color color1, Color color2, double fraction)
        {
            double r = Lerp(color1.R, color2.R, fraction),
                   g = Lerp(color1.G, color2.G, fraction),
                   b = Lerp(color1.B, color2.B, fraction);
            return Color.FromArgb((int)Math.Round(r), (int)Math.Round(g), (int)Math.Round(b));
        }

        private static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        private void ShowMouseActivity()
        {
            Form f = new Form();
            f.Load += FormMouse_Load;
            f.BackColor = Color.White;
            f.TransparencyKey = Color.Transparent;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Bounds = Screen.PrimaryScreen.Bounds;
            f.TopMost = true;

            f.Paint += BackgroundFill;

            f.ShowDialog();
        }

        private void ShowKeyboardActivity()
        {
            Form f = new Form();
            f.Load += FormKey_Load;
            f.BackColor = Color.White;
            //f.TransparencyKey = Color.Transparent;
            //f.FormBorderStyle = FormBorderStyle.None;
            f.Bounds = Screen.PrimaryScreen.Bounds;
            f.TopMost = true;
            f.Size = new Size(950, 300);

            f.Paint += FormKey_Paint;

            if(keysDB.Count > 0)
                maxKeyPress = keysDB.Values.Max();
            if (maxKeyPress == 0)
                ++maxKeyPress;

            //foreach (KeyValuePair<Keys, ulong> key in keysDB)
            //    MessageBox.Show(key.Key.ToString() + ": " + KeysManager.GetRectangle(key.Key).ToString() + " (Used: " + key.Value + " times)");

            f.ShowDialog();
        }

        protected void BackgroundFill(object o, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Transparent, ((Form)o).ClientRectangle);
        }

        private void menu1_Click(object sender, EventArgs e)
        {
            ShowMouseActivity();
        }

        private void menu2_Click(object sender, EventArgs e)
        {
            ShowKeyboardActivity();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            ++total;
            Point c = mousePos;
            double xyDist = Math.Sqrt(Math.Pow(lastPos.X - c.X, 2) + Math.Pow(lastPos.Y - c.Y, 2));
            lastPos = c;
            totalDist += xyDist;
            label1.Text = string.Format("Moving at: {0:F2}; Distance: {1:F2} pixels", xyDist, totalDist); //"Moving at: " + xyDist + "; Distance = " + totalDist + " pixels";
            double avgDist = totalDist / total,
                   cm = totalDist / dpi * 2.54d;
            label2.Text = string.Format("Average move: {0:F2}; Seconds: {1:F4} s", avgDist, total / (1000d / interval)); //"Average move: " + avgDist.ToString("F2");
            label3.Text = string.Format("Distance (cm): {0:F2} cm; (m): {1:F2} m", cm, cm / 100d);
            if (xyDist > 100)
            {
                ++overticks;
                lastcTick = total;
            }
            else
            {
                if ((total - lastcTick) / (1000d / interval) > warnTime && overticks > 0)
                    overticks = 0;
            }
            if (overticks / (1000d / interval) > 1)
                counting = false;
            else
            {
                if (!counting)
                    counting = true;
            }
            if (!counting)
                ++cheatedTicks;
            label4.Text = (counting ? "Counting!" : "Not counting!") + " " + string.Format(" {0} cheated ticks!", cheatedTicks);
        }

        public void ExtMouseMoved(object sender, MouseEventExtArgs e)
        {
            mousePos = e.Location;
            isMoving = mousePos != realLastPos;
            if (isMoving)
                try
                {
                    ++mouse[mousePos.X, mousePos.Y];
                }
                catch (Exception ex)
                {

                }
            realLastPos = mousePos;
        }

        /*public void ExtKeyPress(object sender, KeyPressEventArgs e)
        {
            ++keyPressed;
            //label2.Text = string.Format("Key pressed: {0}, Last Key Pressed: {1}", keyPressed, e.KeyChar);
        }*/

        public void ExtKeyPress(object sender, KeyEventArgs e)
        {
            ++keyPressed;
            if (!keysDB.ContainsKey(e.KeyCode))
                keysDB.Add(e.KeyCode, 0);
            ++keysDB[e.KeyCode];
        }

        private static double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        private static unsafe int GetMouseSpeed()
        {
            int speed;
            SystemParametersInfo(
                SPI_GETMOUSESPEED,
                0,
                new IntPtr(&speed),
                0);
            return speed;
        }
    }
}