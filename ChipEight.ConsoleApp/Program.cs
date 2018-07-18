// 
// Program.cs
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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChipEight.ConsoleApp
{
    public class Program
    {
        public static Chip8EmuView View { get; private set; } = new Chip8EmuView();

        public static Chip8Emu Emulator { get; private set; }

        public static CancellationTokenSource CTS { get; private set; } = new CancellationTokenSource();

        public static LogLevel LogLevel { get; private set; } = LogLevel.DebugInfo;

        public static void Main(string[] args)
        {
            if (null == args || args.Length == 0)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Console.OutputEncoding = Encoding.UTF8;
            Console.SetWindowSize(Chip8Emu.DisplayColumns + Chip8EmuView.LogWidth, Chip8Emu.DisplayRows);
            Console.CursorVisible = false;

            Console.CancelKeyPress += Console_CancelKeyPress;
            
            try
            {
                View.Log($"Loading ROM: {args[0]}", LogLevel.Info);

                byte[] romData = File.ReadAllBytes(args[0]);
                Emulator = new Chip8Emu(View, romData);

                Task uiTask = Task.Factory.StartNew(() =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    while (!CTS.Token.IsCancellationRequested)
                    {
                        if (sw.Elapsed >= ViewDelay)
                        {
                            View.ReadKeys();
                            View.DrawScreen();
                            sw.Restart();
                        }
                        Thread.Yield();
                    }
                });

                Task emuTask = Emulator.Start(CTS);

                emuTask.Wait(CTS.Token);
                uiTask.Wait(CTS.Token);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            CTS.Cancel();
        }

        private static TimeSpan ViewDelay = TimeSpan.FromSeconds(1 / 60.0);

    }
}
