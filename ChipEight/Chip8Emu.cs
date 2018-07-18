// 
// Chip8Emu.cs
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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ChipEight
{
    public class Chip8Emu
    {
        #region Speed Properties

        public uint CycleRateHz { get; set; } = DefaultCycleRateHz;

        public uint DisplayRateHz { get; set; } = DefaultDisplayRateHz;

        #endregion

        #region Memory Properties

        public byte[] Memory { get; private set; } = new byte[MemorySize];

        public byte[] DataRegisters { get; private set; } = new byte[DataRegisterCount];  // V0-VF

        public ushort AddressRegister { get; set; } // I

        #endregion

        #region Execution Properties

        public ushort ProgramCounter { get; set; }

        public Stack<ushort> Stack { get; private set; } = new Stack<ushort>();

        #endregion

        #region Timer Properties

        public byte DelayTimer
        {
            get
            {
                return _delayTimer;
            }
            set
            {
                _delayTimer = value;
                _nextDelayTimerCycle = _curentCycle + (CycleRateHz / TimerRateHz);
            }
        }
        private byte _delayTimer = 0;

        private ulong _nextDelayTimerCycle = ulong.MaxValue;

        public byte SoundTimer
        {
            get
            {
                return _soundTimer;
            }
            set
            {
                if (value > 0 && _soundTimer == 0)
                {
                    _view.Log("Starting beep...", LogLevel.DebugInfo);
                    _view.StartBeep();
                }
                else if (value == 0 && _soundTimer != 0)
                {
                    _view.Log("Stopping beep...", LogLevel.DebugInfo);
                    _view.StopBeep();
                }
                _soundTimer = value;
                _nextSoundTimerCycle = _curentCycle + (CycleRateHz / TimerRateHz); ;
            }
        }
        private byte _soundTimer = 0;

        private ulong _nextSoundTimerCycle = ulong.MaxValue;

        #endregion

        #region Input Properties

        public bool[] KeyState { get; private set; } = new bool[NumKeys];

        #endregion

        #region Graphics Properties

        public bool[,] DisplayData { get; private set; } = new bool[DisplayColumns, DisplayRows];

        public bool UpdateDisplay { get; set; } = false;

        #endregion

        private IChip8EmuView _view;

        private Random _random;

        private ulong _curentCycle;

        private Task _task;

        private CancellationTokenSource _cts;

        #region Constructor

        public Chip8Emu(IChip8EmuView view, byte[] romData)
        {
            if (null == romData || romData.Length == 0)
            {
                throw new ArgumentNullException(nameof(romData));
            }

            _view = view ?? throw new ArgumentNullException(nameof(view));

            Reset();

            _view.Log("Loading FontSet.", LogLevel.Info);
            Array.Copy(FontSet, Memory, FontSet.Length);

            _view.Log("Loading ROM...", LogLevel.Info);
            Array.Copy(romData, 0, Memory, RomStart, romData.Length);

            _view.Log("Ready.", LogLevel.Info);
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _view.Log("Resetting...", LogLevel.Info);

            // Memory
            Array.Clear(Memory, RomStart, Memory.Length - RomStart);
            DataRegisters.Initialize();
            AddressRegister = 0;

            // Execution
            ProgramCounter = RomStart;
            Stack.Clear();

            // Timers
            DelayTimer = 0;
            SoundTimer = 0;

            // Input
            Array.Clear(KeyState, 0, KeyState.Length);

            // Graphics
            Array.Clear(DisplayData, 0, DisplayData.Length);
            UpdateDisplay = false;

            _random = new Random();
            _curentCycle = 0;
        }

        public Task Start(CancellationTokenSource cts)
        {
            if (null == _task)
            {
                _cts = cts;
                CancellationToken token = _cts.Token;

                _task = Task.Factory.StartNew(() =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    while (!token.IsCancellationRequested)
                    {
                        if (sw.Elapsed >= TimeSpan.FromSeconds(1.0 / CycleRateHz))
                        {
                            Step();
                            sw.Restart();
                        }
                        Thread.Yield();
                    }
                }, token);
            }
            return _task;
        }

        public void Stop()
        {
            _cts.Cancel();
            _task.Wait();
        }

        public void Step()
        {
            // Fetch opcode
            ushort opcode = (ushort)((Memory[ProgramCounter] << 8) | Memory[ProgramCounter + 1]);

            _view.Log($"Emulating opcode: 0x{opcode:X4}", LogLevel.DebugInfo);

            // Decode opcode
            switch (opcode & 0xF000)
            {
                case 0x0000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x00E0: // 00E0: Clear screen
                            Array.Clear(DisplayData, 0, DisplayData.Length);
                            UpdateDisplay = true;
                            ProgramCounter += 2;
                            break;
                        case 0x00EE: // 00EE: Subroutine return
                            ProgramCounter = Stack.Pop();
                            ProgramCounter += 2;
                            break;
                        default:
                            _view.Log($"Unknown opcode: 0x{opcode:X4}", LogLevel.Error);
                            break;
                    }
                    break;
                case 0x1000: // 1NNN: Jump to address NNN
                    ProgramCounter = (ushort)(opcode & 0x0FFF);
                    break;
                case 0x2000: // 2NNN: Call subroutine at address NNN
                    Stack.Push(ProgramCounter);
                    ProgramCounter = (ushort)(opcode & 0x0FFF);
                    break;
                case 0x3000: // 3XNN: Skip next instruction if VX == NN
                    if (DataRegisters[(opcode & 0x0F00) >> 8] == (byte)(opcode & 0x00FF))
                    {
                        ProgramCounter += 2;
                    }
                    ProgramCounter += 2;
                    break;
                case 0x4000: // 4XNN: Skip next instruction if VX != NN
                    if (DataRegisters[(opcode & 0x0F00) >> 8] != (byte)(opcode & 0x00FF))
                    {
                        ProgramCounter += 2;
                    }
                    ProgramCounter += 2;
                    break;
                case 0x5000: // 5XY0: Skip next instruction if VX == VY
                    if (DataRegisters[(opcode & 0x0F00) >> 8] == DataRegisters[(opcode & 0x00F0) >> 4])
                    {
                        ProgramCounter += 2;
                    }
                    ProgramCounter += 2;
                    break;
                case 0x6000: // 6XNN: Set VX to NN
                    DataRegisters[(opcode & 0x0F00) >> 8] = (byte)(opcode & 0x00FF);
                    ProgramCounter += 2;
                    break;
                case 0x7000: // 7XNN: Add NN to VX (no carry flag)
                    DataRegisters[(opcode & 0x0F00) >> 8] += (byte)(opcode & 0x00FF);
                    ProgramCounter += 2;
                    break;
                case 0x8000: 
                    switch (opcode & 0x000F)
                    {
                        case 0x0000: // 8XY0: Set VX to VY
                            DataRegisters[(opcode & 0x0F00) >> 8] = DataRegisters[(opcode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;
                        case 0x0001: // 8XY1: Set VX to VX or VY
                            DataRegisters[(opcode & 0x0F00) >> 8] = (byte)(DataRegisters[(opcode & 0x0F00) >> 8] | DataRegisters[(opcode & 0x00F0) >> 4]);
                            ProgramCounter += 2;
                            break;
                        case 0x0002: // 8XY2: Set VX to VX and VY
                            DataRegisters[(opcode & 0x0F00) >> 8] = (byte)(DataRegisters[(opcode & 0x0F00) >> 8] & DataRegisters[(opcode & 0x00F0) >> 4]);
                            ProgramCounter += 2;
                            break;
                        case 0x0003: // 8XY3: Set VX to VX xor VY
                            DataRegisters[(opcode & 0x0F00) >> 8] = (byte)(DataRegisters[(opcode & 0x0F00) >> 8] ^ DataRegisters[(opcode & 0x00F0) >> 4]);
                            ProgramCounter += 2;
                            break;
                        case 0x0004: // 8XY4: Add VY to VX, VF = 1 when carry, 0 otherwise
                            DataRegisters[0xF] = (byte)(DataRegisters[(opcode & 0x00F0) >> 4] > (0xFF - DataRegisters[(opcode & 0x0F00) >> 8]) ? 0x1 : 0x0);
                            DataRegisters[(opcode & 0x0F00) >> 8] += DataRegisters[(opcode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;
                        case 0x0005: // 8XY5: Subtract VY from VX, VF = 1 when not borrow
                            DataRegisters[0xF] = (byte)(DataRegisters[(opcode & 0x0F00) >> 8] > DataRegisters[(opcode & 0x00F0) >> 4] ? 0x1 : 0x0);
                            DataRegisters[(opcode & 0x0F00) >> 8] -= DataRegisters[(opcode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;
                        case 0x0006: // 8XY6: Shift VX >> 1, VF stores least significant bit
                            DataRegisters[0xF] = (byte)(DataRegisters[(opcode & 0x0F00) >> 8] & 0x1);
                            DataRegisters[(opcode & 0x0F00) >> 8] = (byte)(DataRegisters[(opcode & 0x0F00) >> 8] >> 1);
                            ProgramCounter += 2;
                            break;
                        case 0x0007: // 8XY7: Set VX to VY - VX, VF = 1 when not borrow
                            DataRegisters[0xF] = (byte)(DataRegisters[(opcode & 0x00F0) >> 4] > DataRegisters[(opcode & 0x0F00) >> 8] ? 0x1 : 0x0);
                            DataRegisters[(opcode & 0x0F00) >> 8] = (byte)(DataRegisters[(opcode & 0x00F0) >> 4] - DataRegisters[(opcode & 0x0F00) >> 8]);
                            ProgramCounter += 2;
                            break;
                        case 0x000E: // 8XYE: Shift VX << 1, VF stores most significant bit
                            DataRegisters[0xF] = (byte)((DataRegisters[(opcode & 0x0F00) >> 8] & 0x80) >> 7);
                            DataRegisters[(opcode & 0x0F00) >> 8] = (byte)(DataRegisters[(opcode & 0x0F00) >> 8] << 1);
                            ProgramCounter += 2;
                            break;
                        default:
                            _view.Log($"Unknown opcode: 0x{opcode:X4}", LogLevel.Error);
                            break;
                    }
                    break;
                case 0x9000: // 9XY0: Skip next instruction if VX != VY
                    if (DataRegisters[(opcode & 0x0F00) >> 8] != DataRegisters[(opcode & 0x00F0) >> 4])
                    {
                        ProgramCounter += 2;
                    }
                    ProgramCounter += 2;
                    break;
                case 0xA000: // ANNN: Set I to NNN
                    AddressRegister = (ushort)(opcode & 0x0FFF);
                    ProgramCounter += 2;
                    break;
                case 0xB000: // BNNN: Jump to address V0 + NNN
                    ProgramCounter = (ushort)((opcode & 0x0FFF) + DataRegisters[0]);
                    break;
                case 0xC000: // CXNN: Set VX to random number & NN
                    DataRegisters[(opcode & 0x0F00) >> 8] = (byte)((opcode & 0x00FF) & _random.Next());
                    ProgramCounter += 2;
                    break;
                case 0xD000: // DXYN: Draw sprite stored in I with height N to coordinates (VX, VY)
                    {
                        // Get params
                        int startX = DataRegisters[(opcode & 0x0F00) >> 8];
                        int startY = DataRegisters[(opcode & 0x00F0) >> 4];
                        int height = (opcode & 0x000F);

                        // Set VF to 0
                        DataRegisters[0xF] = 0x00;

                        for (int dy = 0; dy < height; dy++)
                        {
                            if (AddressRegister + dy < MemorySize)
                            {
                                byte spriteData = Memory[AddressRegister + dy];
                                for (int dx = 0; dx < SpriteWidth; dx++)
                                {
                                    int x = startX + dx;
                                    int y = startY + dy;

                                    if (x < DisplayColumns && y < DisplayRows)
                                    {
                                        bool oldPixel = DisplayData[x, y];
                                        bool spritePixel = (spriteData & (0x80 >> dx)) != 0;
                                        bool newPixel = oldPixel ^ spritePixel;

                                        // "Collision" dectection
                                        if (oldPixel && !newPixel)
                                        {
                                            // Set VF to 1
                                            DataRegisters[0xF] = 0x01;
                                        }

                                        // Update pixel in display
                                        DisplayData[x, y] = newPixel;
                                    }
                                }
                            }
                        }

                        UpdateDisplay = true;
                        ProgramCounter += 2;
                    }
                    break;
                case 0xE000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x009E: // EX9E: Skip next instruction if key in VX is pressed
                            UpdateKeys();
                            if (KeyState[DataRegisters[(opcode & 0x0F00) >> 8] & 0x0F])
                            {
                                ProgramCounter += 2;
                            }
                            ProgramCounter += 2;
                            break;
                        case 0x00A1: // EXA1: Skip next instruction if key in VX is not pressed
                            UpdateKeys();
                            if (!KeyState[DataRegisters[(opcode & 0x0F00) >> 8] & 0x0F])
                            {
                                ProgramCounter += 2;
                            }
                            ProgramCounter += 2;
                            break;
                        default:
                            _view.Log($"Unknown opcode: 0x{opcode:X4}", LogLevel.Error);
                            break;
                    }
                    break;
                case 0xF000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x0007: // FX07: Set VX to delay timer
                            DataRegisters[(opcode & 0x0F00) >> 8] = DelayTimer;
                            ProgramCounter += 2;
                            break;
                        case 0x000A: // FX0A: Wait for keypress, store in VX
                            {
                                UpdateKeys();
                                bool found = false;
                                for (byte key = 0; key < NumKeys; key++)
                                {
                                    if (KeyState[key])
                                    {
                                        DataRegisters[(opcode & 0x0F00) >> 8] = key;
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    // Exit without updating timers / cycle
                                    return;
                                }

                                ProgramCounter += 2;
                                break;
                            }
                        case 0x0015: // FX15: Set delay timer to VX
                            DelayTimer = DataRegisters[(opcode & 0x0F00) >> 8];
                            ProgramCounter += 2;
                            break;
                        case 0x0018: // FX18: Set sound timer to VX
                            SoundTimer = DataRegisters[(opcode & 0x0F00) >> 8];
                            ProgramCounter += 2;
                            break;
                        case 0x001E: // FX1E: Add VX to I
                            AddressRegister += DataRegisters[(opcode & 0x0F00) >> 8];
                            ProgramCounter += 2;
                            break;
                        case 0x0029: // FX29: Set I to address for character in VX
                            AddressRegister = (ushort)(FontStart + FontHeight * (DataRegisters[(opcode & 0x0F00) >> 8] & 0x0F));
                            ProgramCounter += 2;
                            break;
                        case 0x0033: // FX33: Store binary coded decimal in VX to memory at I through I+2
                            {
                                byte data = DataRegisters[(opcode & 0x0F00) >> 8];
                                WriteMemory((ushort)(AddressRegister + 0), (byte)(data / 100));
                                WriteMemory((ushort)(AddressRegister + 1), (byte)((data % 100) / 10));
                                WriteMemory((ushort)(AddressRegister + 2), (byte)(data % 10));
                                ProgramCounter += 2;
                            }
                            break;
                        case 0x0055: // FX55: Store V0 through VX into memory at I
                            for (int i = 0; i <= ((opcode & 0x0F00) >> 8); i++)
                            {
                                WriteMemory((ushort)(AddressRegister + i), DataRegisters[i]);
                            }
                            ProgramCounter += 2;
                            break;
                        case 0x0065: // FX65: Fill V0 through VX from memory at I
                            for (int i = 0; i <= ((opcode & 0x0F00) >> 8); i++)
                            {
                                if (AddressRegister + i < MemorySize)
                                {
                                    DataRegisters[i] = Memory[AddressRegister + i];
                                }
                            }
                            ProgramCounter += 2;
                            break;
                        default:
                            _view.Log($"Unknown opcode: 0x{opcode:X4}", LogLevel.Error);
                            break;
                    }
                    break;
                default:
                    _view.Log($"Unknown opcode: 0x{opcode:X4}", LogLevel.Error);
                    break;
            }

            // Update Timers
            if (DelayTimer > 0 && _nextDelayTimerCycle == _curentCycle)
            {
                DelayTimer--;
            }

            if (SoundTimer > 0 && _nextSoundTimerCycle == _curentCycle)
            {
                SoundTimer--;
            }

            // Update Display
            if (UpdateDisplay && (_curentCycle % (CycleRateHz / DisplayRateHz) == 0))
            {
                bool[,] displayData = new bool[DisplayColumns, DisplayRows];
                Array.Copy(DisplayData, displayData, DisplayData.Length);

                _view.Log("Updating display...", LogLevel.DebugInfo);
                _view.UpdateDisplay(displayData);

                UpdateDisplay = false;
            }

            _curentCycle++;
        }

        private void WriteMemory(ushort address, byte data)
        {
            if (address < MemorySize)
            {
                Memory[address] = data;
            }
        }

        private void UpdateKeys()
        {
            bool[] keyState = _view.GetKeyState();
            Array.Copy(keyState, KeyState, keyState.Length);
        }

        #endregion

        #region Constants

        public const int MemorySize = 4096;
        public const int DataRegisterCount = 16;

        public const int RomStart = 0x200;

        public const int NumKeys = 16;

        public const int DisplayColumns = 64;
        public const int DisplayRows = 32;

        public const int SpriteWidth = 8;

        public const uint DefaultCycleRateHz = 540;
        public const uint DefaultDisplayRateHz = 60;
        public const uint TimerRateHz = 60;

        #endregion

        #region FontSetData

        public const int FontStart = 0x000;
        public const int FontHeight = 5;

        private static byte[] FontSet = new byte[]
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        #endregion
    }
}
