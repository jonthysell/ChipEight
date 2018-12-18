// 
// MainForm.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2018 Jon Thysell <http://jonthysell.com>
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
        public static Chip8Emu Emulator { get; private set; } = null;

        public static LogLevel LogLevel { get; private set; } = LogLevel.DebugInfo;

        public static CancellationTokenSource CTS { get; private set; } = null;

        public static Task DisplayTask { get; private set; } = null;

        private static TimeSpan ViewDelay = TimeSpan.FromSeconds(1 / 60.0);

        public MainForm()
        {
            InitializeComponent();
        }

        #region IChip8EmuView

        public bool[,] DisplayBuffer { get; private set; } = new bool[Chip8Emu.DisplayColumns, Chip8Emu.DisplayRows];

        public bool[] KeyState { get; private set; } = new bool[Chip8Emu.NumKeys];

        public void StartBeep()
        {
        }

        public void StopBeep()
        {
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CTS?.Cancel();
            Emulator?.Stop();
            DisplayTask?.Wait();
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

        private void SetKeyByName(string key, bool value)
        {
            KeyState[int.Parse(key, System.Globalization.NumberStyles.HexNumber)] = value;
        }

        private void keyPadKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is Button b)
            {
                SetKeyByName(b.Text, true);
            }
        }

        private void keyPadKeyUp(object sender, KeyEventArgs e)
        {
            if (sender is Button b)
            {
                SetKeyByName(b.Text, false);
            }
        }
    }
}
