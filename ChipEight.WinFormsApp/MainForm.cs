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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

            MethodInvoker updater = delegate
            {
                DrawScreen();
            };

            mainPictureBox.BeginInvoke(updater);
        }

        #endregion

        public void DrawScreen()
        {
            Bitmap newScreen = new Bitmap(Chip8Emu.DisplayColumns, Chip8Emu.DisplayRows);

            for (int y = 0; y < Chip8Emu.DisplayRows; y++)
            {
                for (int x = 0; x < Chip8Emu.DisplayColumns; x++)
                {
                    newScreen.SetPixel(x, y, DisplayBuffer[x, y] ? Color.White : Color.Black);
                }
            }

            mainPictureBox.Image = newScreen;
        }

        public void HandleException(Exception ex)
        {
            MessageBox.Show(string.Format(Resources.ExceptionTextFormat, ex.Message, ex.StackTrace), Resources.ExceptionCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Emulator?.Stop();

                OpenFileDialog dialog = new OpenFileDialog();

                dialog.Title = Resources.OpenTitle;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string romFile = dialog.FileName;

                    Log($"Loading ROM: {romFile}", LogLevel.Info);

                    byte[] romData = File.ReadAllBytes(romFile);
                    Emulator = new Chip8Emu(this, romData);
                }

                Emulator?.Start(new CancellationTokenSource());
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
            Emulator?.Stop();
        }
    }
}
