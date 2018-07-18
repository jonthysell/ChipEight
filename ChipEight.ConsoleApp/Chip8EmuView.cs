// 
// Chip8EmuView.cs
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

namespace ChipEight.ConsoleApp
{
    public class Chip8EmuView : IChip8EmuView
    {
        public List<string> LogBuffer { get; private set; } = new List<string>(Chip8Emu.DisplayRows * 2);

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
            if ((int)Program.LogLevel >= (int)level)
            {
                LogBuffer.Add(s);
                if (LogBuffer.Count > Chip8Emu.DisplayRows)
                {
                    LogBuffer.RemoveAt(0);
                }
            }
        }

        public void UpdateDisplay(bool[,] displayData)
        {
            DisplayBuffer = displayData;
        }

        public void DrawScreen()
        {
            ConsoleColor background = Console.BackgroundColor;
            ConsoleColor foreground = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            for (int y = 0; y < Chip8Emu.DisplayRows; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                for (int x = 0; x < Chip8Emu.DisplayColumns; x++)
                {
                    Console.Write(DisplayBuffer[x, y] ? '█' : ' ');
                }

                if (y < LogBuffer.Count)
                {
                    Console.BackgroundColor = background;
                    Console.ForegroundColor = foreground;
                    Console.Write($" {LogBuffer[y]}".PadRight(LogWidth, ' '));
                }
            }

            Console.SetCursorPosition(0, 0);

            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
        }

        public void ReadKeys()
        {
            for (int i = 0; i < Chip8Emu.NumKeys; i++)
            {
                KeyState[i] = NativeKeyboard.IsKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), ($"Key{i:X}")));
            }
        }

        public const int LogWidth = 64;
    }
}
