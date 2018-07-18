// 
// MockChip8EmuView.cs
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

namespace ChipEight.Test
{
    class MockChip8EmuView : IChip8EmuView
    {
        public bool[] KeyState = new bool[Chip8Emu.NumKeys];

        public event EventHandler StartBeepEvent;

        public event EventHandler StopBeepEvent;

        public event EventHandler GetKeyStateEvent;

        public event EventHandler<LogEventArgs> LogEvent;

        public event EventHandler<UpdateDisplayEventArgs> UpdateDisplayEvent;

        public void StartBeep()
        {
            StartBeepEvent?.Invoke(this, null);
        }

        public void StopBeep()
        {
            StopBeepEvent?.Invoke(this, null);
        }

        public bool[] GetKeyState()
        {
            GetKeyStateEvent?.Invoke(this, null);
            return KeyState;
        }

        public void Log(string s, LogLevel level)
        {
            LogEvent?.Invoke(this, new LogEventArgs() { S = s, Level = level });
        }

        public void UpdateDisplay(bool[,] displayData)
        {
            UpdateDisplayEvent?.Invoke(this, new UpdateDisplayEventArgs() { DisplayData = displayData });
        }
    }

    class LogEventArgs : EventArgs
    {
        public string S;
        public LogLevel Level;
    }

    class UpdateDisplayEventArgs : EventArgs
    {
        public bool[,] DisplayData;
    }
}
