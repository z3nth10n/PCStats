using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PCStats
{
    internal class KeysManager
    {
        //public Keys k = Keys.None;
        public static Dictionary<Keys, Rectangle> keyPositions = new Dictionary<Keys, Rectangle>();

        public const int keySize = 33;

        public static void GetPositions()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                if (!keyPositions.ContainsKey(key))
                    keyPositions.Add(key, GetRectangle(key));
        }

        public static bool IsKey(Point p)
        {
            return keyPositions.Values.Any(x => x.Contains(p.X, p.Y));
        }

        public static Keys GetKey(Rectangle r)
        {
            return keyPositions.ContainsValue(r) ? keyPositions.FirstOrDefault(x => x.Value == r).Key : Keys.None;
        }

        public static Rectangle GetRectangle(Keys k)
        {
            Keys[] firstRow = new Keys[] { Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P },
                   secondRow = new Keys[] { Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L },
                   thirdRow = new Keys[] { Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N, Keys.M };
            int startY = 101, startX = 77; //Only for this stage
            if (firstRow.Contains(k))
                startX += 40 * Array.IndexOf(firstRow, k);
            else if (secondRow.Contains(k))
            {
                startY += 40;
                startX += 40 * Array.IndexOf(secondRow, k) + 10;
            }
            else if (thirdRow.Contains(k))
            {
                startY += 79;
                startX += 40 * Array.IndexOf(thirdRow, k) + 29;
            }
            return firstRow.Union(secondRow).Union(thirdRow).Contains(k) ? new Rectangle(startX, startY, keySize, keySize) : new Rectangle(0, 0, 0, 0);
        }

        public static Rectangle GetRectangleBeta(Keys k)
        {
            switch (k)
            {
                case Keys.A:

                    break;

                case Keys.Add:

                    break;

                case Keys.Alt:

                    break;

                case Keys.Apps:

                    break;

                case Keys.Attn:

                    break;

                case Keys.B:

                    break;

                case Keys.Back:

                    break;

                case Keys.BrowserBack:

                    break;

                case Keys.BrowserFavorites:

                    break;

                case Keys.BrowserForward:

                    break;

                case Keys.BrowserHome:

                    break;

                case Keys.BrowserRefresh:

                    break;

                case Keys.BrowserSearch:

                    break;

                case Keys.BrowserStop:

                    break;

                case Keys.C:

                    break;

                case Keys.Cancel:

                    break;

                case Keys.Capital:

                    break;
                /*case Keys.CapsLock: //Multiple cases

                    break;*/
                case Keys.Clear:

                    break;

                case Keys.Control:

                    break;

                case Keys.ControlKey:

                    break;

                case Keys.Crsel:

                    break;

                case Keys.D:

                    break;

                case Keys.D0:

                    break;

                case Keys.D1:

                    break;

                case Keys.D2:

                    break;

                case Keys.D3:

                    break;

                case Keys.D4:

                    break;

                case Keys.D5:

                    break;

                case Keys.D6:

                    break;

                case Keys.D7:

                    break;

                case Keys.D8:

                    break;

                case Keys.D9:

                    break;

                case Keys.Decimal:

                    break;

                case Keys.Delete:

                    break;

                case Keys.Divide:

                    break;

                case Keys.Down:

                    break;

                case Keys.E:

                    break;

                case Keys.End:

                    break;

                case Keys.Enter:

                    break;

                case Keys.EraseEof:

                    break;

                case Keys.Escape:

                    break;

                case Keys.Execute:

                    break;

                case Keys.Exsel:

                    break;

                case Keys.F:

                    break;

                case Keys.F1:

                    break;

                case Keys.F10:

                    break;

                case Keys.F11:

                    break;

                case Keys.F12:

                    break;

                case Keys.F13:

                    break;

                case Keys.F14:

                    break;

                case Keys.F15:

                    break;

                case Keys.F16:

                    break;

                case Keys.F17:

                    break;

                case Keys.F18:

                    break;

                case Keys.F19:

                    break;

                case Keys.F2:

                    break;

                case Keys.F20:

                    break;

                case Keys.F21:

                    break;

                case Keys.F22:

                    break;

                case Keys.F23:

                    break;

                case Keys.F24:

                    break;

                case Keys.F3:

                    break;

                case Keys.F4:

                    break;

                case Keys.F5:

                    break;

                case Keys.F6:

                    break;

                case Keys.F7:

                    break;

                case Keys.F8:

                    break;

                case Keys.F9:

                    break;

                case Keys.FinalMode:

                    break;

                case Keys.G:

                    break;

                case Keys.H:

                    break;

                case Keys.HanguelMode:

                    break;
                /*case Keys.HangulMode: //Multiple cases

                    break;*/
                case Keys.HanjaMode:

                    break;

                case Keys.Help:

                    break;

                case Keys.Home:

                    break;

                case Keys.I:

                    break;

                case Keys.IMEAccept:

                    break;
                /*case Keys.IMEAceept: //Multiple cases

                    break;*/
                case Keys.IMEConvert:

                    break;

                case Keys.IMEModeChange:

                    break;

                case Keys.IMENonconvert:

                    break;

                case Keys.Insert:

                    break;

                case Keys.J:

                    break;

                case Keys.JunjaMode:

                    break;

                case Keys.K:

                    break;
                /*case Keys.KanaMode: //Multiple cases

                    break;

                case Keys.KanjiMode: //Multiple cases

                    break;*/
                case Keys.KeyCode:

                    break;

                case Keys.L:

                    break;

                case Keys.LaunchApplication1:

                    break;

                case Keys.LaunchApplication2:

                    break;

                case Keys.LaunchMail:

                    break;

                case Keys.LButton:

                    break;

                case Keys.LControlKey:

                    break;

                case Keys.Left:

                    break;

                case Keys.LineFeed:

                    break;

                case Keys.LMenu:

                    break;

                case Keys.LShiftKey:

                    break;

                case Keys.LWin:

                    break;

                case Keys.M:

                    break;

                case Keys.MButton:

                    break;

                case Keys.MediaNextTrack:

                    break;

                case Keys.MediaPlayPause:

                    break;

                case Keys.MediaPreviousTrack:

                    break;

                case Keys.MediaStop:

                    break;

                case Keys.Menu:

                    break;

                case Keys.Modifiers:

                    break;

                case Keys.Multiply:

                    break;

                case Keys.N:

                    break;

                case Keys.Next:

                    break;

                case Keys.NoName:

                    break;

                case Keys.None:

                    break;

                case Keys.NumLock:

                    break;

                case Keys.NumPad0:

                    break;

                case Keys.NumPad1:

                    break;

                case Keys.NumPad2:

                    break;

                case Keys.NumPad3:

                    break;

                case Keys.NumPad4:

                    break;

                case Keys.NumPad5:

                    break;

                case Keys.NumPad6:

                    break;

                case Keys.NumPad7:

                    break;

                case Keys.NumPad8:

                    break;

                case Keys.NumPad9:

                    break;

                case Keys.O:

                    break;

                case Keys.Oem1:

                    break;

                case Keys.Oem102:

                    break;

                case Keys.Oem2:

                    break;

                case Keys.Oem3:

                    break;

                case Keys.Oem4:

                    break;

                case Keys.Oem5:

                    break;

                case Keys.Oem6:

                    break;

                case Keys.Oem7:

                    break;

                case Keys.Oem8:

                    break;
                /*case Keys.OemBackslash: //Multiple cases

                    break;*/
                case Keys.OemClear:

                    break;
                /*case Keys.OemCloseBrackets: //Multiple cases

                    break;*/
                case Keys.Oemcomma:

                    break;

                case Keys.OemMinus:

                    break;
                /*case Keys.OemOpenBrackets: //Multiple cases

                    break;*/
                case Keys.OemPeriod:

                    break;
                /*case Keys.OemPipe: //Multiple cases

                    break;*/
                case Keys.Oemplus:

                    break;
                /*case Keys.OemQuestion: //Multiple cases

                    break;

                case Keys.OemQuotes: //Multiple cases

                    break;

                case Keys.OemSemicolon: //Multiple cases

                    break;

                case Keys.Oemtilde: //Multiple cases

                    break;*/
                case Keys.P:

                    break;

                case Keys.Pa1:

                    break;

                case Keys.Packet:

                    break;
                /*case Keys.PageDown: //Multiple cases

                    break;*/
                case Keys.PageUp:

                    break;

                case Keys.Pause:

                    break;

                case Keys.Play:

                    break;

                case Keys.Print:

                    break;

                case Keys.PrintScreen:

                    break;
                /*case Keys.Prior: //Multiple cases

                    break;*/
                case Keys.ProcessKey:

                    break;

                case Keys.Q:

                    break;

                case Keys.R:

                    break;

                case Keys.RButton:

                    break;

                case Keys.RControlKey:

                    break;
                /*case Keys.Return: //Multiple cases

                    break;*/
                case Keys.Right:

                    break;

                case Keys.RMenu:

                    break;

                case Keys.RShiftKey:

                    break;

                case Keys.RWin:

                    break;

                case Keys.S:

                    break;

                case Keys.Scroll:

                    break;

                case Keys.Select:

                    break;

                case Keys.SelectMedia:

                    break;

                case Keys.Separator:

                    break;

                case Keys.Shift:

                    break;

                case Keys.ShiftKey:

                    break;

                case Keys.Sleep:

                    break;
                /*case Keys.Snapshot: //Multiple cases

                    break;*/
                case Keys.Space:

                    break;

                case Keys.Subtract:

                    break;

                case Keys.T:

                    break;

                case Keys.Tab:

                    break;

                case Keys.U:

                    break;

                case Keys.Up:

                    break;

                case Keys.V:

                    break;

                case Keys.VolumeDown:

                    break;

                case Keys.VolumeMute:

                    break;

                case Keys.VolumeUp:

                    break;

                case Keys.W:

                    break;

                case Keys.X:

                    break;

                case Keys.XButton1:

                    break;

                case Keys.XButton2:

                    break;

                case Keys.Y:

                    break;

                case Keys.Z:

                    break;

                case Keys.Zoom:

                    break;
            }
            return new Rectangle(0, 0, 0, 0);
        }
    }
}