// 
// MainForm.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2018, 2020 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ChipEight.WinFormsApp.Properties;

namespace ChipEight.WinFormsApp
{
    public partial class MainForm : Form, IChip8EmuView
    {
        public static string ProgramName
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0} v{1}", Assembly.GetExecutingAssembly().GetName().Name, (version.Build == 0 && version.Revision == 0) ? string.Format("{0}.{1}", version.Major, version.Minor) : Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
        }

        public static string ProgramCopyright
        {
            get
            {
                return ((AssemblyCopyrightAttribute)(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true)[0])).Copyright;
            }
        }

        public Chip8Emu Emulator { get; private set; } = null;

        public LogLevel LogLevel { get; private set; } = LogLevel.Info;

        public CancellationTokenSource CTS { get; private set; } = null;

        public Task DisplayTask { get; private set; } = null;

        private SoundPlayer SoundPlayer = new SoundPlayer(Resources.SoundSample);

        public bool ResetRequested { get; private set; } = false;

        public MainForm()
        {
            InitializeComponent();
            Text = ProgramName;
#if DEBUG
            LogLevel = LogLevel.DebugInfo;
#endif
            UpdateConfig();
        }

        #region IChip8EmuView

        public bool[,] DisplayBuffer { get; private set; } = new bool[Chip8Emu.DisplayColumns, Chip8Emu.DisplayRows];

        public bool[] KeyState { get; private set; } = new bool[Chip8Emu.NumKeys];

        public void StartBeep()
        {
            SoundPlayer.PlayLooping();
        }

        public void StopBeep()
        {
            SoundPlayer.Stop();
        }

        public bool[] GetKeyState()
        {
            bool[] keyState = new bool[Chip8Emu.NumKeys];
            Array.Copy(KeyState, keyState, KeyState.Length);
            return keyState;
        }

        public void Log(string s, LogLevel level)
        {
            if ((int)LogLevel >= (int)level)
            {
                Console.WriteLine(s);

                MethodInvoker updater = delegate
                {
                    statusLabel.Text = s;
                };

                statusStrip.BeginInvoke(updater);
            }
        }

        public void UpdateDisplay(bool[,] displayData)
        {
            DisplayBuffer = displayData;
        }

        #endregion

        public void DrawDisplay()
        {
            MethodInvoker updater = delegate
            {
                Bitmap bmp = GetDisplayDataAsBitmap(DisplayBuffer);
                int scaleFactor = (int)Math.Floor(Math.Min((double)mainPictureBox.Width / bmp.Width, (double)mainPictureBox.Height / bmp.Height));
                mainPictureBox.Image = ResizeImage(bmp, scaleFactor);
            };

            mainPictureBox.BeginInvoke(updater);
        }

        public void HighlightKeys()
        {
            MethodInvoker updater = delegate
            {
                key0Button.BackColor = KeyState[0] ? SystemColors.Highlight : SystemColors.ControlLight;
                key1Button.BackColor = KeyState[1] ? SystemColors.Highlight : SystemColors.ControlLight;
                key2Button.BackColor = KeyState[2] ? SystemColors.Highlight : SystemColors.ControlLight;
                key3Button.BackColor = KeyState[3] ? SystemColors.Highlight : SystemColors.ControlLight;
                key4Button.BackColor = KeyState[4] ? SystemColors.Highlight : SystemColors.ControlLight;
                key5Button.BackColor = KeyState[5] ? SystemColors.Highlight : SystemColors.ControlLight;
                key6Button.BackColor = KeyState[6] ? SystemColors.Highlight : SystemColors.ControlLight;
                key7Button.BackColor = KeyState[7] ? SystemColors.Highlight : SystemColors.ControlLight;
                key8Button.BackColor = KeyState[8] ? SystemColors.Highlight : SystemColors.ControlLight;
                key9Button.BackColor = KeyState[9] ? SystemColors.Highlight : SystemColors.ControlLight;
                keyAButton.BackColor = KeyState[10] ? SystemColors.Highlight : SystemColors.ControlLight;
                keyBButton.BackColor = KeyState[11] ? SystemColors.Highlight : SystemColors.ControlLight;
                keyCButton.BackColor = KeyState[12] ? SystemColors.Highlight : SystemColors.ControlLight;
                keyDButton.BackColor = KeyState[13] ? SystemColors.Highlight : SystemColors.ControlLight;
                keyEButton.BackColor = KeyState[14] ? SystemColors.Highlight : SystemColors.ControlLight;
                keyFButton.BackColor = KeyState[15] ? SystemColors.Highlight : SystemColors.ControlLight;
            };

            keyPadTableLayoutPanel.BeginInvoke(updater);
        }

        public static Bitmap GetDisplayDataAsBitmap(bool[,] displayData)
        {
            Bitmap bmp = new Bitmap(displayData.GetLength(0), displayData.GetLength(1), PixelFormat.Format1bppIndexed);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = data.Scan0;

            for (int y = 0; y < displayData.GetLength(1); y++)
            {
                for (int x = 0; x < displayData.GetLength(0); x++)
                {
                    SetIndexedPixel(x, y, data, !displayData[x, y]);
                }
            }

            bmp.UnlockBits(data);

            return bmp;
        }

        // Adapted from https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        public static Bitmap ResizeImage(Image image, int scaleFactor)
        {
            scaleFactor = Math.Max(1, scaleFactor);

            int width = image.Width * scaleFactor;
            int height = image.Height * scaleFactor;

            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.AssumeLinear;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.None;
                graphics.PixelOffsetMode = PixelOffsetMode.None;

                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        // Adapted from https://stackoverflow.com/questions/10045238/lockbits-stride-on-1bpp-indexed-image-byte-boundaries
        private static void SetIndexedPixel(int x, int y, BitmapData bmd, bool pixel)
        {
            int index = y * bmd.Stride + (x >> 3);
            byte p = Marshal.ReadByte(bmd.Scan0, index);
            byte mask = (byte)(0x80 >> (x & 0x7));

            if (pixel)
            {
                p &= (byte)(mask ^ 0xff);
            }
            else
            {
                p |= mask;
            }

            Marshal.WriteByte(bmd.Scan0, index, p);
        }

        public void HandleException(Exception ex)
        {
            MessageBox.Show(string.Format(Resources.ExceptionTextFormat, ex.Message, ex.StackTrace), Resources.ExceptionCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region File Menu

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CTS?.Cancel();
                Emulator?.Stop();
                DisplayTask?.Wait();

                OpenFileDialog dialog = new OpenFileDialog();

                dialog.Title = Resources.OpenTitle;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string romFile = dialog.FileName;

                    Log($"Loading ROM: {romFile}", LogLevel.Info);

                    byte[] romData = File.ReadAllBytes(romFile);
                    Emulator = new Chip8Emu(this, romData);

                    DisplayBuffer = new bool[Chip8Emu.DisplayColumns, Chip8Emu.DisplayRows];

                    Emulator.TryConfigureQuirks();

                    Log("Ready.", LogLevel.Info);
                }

                CTS = new CancellationTokenSource();

                Emulator?.Start(CTS);
                DisplayTask = Task.Factory.StartNew(() =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    while (!CTS.Token.IsCancellationRequested)
                    {
                        if (sw.Elapsed >= ViewDelay)
                        {
                            DrawDisplay();
                            sw.Restart();
                        }
                        Thread.Yield();
                    }
                });
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                UpdateConfig();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        #endregion

        #region Config Menu

        private void defaultSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (null != Emulator)
                {
                    Emulator.CycleRateHz = Chip8Emu.DefaultCycleRateHz;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                UpdateConfig();
            }
        }

        private void turboSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (null != Emulator)
                {
                    Emulator.CycleRateHz = Chip8Emu.TurboCycleRateHz;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                UpdateConfig();
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (null != Emulator)
                {
                    Emulator.Reset();
                    Array.Clear(DisplayBuffer, 0, DisplayBuffer.Length);
                    DrawDisplay();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                UpdateConfig();
            }
        }

        private void loadStoreQuirkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (null != Emulator)
                {
                    Emulator.LoadStoreQuirkEnabled = !Emulator.LoadStoreQuirkEnabled;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                UpdateConfig();
            }
        }

        private void shiftQuirkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (null != Emulator)
                {
                    Emulator.ShiftQuirkEnabled = !Emulator.ShiftQuirkEnabled;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                UpdateConfig();
            }
        }

        private void debugLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (LogLevel == LogLevel.DebugInfo)
                {
                    LogLevel = LogLevel.Info;
                    Log("Debug logging disabled.", LogLevel.Info);
                }
                else
                {
                    LogLevel = LogLevel.DebugInfo;
                    Log("Debug logging enabled.", LogLevel.Info);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                UpdateConfig();
            }
        }

        private void UpdateConfig()
        {
            bool activeEmulator = (null != Emulator);

            cPUCycleRateToolStripMenuItem.Enabled = activeEmulator;

            defaultSpeedToolStripMenuItem.Enabled = activeEmulator;
            defaultSpeedToolStripMenuItem.Checked = activeEmulator && Emulator.CycleRateHz == Chip8Emu.DefaultCycleRateHz;

            turboSpeedToolStripMenuItem.Enabled = activeEmulator;
            turboSpeedToolStripMenuItem.Checked = activeEmulator && Emulator.CycleRateHz == Chip8Emu.TurboCycleRateHz;

            resetToolStripMenuItem.Enabled = activeEmulator;

            quirksToolStripMenuItem.Enabled = activeEmulator;

            loadStoreQuirkToolStripMenuItem.Enabled = activeEmulator;
            loadStoreQuirkToolStripMenuItem.Checked = activeEmulator && Emulator.LoadStoreQuirkEnabled;

            shiftQuirkToolStripMenuItem.Enabled = activeEmulator;
            shiftQuirkToolStripMenuItem.Checked = activeEmulator && Emulator.ShiftQuirkEnabled;

            debugLoggingToolStripMenuItem.Checked = LogLevel == LogLevel.DebugInfo;
        }

        #endregion

        #region Help Menu

        private void keyboardMappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("1 2 3 4 | 1 2 3 C");
                sb.AppendLine("Q W E R | 4 5 6 D");
                sb.AppendLine("A S D F | 7 8 9 E");
                sb.AppendLine("Z X C V | A 0 B F");

                MessageBox.Show(sb.ToString(), Resources.KeyboardMappingCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(ProgramName);
                sb.AppendLine();

                sb.AppendLine(ProgramCopyright);
                sb.AppendLine();

                sb.Append(string.Join(Environment.NewLine + Environment.NewLine, _license));

                MessageBox.Show(sb.ToString(), Resources.AboutCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CTS?.Cancel();
            Emulator?.Stop();
            DisplayTask?.Wait();
        }

        private void SetKeyByName(string key, bool value)
        {
            KeyState[int.Parse(key, System.Globalization.NumberStyles.HexNumber)] = value;
            HighlightKeys();
        }

        private void keyPadMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Button b)
            {
                SetKeyByName(b.Text, true);
            }
        }

        private void keyPadMouseUp(object sender, MouseEventArgs e)
        {
            if (sender is Button b)
            {
                SetKeyByName(b.Text, false);
            }
        }

        private void keyPadKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && sender is Button b)
            {
                SetKeyByName(b.Text, true);
            }
            else
            {
                MainForm_KeyDown(sender, e);
            }
        }

        private void keyPadKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && sender is Button b)
            {
                SetKeyByName(b.Text, false);
                e.Handled = true;
            }
            else
            {
                MainForm_KeyUp(sender, e);
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.X: SetKeyByName("0", true); break;
                case Keys.D1: SetKeyByName("1", true); break;
                case Keys.D2: SetKeyByName("2", true); break;
                case Keys.D3: SetKeyByName("3", true); break;
                case Keys.Q: SetKeyByName("4", true); break;
                case Keys.W: SetKeyByName("5", true); break;
                case Keys.E: SetKeyByName("6", true); break;
                case Keys.A: SetKeyByName("7", true); break;
                case Keys.S: SetKeyByName("8", true); break;
                case Keys.D: SetKeyByName("9", true); break;
                case Keys.Z: SetKeyByName("A", true); break;
                case Keys.C: SetKeyByName("B", true); break;
                case Keys.D4: SetKeyByName("C", true); break;
                case Keys.R: SetKeyByName("D", true); break;
                case Keys.F: SetKeyByName("E", true); break;
                case Keys.V: SetKeyByName("F", true); break;
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.X: SetKeyByName("0", false); break;
                case Keys.D1: SetKeyByName("1", false); break;
                case Keys.D2: SetKeyByName("2", false); break;
                case Keys.D3: SetKeyByName("3", false); break;
                case Keys.Q: SetKeyByName("4", false); break;
                case Keys.W: SetKeyByName("5", false); break;
                case Keys.E: SetKeyByName("6", false); break;
                case Keys.A: SetKeyByName("7", false); break;
                case Keys.S: SetKeyByName("8", false); break;
                case Keys.D: SetKeyByName("9", false); break;
                case Keys.Z: SetKeyByName("A", false); break;
                case Keys.C: SetKeyByName("B", false); break;
                case Keys.D4: SetKeyByName("C", false); break;
                case Keys.R: SetKeyByName("D", false); break;
                case Keys.F: SetKeyByName("E", false); break;
                case Keys.V: SetKeyByName("F", false); break;
            }
        }

        private static readonly TimeSpan ViewDelay = TimeSpan.FromSeconds(1 / 60.0);

        private static readonly string[] _license = {
            @"Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:",
            @"The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.",
            @"THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE."
        };
    }
}
