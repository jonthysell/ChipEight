// 
// NativeKeyboard.cs
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

namespace ChipEight.ConsoleApp
{
    // Adapted from https://stackoverflow.com/questions/4351258/c-sharp-arrow-key-input-for-a-console-app
    internal static class NativeKeyboard
    {
        private const int KeyPressed = 0x8000;

        public static bool IsKeyDown(KeyCode key)
        {
            return (GetKeyState((int)key) & KeyPressed) != 0;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetKeyState(int key);
    }

    internal enum KeyCode : int
    {
        Key0 = 0x58, // X
        Key1 = 0x31, // 1
        Key2 = 0x32, // 2
        Key3 = 0x33, // 3
        Key4 = 0x51, // Q
        Key5 = 0x57, // W
        Key6 = 0x45, // E
        Key7 = 0x41, // A
        Key8 = 0x53, // S
        Key9 = 0x44, // D
        KeyA = 0x5A, // Z
        KeyB = 0x43, // C
        KeyC = 0x34, // 4
        KeyD = 0x52, // R
        KeyE = 0x46, // F
        KeyF = 0x56, // V
    }
}
