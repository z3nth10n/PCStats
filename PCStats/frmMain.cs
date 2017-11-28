using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using static PCStats.InteroperabilityAPI;
using LiteLerped_WF_API.Controls;
using LiteLerped_WF_API.Classes;

namespace PCStats
{
    public partial class frmMain : LerpedForm
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

        private static string testFile = Path.Combine(aPath, "test.txt");

        private static Dictionary<Keys, KeyInfo> keysDB = new Dictionary<Keys, KeyInfo>();
        private static Dictionary<Keys, ToolTip> tooltipKey = new Dictionary<Keys, ToolTip>();
        private static PictureBox keyboard;

        public static ulong maxKeyPress;

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (StartupManager.IsUserAdministrator() && StartupManager.ExistsStartupKey(Program.appName) == null)
                StartupManager.AddApplicationToStartup(Program.appName);

            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.MouseMoveExt += ExtMouseMoved;
            m_GlobalHook.KeyDown += ExtKeyPress;

            /*foreach (var screen in Screen.AllScreens)
            {
                // For each screen, add the screen properties to a list box.
                //listBox1.Items.Add("Device Name: " + screen.DeviceName);
                //listBox1.Items.Add("Bounds: " + screen.Bounds.ToString());
                //listBox1.Items.Add("Type: " + screen.GetType().ToString());
                //listBox1.Items.Add("Working Area: " + screen.WorkingArea.ToString());
                //listBox1.Items.Add("Primary Screen: " + screen.Primary.ToString());
                Console.WriteLine("Name: {0}", screen.DeviceName);
                Console.WriteLine("Bounds: {0}", screen.Bounds.ToString());
                Console.WriteLine("Working area: {0}", screen.WorkingArea.ToString());
            }

            Console.WriteLine("({0}, {1})", ScreenSize.Width, ScreenSize.Height);
            mouse = new int[ScreenSize.Width, ScreenSize.Height];*/

            DEVMODE vDevMode = new DEVMODE();
            int i = 0;
            /*while (EnumDisplaySettings(null, i, ref vDevMode))
            {
                Console.WriteLine("Width:{0} Height:{1} Color:{2} Frequency:{3}",
                                        vDevMode.dmPelsWidth,
                                        vDevMode.dmPelsHeight,
                                        1 << vDevMode.dmBitsPerPel, vDevMode.dmDisplayFrequency
                                    );
                ++i;
            }*/

            Timer1.Tick += Timer1_Tick;
            Timer1.Interval = interval;
            Timer1.Start();

            dpi = GetSystemDpi();

            //Console.WriteLine("Screen phys size: " + new Point(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height));
            Console.WriteLine("Screen phys size: " + GetScreenPhysicalSize());
            Console.WriteLine("DPI: " + dpi);
            Console.WriteLine("Mouse Speed: " + GetMouseSpeed());

            KeysManager.GetPositions();

