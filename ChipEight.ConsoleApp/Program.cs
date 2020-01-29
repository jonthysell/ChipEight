// 
// Program.cs
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
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChipEight.ConsoleApp
{
    public class Program
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

        public static Chip8EmuView View { get; private set; } = new Chip8EmuView();

        public static Chip8Emu Emulator { get; private set; }

        public static CancellationTokenSource CTS { get; private set; } = new CancellationTokenSource();

        public static LogLevel LogLevel { get; private set; } = LogLevel.Info;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.SetWindowSize(Chip8Emu.DisplayColumns + Chip8EmuView.LogWidth, Chip8Emu.DisplayRows);
            Console.CursorVisible = false;

            Console.CancelKeyPress += Console_CancelKeyPress;

#if DEBUG
            LogLevel = LogLevel.DebugInfo;
#endif

            try
            {
                // Parse args
                bool? shiftQuirkEnabled = null;
                bool? loadStoreQuirkEnabled = null;
                string romFile = "";

                bool turbo = false;

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-loadstore":
                        case "/loadstore":
                        case "-loadstorequirk":
                        case "/loadstorequirk":
                            loadStoreQuirkEnabled = true;
                            break;
                        case "-shift":
                        case "/shift":
                        case "-shiftquirk":
                        case "/shiftquirk":
                            shiftQuirkEnabled = true;
                            break;
                        case "-turbo":
                        case "/turbo":
                            turbo = true;
                            break;
                        default:
                            romFile = args[i];
                            break;
                    }
                }

                if (string.IsNullOrWhiteSpace(romFile))
                {
                    Usage();
                    return;
                }

                View.Log($"Loading ROM: {romFile}", LogLevel.Info);

                byte[] romData = File.ReadAllBytes(romFile);
                Emulator = new Chip8Emu(View, romData);

                Emulator.TryConfigureQuirks();

                if (loadStoreQuirkEnabled.HasValue)
                {
                    View.Log("Forcing LoadStoreQuirkEnabled.", LogLevel.Info);
                    Emulator.LoadStoreQuirkEnabled = loadStoreQuirkEnabled.Value;
                }

                if (shiftQuirkEnabled.HasValue)
                {
                    View.Log("Forcing ShiftQuirkEnabled.", LogLevel.Info);
                    Emulator.ShiftQuirkEnabled = shiftQuirkEnabled.Value;
                }

                if (turbo)
                {
                    Emulator.CycleRateHz = Chip8Emu.TurboCycleRateHz;
                }

                View.Log("Ready.", LogLevel.Info);

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

        private static void Usage()
        {
            Console.WriteLine("{0} {1}", ProgramName, ProgramCopyright);
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("ChipEight.ConsoleApp.exe [options] RomFile");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("/loadstore  Enable the Load/Store quirk (required by some roms)");
            Console.WriteLine("/shift      Enable the Shift quirk (required by some roms)");
            Console.WriteLine("/turbo      Enable turbo mode (double cycle rate)");
            Console.WriteLine();
            Console.WriteLine("Control Mapping:");
            Console.WriteLine("  Keys  | Key Pad ");
            Console.WriteLine("1 2 3 4 | 1 2 3 C");
            Console.WriteLine("Q W E R | 4 5 6 D");
            Console.WriteLine("A S D F | 7 8 9 E");
            Console.WriteLine("Z X C V | A 0 B F");
            Console.WriteLine();
            
            Console.WriteLine();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            CTS.Cancel();
        }

        private static readonly TimeSpan ViewDelay = TimeSpan.FromSeconds(1 / 60.0);

    }
}