            counting = true;
        }

        private void FormKey_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Image.FromFile(Path.Combine(aPath, "keyboard.png")), new Rectangle(0, 0, 950, 268));
            foreach (KeyValuePair<Keys, KeyInfo> key in keysDB)
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int) (key.Value.timesPressed / maxKeyPress * 240), Color.Red)), KeysManager.GetRectangle(key.Key));
        }

        /*private void FormMouse_Load(object sender, EventArgs e)
        {
        }*/

        private void FormKey_Load(object sender, EventArgs e)
        {
            //Nothing to do here yet
        }

        private Bitmap GetMouseActivity(Rectangle r, Bitmap bo, int screenID, int[,] mouse, bool writeTestFile = false)
        {
            StringBuilder sb = new StringBuilder();
            Bitmap b = new Bitmap(r.Width, r.Height);

            int maxNum = mouse.Cast<int>().Max(),
                used = mouse.Cast<int>().Count(x => x > 0);

            MessageBox.Show(r.Width + " " + r.Height + "; Max Num = " + maxNum + string.Format("; % ({0} used of {1}) = " + (used / (double) (r.Width * r.Height)).ToString("F10") + "%", used, r.Width * r.Height));

            for (int i = 0; i < r.Width; ++i)
                for (int j = 0; j < r.Height; ++j)
                {
                    b.SetPixel(i, j, Interpolate(bo.GetPixel(i, j), Color.Red, Clamp(mouse[i, j] / (double) maxNum, 0, 1))); //new Color(255, 0, 0, mouse[i, j] * 255 / maxNum)

                    if (writeTestFile)
                        sb.Append(string.Format("{0}x{1}: {2} ({3:F2})", i, j, mouse[i, j], mouse[i, j] / (double) maxNum) + Environment.NewLine);
                }

            if (writeTestFile)
                File.WriteAllText(testFile, sb.ToString());

            b.Save(Path.Combine(aPath, string.Format("map{0}.png", screenID)));

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

            return Color.FromArgb((int) Math.Round(r), (int) Math.Round(g), (int) Math.Round(b));
        }

        private static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        private EventHandler MouseLoad(Rectangle workingArea, int id, int[,] mouse)
        {
            return (sender, e) =>
            {
                Form main = (Form) sender;

                var pb = new PictureBox();
                pb.BackColor = Color.Transparent;

                // Determine the Width and Height of the splash form
                int FormWidth = workingArea.Width,
                    FormHeight = workingArea.Height;

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

                    newPic = GetMouseActivity(new Rectangle(0, 0, workingArea.Width, workingArea.Height), BufferBitmap, id, mouse);
                }

                // Put the final result into the PictureBox_SplashImage which will cover the entire splash form
                pb.Size = new Size(FormWidth, FormHeight);
                pb.Image = newPic;

                main.Location = new Point(workingArea.X, workingArea.Y);
                main.Width = FormWidth;
                main.Height = FormHeight;
                main.WindowState = FormWindowState.Normal;

                main.Controls.Add(pb);

                if (!File.Exists(testFile))
                    File.Create(testFile);
            };
        }

        private void ShowMouseActivity()
        {
            ScreenManager.showingForm = true;
            foreach (KeyValuePair<int, int[,]> screen in ScreenManager.screenHits)
            {
                Form f = new Form();
                f.Load += MouseLoad(ScreenManager.GetScreenArea(screen.Key), screen.Key, screen.Value);
                f.BackColor = Color.White;
                f.TransparencyKey = Color.Transparent;
                f.FormBorderStyle = FormBorderStyle.None;
                f.Bounds = Screen.PrimaryScreen.Bounds;
                f.TopMost = true;

                f.Paint += BackgroundFill;

                f.ShowDialog();
            }
            ScreenManager.showingForm = false;
        }

        private void ShowKeyboardActivity()
        {
            Form f = new Form();
            f.Load += FormKey_Load;
            f.BackColor = Color.White;
            f.Bounds = Screen.PrimaryScreen.Bounds;
            f.TopMost = true;
            f.Size = new Size(950, 300);

            RefreshKeysNum();

            keyboard = new PictureBox();

            keyboard.Image = Properties.Resources.keyboard; //Image.FromFile(Path.Combine(aPath, "keyboard.png"));
            keyboard.Size = new Size(950, 268);
            keyboard.Location = new Point(0, 0);

            keyboard.Paint += Keyboard_Paint;

            f.Controls.Add(keyboard);

            foreach (KeyValuePair<Keys, Rectangle> kp in KeysManager.keyPositions)
                if (kp.Value != Rectangle.Empty)
                {
                    TransparentPanel pnl = new TransparentPanel();

                    pnl.Size = new Size(KeysManager.keySize, KeysManager.keySize);
                    pnl.Location = new Point(kp.Value.X, kp.Value.Y);

                    pnl.Name = kp.Key.ToString();

                    pnl.MouseEnter += Panel_Enter;
                    pnl.MouseLeave += Panel_Leave;

                    keyboard.Controls.Add(pnl);
                }

            f.ShowDialog();
        }

        private void RefreshKeysNum(bool repaint = false)
        {
            if (keysDB.Count > 0)
                maxKeyPress = keysDB.Select(x => x.Value.timesPressed).Max();
            if (maxKeyPress == 0)
                ++maxKeyPress;
            if (repaint)
            {
                try
                {
                    //keyboard.Invalidate();  // request a delayed Repaint by the normal MessageLoop system
                    //keyboard.Update();      // forces Repaint of invalidated area
                    keyboard.Refresh();     // Combines Invalidate() and Update()
                }
                catch
                {
                    Console.WriteLine("Something unexpected ocurred with the keyboard!");
                }
            }
        }

        protected void Panel_Enter(object sender, EventArgs e)
        {
            Keys key = Keys.None;
            if (Enum.TryParse(((TransparentPanel) sender).Name, out key))
            {
                if (!tooltipKey.ContainsKey(key))
                    tooltipKey[key] = new ToolTip();

                tooltipKey[key].ShowAlways = true;
                tooltipKey[key].Show(keysDB.ContainsKey(key) ? keysDB[key].GetToolTip() : "Key wasn't pressed yet.", (TransparentPanel) sender, new Point(KeysManager.keySize, KeysManager.keySize / 2));
            }
        }

        protected void Panel_Leave(object sender, EventArgs e)
        {
            Keys key = Keys.None;
            if (Enum.TryParse(((TransparentPanel) sender).Name, out key) && tooltipKey.ContainsKey(key))
                tooltipKey[key].Hide((TransparentPanel) sender);
        }

        protected void Keyboard_Paint(object sender, PaintEventArgs e)
        {
            PictureBox o = (PictureBox) sender;
            foreach (KeyValuePair<Keys, KeyInfo> key in keysDB)
            {
                Rectangle r = KeysManager.GetRectangle(key.Key);
                if (new Rectangle(0, 0, o.Width, o.Height).Contains(r.X, r.Y))
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int) (key.Value.timesPressed * 225 / maxKeyPress), Color.Red)), r);
            }
        }

        protected void Keyboard_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.Location;
            if (KeysManager.IsKey(mousePos))
            {
                ToolTip tt = new ToolTip();
                IWin32Window win = this;
                tt.Show("String", (PictureBox) sender, mousePos);
            }
        }

        protected void BackgroundFill(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Transparent, ((Form) sender).ClientRectangle);
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
                    ScreenManager.RegisterHit(mousePos);
                }
                catch
                {
                    //Console.WriteLine("Excepción: ({0}, {1})", mousePos.X, mousePos.Y);
                }
            realLastPos = mousePos;
        }

        public void ExtKeyPress(object sender, KeyEventArgs e)
        {
            ++keyPressed;
            if (!keysDB.ContainsKey(e.KeyCode))
                keysDB.Add(e.KeyCode, new KeyInfo(e.KeyCode));
            ++keysDB[e.KeyCode].timesPressed;
            keysDB[e.KeyCode].whenPressed.Add(DateTime.Now);
            if (ModifierKeys != Keys.None)
            {
                if (!keysDB[e.KeyCode].modPressed.Keys.Any(x => x.key == ModifierKeys))
                    keysDB[e.KeyCode].modPressed.Add(new ModifierKey(ModifierKeys), 0);
                keysDB[e.KeyCode].IncrMod(ModifierKeys);
            }
            RefreshKeysNum(true);
        }

        private static double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }
    }

    public class KeyInfo
    {
        public KeyInfo(Keys k)
        {
            key = k;
        }

        public Keys key = Keys.None;
        public ulong timesPressed = 0, modifierKeys = 0;
        public List<DateTime> whenPressed = new List<DateTime>();
        public Dictionary<ModifierKey, ulong> modPressed = new Dictionary<ModifierKey, ulong>();

        public ModifierKey GetMod(Keys k)
        {
            return modPressed.Keys.FirstOrDefault(x => x.key == k);
        }

        public void IncrMod(Keys k)
        {
            ulong v = modPressed.FirstOrDefault(x => x.Key.key == key).Value;
            SetMod(k, ++v);
        }

        public void SetMod(Keys k, ulong value)
        {
            ModifierKey rmk = new ModifierKey(k);
            foreach (ModifierKey mk in modPressed.Keys)
                if (mk.key == k)
                {
                    rmk = mk;
                    break;
                }
            rmk.whenPressed.Add(DateTime.Now);
            modPressed.Remove(GetMod(k));
            modPressed.Add(rmk, value);
        }

        public string GetToolTip()
        { //Create method for the name of the keys
            string t = (DateTime.Now - whenPressed.Last()).ToString(@"d\d\ hh\h\ mm\m\ ss\s").TrimStart(' ', 'd', 'h', 'm', 's', '0');
            return string.Format("[Key '{0}']: {1} times pressed ({4:F2}%)\n{2} mod keys pressed within it.\nPressed {3} ago.", key.ToString(), timesPressed, modifierKeys, t, timesPressed * 1d / frmMain.maxKeyPress);
        }
    }

    public class ModifierKey
    {
        public ModifierKey(Keys k)
        {
            key = k;
        }

        public Keys key = Keys.None;
        public List<DateTime> whenPressed = new List<DateTime>();
    }

    public class TransparentPanel : Panel
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
    }

    public class ScreenManager
    {
        public static Dictionary<int, int[,]> screenHits = new Dictionary<int, int[,]>();

        internal static Dictionary<int, Rectangle> _sc = new Dictionary<int, Rectangle>();
        internal static bool showingForm;

        internal static Dictionary<int, Rectangle> screens
        {
            get
            {
                if (_sc.Count == 0)
                {
                    foreach (Screen screen in Screen.AllScreens)
                        _sc.Add(_sc.Count, screen.WorkingArea);
                }
                return _sc;
            }
        }

        public static void RegisterHit(Point mousePos)
        {
            if (showingForm) return;
            KeyValuePair<int, Rectangle> sc = GetScreen(mousePos);
            //Console.WriteLine(sc.Equals(default(KeyValuePair<int, Rectangle>)));
            int screen = sc.Key;
            Rectangle wArea = sc.Value;
            if (!screenHits.ContainsKey(screen))
            {
                //Console.WriteLine("({0}, {1})", wArea.Width, wArea.Height);
                screenHits.Add(screen, new int[wArea.Width, wArea.Height]);
            }
            //Console.WriteLine("({0}, {1})", wArea.X - mousePos.X, wArea.Y - mousePos.Y);
            //Console.WriteLine("({0}, {1})", wArea.X, wArea.Y);
            ++screenHits[screen][mousePos.X - wArea.X, mousePos.Y - wArea.Y];
        }

        public static Rectangle GetScreenArea(int screenID)
        {
            return screens[screenID];
        }

        internal static KeyValuePair<int, Rectangle> GetScreen(Point mousePos)
        {
            return screens.SingleOrDefault(x => x.Value.Contains(mousePos));
        }
    }

    public class InteroperabilityAPI
    {
        #region "System DPI"

        private enum DeviceCaps
        {
            /// <summary>
            /// Device driver version
            /// </summary>
            DRIVERVERSION = 0,

            /// <summary>
            /// Device classification
            /// </summary>
            TECHNOLOGY = 2,

            /// <summary>
            /// Horizontal size in millimeters
            /// </summary>
            HORZSIZE = 4,

            /// <summary>
            /// Vertical size in millimeters
            /// </summary>
            VERTSIZE = 6,

            /// <summary>
            /// Horizontal width in pixels
            /// </summary>
            HORZRES = 8,

            /// <summary>
            /// Vertical height in pixels
            /// </summary>
            VERTRES = 10,

            /// <summary>
            /// Number of bits per pixel
            /// </summary>
            BITSPIXEL = 12,

            /// <summary>
            /// Number of planes
            /// </summary>
            PLANES = 14,

            /// <summary>
            /// Number of brushes the device has
            /// </summary>
            NUMBRUSHES = 16,

            /// <summary>
            /// Number of pens the device has
            /// </summary>
            NUMPENS = 18,

            /// <summary>
            /// Number of markers the device has
            /// </summary>
            NUMMARKERS = 20,

            /// <summary>
            /// Number of fonts the device has
            /// </summary>
            NUMFONTS = 22,

            /// <summary>
            /// Number of colors the device supports
            /// </summary>
            NUMCOLORS = 24,

            /// <summary>
            /// Size required for device descriptor
            /// </summary>
            PDEVICESIZE = 26,

            /// <summary>
            /// Curve capabilities
            /// </summary>
            CURVECAPS = 28,

            /// <summary>
            /// Line capabilities
            /// </summary>
            LINECAPS = 30,

            /// <summary>
            /// Polygonal capabilities
            /// </summary>
            POLYGONALCAPS = 32,

            /// <summary>
            /// Text capabilities
            /// </summary>
            TEXTCAPS = 34,

            /// <summary>
            /// Clipping capabilities
            /// </summary>
            CLIPCAPS = 36,

            /// <summary>
            /// Bitblt capabilities
            /// </summary>
            RASTERCAPS = 38,

            /// <summary>
            /// Length of the X leg
            /// </summary>
            ASPECTX = 40,

            /// <summary>
            /// Length of the Y leg
            /// </summary>
            ASPECTY = 42,

            /// <summary>
            /// Length of the hypotenuse
            /// </summary>
            ASPECTXY = 44,

            /// <summary>
            /// Shading and Blending caps
            /// </summary>
            SHADEBLENDCAPS = 45,

            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LOGPIXELSX = 88,

            /// <summary>
            /// Logical pixels inch in Y
            /// </summary>
            LOGPIXELSY = 90,

            /// <summary>
            /// Number of entries in physical palette
            /// </summary>
            SIZEPALETTE = 104,

            /// <summary>
            /// Number of reserved entries in palette
            /// </summary>
            NUMRESERVED = 106,

            /// <summary>
            /// Actual color resolution
            /// </summary>
            COLORRES = 108,

            // Printing related DeviceCaps. These replace the appropriate Escapes
            /// <summary>
            /// Physical Width in device units
            /// </summary>
            PHYSICALWIDTH = 110,

            /// <summary>
            /// Physical Height in device units
            /// </summary>
            PHYSICALHEIGHT = 111,

            /// <summary>
            /// Physical Printable Area x margin
            /// </summary>
            PHYSICALOFFSETX = 112,

            /// <summary>
            /// Physical Printable Area y margin
            /// </summary>
            PHYSICALOFFSETY = 113,

            /// <summary>
            /// Scaling factor x
            /// </summary>
            SCALINGFACTORX = 114,

            /// <summary>
            /// Scaling factor y
            /// </summary>
            SCALINGFACTORY = 115,

            /// <summary>
            /// Current vertical refresh rate of the display device (for displays only) in Hz
            /// </summary>
            VREFRESH = 116,

            /// <summary>
            /// Vertical height of entire desktop in pixels
            /// </summary>
            DESKTOPVERTRES = 117,

            /// <summary>
            /// Horizontal width of entire desktop in pixels
            /// </summary>
            DESKTOPHORZRES = 118,

            /// <summary>
            /// Preferred blt alignment
            /// </summary>
            BLTALIGNMENT = 119
        }

        /// <summary>
        /// Retrieves device-specific information for the specified device.
        /// </summary>
        /// <param name="hdc">A handle to the DC.</param>
        /// <param name="nIndex">The item to be returned.</param>
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, DeviceCaps nIndex);

        public static int GetSystemDpi()
        {
            using (Graphics screen = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr hdc = screen.GetHdc();

                int virtualWidth = GetDeviceCaps(hdc, DeviceCaps.HORZRES);
                int physicalWidth = GetDeviceCaps(hdc, DeviceCaps.DESKTOPHORZRES);
                screen.ReleaseHdc(hdc);

                return (int) (96f * physicalWidth / virtualWidth);
            }
        }

        public static Point GetScreenPhysicalSize()
        {
            using (Graphics screen = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr hdc = screen.GetHdc();

                int physWidth = GetDeviceCaps(hdc, DeviceCaps.HORZSIZE),
                    physHeight = GetDeviceCaps(hdc, DeviceCaps.VERTSIZE);
                screen.ReleaseHdc(hdc);

                return new Point(physWidth, physHeight);
            }
        }

        #endregion "System DPI"

        #region "Monitor Refresh Rate"

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(
      string deviceName, int modeNum, ref DEVMODE devMode);

        private const int ENUM_CURRENT_SETTINGS = -1;

        private const int ENUM_REGISTRY_SETTINGS = -2;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;

            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;

            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        #endregion "Monitor Refresh Rate"

        #region "Mouse & Keyboard Hooks / Screenshots"

        //Hooks
        public const uint SPI_GETMOUSESPEED = 0x0070;

        public const int SRCCOPY = 0xcc0020;

        [DllImport("User32.dll")]
        public static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            IntPtr pvParam,
            UInt32 fWinIni);

        public static unsafe int GetMouseSpeed()
        {
            int speed;
            SystemParametersInfo(
                SPI_GETMOUSESPEED,
                0,
                new IntPtr(&speed),
                0);
            return speed;
        }

        [DllImport("user32", EntryPoint = "GetDC", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDC(IntPtr hwnd);

        [DllImport("user32", EntryPoint = "ReleaseDC", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hwnd, int hdc);

        [DllImport("gdi32", EntryPoint = "BitBlt", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(int hDestDC, int x, int y, int nWidth, int nHeight, int hSrcDC, int xSrc, int ySrc, int dwRop);

        #endregion "Mouse & Keyboard Hooks / Screenshots"
    }

    public class StatsData
    {
    }
}