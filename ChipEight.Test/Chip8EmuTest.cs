// 
// Chip8EmuTest.cs
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChipEight.Test
{
    [TestClass]
    public class Chip8EmuTest
    {
        [TestMethod]
        public void Opcode00E0Test()
        {
            // 00E0: Clear screen
            bool[,] displayData = null;
            bool displayUpdated  = false;

            ushort opcode = 0x00E0;

            SingleOpCodeTest(opcode, (emu, view) =>
            {
                view.UpdateDisplayEvent += (sender, args) =>
                {
                    displayData = args.DisplayData;
                    displayUpdated = true;
                };

                for (int x = 0; x < Chip8Emu.DisplayColumns; x++)
                {
                    for (int y = 0; y < Chip8Emu.DisplayRows; y++)
                    {
                        emu.DisplayData[x, y] = true;
                    }
                }
            }, (emu, view) =>
            {
                Assert.IsTrue(displayUpdated, "View's UpdateDisplay not called.");
                Assert.IsNotNull(displayData, "View has null displayData.");

                for (int x = 0; x < Chip8Emu.DisplayColumns; x++)
                {
                    for (int y = 0; y < Chip8Emu.DisplayRows; y++)
                    {
                        Assert.IsFalse(displayData[x, y], "View's displayData is not cleared.");
                        Assert.IsFalse(emu.DisplayData[x, y], "Emu's displayData is not cleared.");
                    }
                }
            });
        }

        [TestMethod]
        public void Opcode00EETest()
        {
            // 00EE: Subroutine return
            foreach (ushort address in TestAddresses)
            {
                ushort opcode = 0x00EE;

                SingleOpCodeTest(opcode, (emu, view) =>
                {
                    emu.Stack.Push(address);
                }, (emu, view) =>
                {
                    Assert.AreEqual(address + 2, emu.ProgramCounter, "Emu did not return to next line after subroutine call.");
                    Assert.AreEqual(0, emu.Stack.Count, "Emu did not push an address onto the stack.");
                });
            }
        }

        [TestMethod]
        public void Opcode1NNNTest()
        {
            // 1NNN: Jump to address NNN
            foreach (ushort address in TestAddresses)
            {
                ushort opcode = (ushort)(0x1000 | address);

                SingleOpCodeTest(opcode, null, (emu, view) =>
                {
                    Assert.AreEqual(address, emu.ProgramCounter, "Emu did not jump.");
                });
            }
        }

        [TestMethod]
        public void Opcode2NNNTest()
        {
            // 2NNN: Call subroutine at address NNN
            foreach (ushort address in TestAddresses)
            {
                ushort opcode = (ushort)(0x2000 | address);

                SingleOpCodeTest(opcode, null, (emu, view) =>
                {
                    Assert.AreEqual(address, emu.ProgramCounter, "Emu did not jump.");
                    Assert.AreEqual(1, emu.Stack.Count, "Emu did not push an address onto the stack.");
                    Assert.AreEqual(Chip8Emu.RomStart, emu.Stack.Peek(), "Emu did not push return address on stack.");
                });
            }
        }

        [TestMethod]
        public void Opcode3XNNTest()
        {
            // 3XNN: Skip next instruction if VX == NN
            for (byte x = 0; x <=0xF; x++)
            {
                foreach (byte data in TestData)
                {
                    ushort opcode = (ushort)(0x3000 | (x << 8) | data);

                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = data;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(Chip8Emu.RomStart + 4, emu.ProgramCounter, "Emu did not skip the next instruction.");
                    });

                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = (byte)(~data);
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                    });
                }
            }
        }

        [TestMethod]
        public void Opcode4XNNTest()
        {
            // 4XNN: Skip next instruction if VX != NN
            for (byte x = 0; x <= 0xF; x++)
            {
                foreach (byte data in TestData)
                {
                    ushort opcode = (ushort)(0x4000 | (x << 8) | data);

                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = (byte)(~data);
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(Chip8Emu.RomStart + 4, emu.ProgramCounter, "Emu did not skip the next instruction.");
                    });

                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = data;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                    });
                }
            }
        }

        [TestMethod]
        public void Opcode5XY0Test()
        {
            // 5XY0: Skip next instruction if VX == VY
            for (int i = 0; i <= 0xFF; i++)
            {
                ushort opcode = (ushort)(0x5000 | (i << 4));
                byte x = (byte)((i & 0xF0) >> 4);
                byte y = (byte)((i & 0x0F));

                foreach (byte data in TestData)
                {
                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = data;
                        emu.DataRegisters[y] = data;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(Chip8Emu.RomStart + 4, emu.ProgramCounter, "Emu did not skip the next instruction.");
                    });

                    if (x != y)
                    {
                        SingleOpCodeTest(opcode, (emu, view) =>
                        {
                            emu.DataRegisters[x] = data;
                            emu.DataRegisters[y] = (byte)(~data);
                        }, (emu, view) =>
                        {
                            Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                        });
                    }
                }
            }
        }

        [TestMethod]
        public void Opcode6XNNTest()
        {
            // 6XNN: Set VX to NN
            for (byte x = 0; x <= 0xF; x++)
            {
                foreach (byte data in TestData)
                {
                    ushort opcode = (ushort)(0x6000 | (x << 8) | data);

                    SingleOpCodeTest(opcode, null, (emu, view) =>
                    {
                        Assert.AreEqual(data, emu.DataRegisters[x], "Emu did not set data register correctly.");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                    });
                }
            }
        }

        [TestMethod]
        public void Opcode7XNNTest()
        {
            // 7XNN: Add NN to VX (no carry flag)
            for (byte x = 0; x <= 0xF; x++)
            {
                foreach (byte data in TestData)
                {
                    foreach (byte nn in TestData)
                    {
                        ushort opcode = (ushort)(0x7000 | (x << 8) | nn);

                        SingleOpCodeTest(opcode, (emu, view) =>
                        {
                            emu.DataRegisters[x] = data;
                        }, (emu, view) =>
                        {
                            Assert.AreEqual((byte)(data + nn), emu.DataRegisters[x], "Emu did not set the data register correctly.");
                            Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                        });
                    }
                }
            }
        }

        [TestMethod]
        public void Opcode8XY0Test()
        {
            // 8XY0: Set VX to VY
            for (int i = 0; i <= 0xFF; i++)
            {
                ushort opcode = (ushort)(0x8000 | (i << 4));
                byte x = (byte)((i & 0xF0) >> 4);
                byte y = (byte)((i & 0x0F));

                foreach (byte data in TestData)
                {
                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[y] = data;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(data, emu.DataRegisters[x], "Emu did not set the data register correctly.");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                    });
                }
            }
        }

        [TestMethod]
        public void Opcode8XY1Test()
        {
            // 8XY1: Set VX to VX or VY
            for (int i = 0; i <= 0xFF; i++)
            {
                ushort opcode = (ushort)(0x8001 | (i << 4));
                byte x = (byte)((i & 0xF0) >> 4);
                byte y = (byte)((i & 0x0F));

                foreach (byte dataX in TestData)
                {
                    foreach (byte dataY in TestData)
                    {
                        byte expected = (byte)(dataX | dataY);
                        SingleOpCodeTest(opcode, (emu, view) =>
                        {
                            emu.DataRegisters[x] = dataX;
                            emu.DataRegisters[y] = dataY;
                            expected = (byte)(emu.DataRegisters[x] | emu.DataRegisters[y]);

                        }, (emu, view) =>
                        {
                            Assert.AreEqual(expected, emu.DataRegisters[x], "Emu did not set the data register correctly.");
                            Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                        });
                    }
                }
            }
        }

        [TestMethod]
        public void Opcode8XY2Test()
        {
            // 8XY2: Set VX to VX and VY
            for (int i = 0; i <= 0xFF; i++)
            {
                ushort opcode = (ushort)(0x8002 | (i << 4));
                byte x = (byte)((i & 0xF0) >> 4);
                byte y = (byte)((i & 0x0F));

                foreach (byte dataX in TestData)
                {
                    foreach (byte dataY in TestData)
                    {
                        byte expected = (byte)(dataX & dataY);
                        SingleOpCodeTest(opcode, (emu, view) =>
                        {
                            emu.DataRegisters[x] = dataX;
                            emu.DataRegisters[y] = dataY;
                            expected = (byte)(emu.DataRegisters[x] & emu.DataRegisters[y]);

                        }, (emu, view) =>
                        {
                            Assert.AreEqual(expected, emu.DataRegisters[x], "Emu did not set the data register correctly.");
                            Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                        });
                    }
                }
            }
        }

        [TestMethod]
        public void Opcode8XY3Test()
        {
            // 8XY3: Set VX to VX xor VY
            for (int i = 0; i <= 0xFF; i++)
            {
                ushort opcode = (ushort)(0x8003 | (i << 4));
                byte x = (byte)((i & 0xF0) >> 4);
                byte y = (byte)((i & 0x0F));

                foreach (byte dataX in TestData)
                {
                    foreach (byte dataY in TestData)
                    {
                        byte expected = (byte)(dataX ^ dataY);
                        SingleOpCodeTest(opcode, (emu, view) =>
                        {
                            emu.DataRegisters[x] = dataX;
                            emu.DataRegisters[y] = dataY;
                            expected = (byte)(emu.DataRegisters[x] ^ emu.DataRegisters[y]);

                        }, (emu, view) =>
                        {
                            Assert.AreEqual(expected, emu.DataRegisters[x], "Emu did not set the data register correctly.");
                            Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                        });
                    }
                }
            }
        }

        [TestMethod]
        public void Opcode8XY4Test()
        {
            // 8XY4: Add VY to VX, VF = 1 when carry, 0 otherwise
            for (byte x = 0; x < 0xF; x++)
            {
                for (byte y = 0; y < 0xF; y++)
                {
                    ushort opcode = (ushort)(0x8004 | (x << 8) | (y << 4));
                    foreach (byte dataX in TestData)
                    {
                        foreach (byte dataY in TestData)
                        {
                            byte expected = (byte)(dataX + dataY);
                            byte carry = (byte)(((dataX + dataY) >> 8) & 0x1);
                            SingleOpCodeTest(opcode, (emu, view) =>
                            {
                                emu.DataRegisters[x] = dataX;
                                emu.DataRegisters[y] = dataY;
                                expected = (byte)(emu.DataRegisters[x] + emu.DataRegisters[y]);
                                carry = (byte)(((emu.DataRegisters[x] + emu.DataRegisters[y]) >> 8) & 0x1);
                            }, (emu, view) =>
                            {
                                Assert.AreEqual(expected, emu.DataRegisters[x], "Emu did not set the data register correctly.");
                                Assert.AreEqual(carry, emu.DataRegisters[0xF], "Emu did not set the carry flag correctly.");
                                Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Opcode8XY5Test()
        {
            // 8XY5: Subtract VY from VX, VF = 1 when not borrow
            for (byte x = 0; x < 0xF; x++)
            {
                for (byte y = 0; y < 0xF; y++)
                {
                    ushort opcode = (ushort)(0x8005 | (x << 8) | (y << 4));
                    foreach (byte dataX in TestData)
                    {
                        foreach (byte dataY in TestData)
                        {
                            byte expected = (byte)(dataX - dataY);
                            byte borrow = (byte)(dataX > dataY ? 0x1 : 0x0);
                            SingleOpCodeTest(opcode, (emu, view) =>
                            {
                                emu.DataRegisters[x] = dataX;
                                emu.DataRegisters[y] = dataY;
                                expected = (byte)(emu.DataRegisters[x] - emu.DataRegisters[y]);
                                borrow = (byte)(emu.DataRegisters[x] >= emu.DataRegisters[y] ? 0x1 : 0x0);
                            }, (emu, view) =>
                            {
                                Assert.AreEqual(expected, emu.DataRegisters[x], "Emu did not set the data register correctly.");
                                Assert.AreEqual(borrow, emu.DataRegisters[0xF], "Emu did not set the borrow flag correctly.");
                                Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Opcode8XY6Test()
        {
            // 8XY6: Shift VX >> 1, VF stores least significant bit
            for (byte x = 0; x < 0xF; x++)
            {
                ushort opcode = (ushort)(0x8006 | (x << 8));
                foreach (byte dataX in TestData)
                {
                    byte expected = (byte)(dataX >> 1);
                    byte lsb = (byte)(dataX & 0x1);
                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = dataX;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(expected, emu.DataRegisters[x], "Emu did not set the data register correctly.");
                        Assert.AreEqual(lsb, emu.DataRegisters[0xF], "Emu did not set the LSB flag correctly.");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                    });
                }
            }
        }

        [TestMethod]
        public void Opcode8XY7Test()
        {
            // 8XY7: Set VX to VY - VX, VF = 1 when not borrow
            for (byte x = 0; x < 0xF; x++)
            {
                for (byte y = 0; y < 0xF; y++)
                {
                    ushort opcode = (ushort)(0x8007 | (x << 8) | (y << 4));
                    foreach (byte dataX in TestData)
                    {
                        foreach (byte dataY in TestData)
                        {
                            byte expected = (byte)(dataY - dataX);
                            byte borrow = (byte)(dataY > dataX ? 0x1 : 0x0);
                            SingleOpCodeTest(opcode, (emu, view) =>
                            {
                                emu.DataRegisters[x] = dataX;
                                emu.DataRegisters[y] = dataY;
                                expected = (byte)(emu.DataRegisters[y] - emu.DataRegisters[x]);
                                borrow = (byte)(emu.DataRegisters[y] >= emu.DataRegisters[x] ? 0x1 : 0x0);
                            }, (emu, view) =>
                            {
                                Assert.AreEqual(expected, emu.DataRegisters[x], "Emu did not set the data register correctly.");
                                Assert.AreEqual(borrow, emu.DataRegisters[0xF], "Emu did not set the borrow flag correctly.");
                                Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Opcode8XYETest()
        {
            // 8XYE: Shift VX << 1, VF stores most significant bit
            for (byte x = 0; x < 0xF; x++)
            {
                ushort opcode = (ushort)(0x800E | (x << 8));
                foreach (byte dataX in TestData)
                {
                    byte expected = (byte)(dataX << 1);
                    byte msb = (byte)(dataX >> 7);
                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = dataX;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(expected, emu.DataRegisters[x], "Emu did not set the data register correctly.");
                        Assert.AreEqual(msb, emu.DataRegisters[0xF], "Emu did not set the MSB flag correctly.");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                    });
                }
            }
        }

        [TestMethod]
        public void Opcode9XY0Test()
        {
            // 9XY0: Skip next instruction if VX != VY
            for (int i = 0; i <= 0xFF; i++)
            {
                ushort opcode = (ushort)(0x9000 | (i << 4));
                byte x = (byte)((i & 0xF0) >> 4);
                byte y = (byte)((i & 0x0F));

                foreach (byte data in TestData)
                {
                    if (x != y)
                    {
                        SingleOpCodeTest(opcode, (emu, view) =>
                        {
                            emu.DataRegisters[x] = data;
                            emu.DataRegisters[y] = (byte)(~data);
                        }, (emu, view) =>
                        {
                            Assert.AreEqual(Chip8Emu.RomStart + 4, emu.ProgramCounter, "Emu did not skip the next instruction.");
                        });
                    }

                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = data;
                        emu.DataRegisters[y] = data;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeANNNTest()
        {
            // ANNN: Set I to NNN
            foreach (ushort address in TestAddresses)
            {
                ushort opcode = (ushort)(0xA000 | address);

                SingleOpCodeTest(opcode, null, (emu, view) =>
                {
                    Assert.AreEqual(address, emu.AddressRegister, "Emu did not set the address register correctly.");
                    Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                });
            }
        }

        [TestMethod]
        public void OpcodeBNNNTest()
        {
            // BNNN: Jump to address V0 + NNN
            foreach (ushort address in TestAddresses)
            {
                ushort opcode = (ushort)(0xB000 | address);

                foreach (byte data in TestData)
                {
                    SingleOpCodeTest(opcode, (emu, view) => {
                        emu.DataRegisters[0x0] = data;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual((ushort)(address + data), emu.ProgramCounter, "Emu did not jump.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeCXNNTest()
        {
            // CXNN: Set VX to random number & NN
            for (byte x = 0; x <= 0xF; x++)
            {
                foreach (byte data in TestData)
                {
                    ushort opcode = (ushort)(0xC000 | (x << 8) | data);

                    SingleOpCodeTest(opcode, null, (emu, view) =>
                    {
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeDXYNTest()
        {
            // DXYN: Draw sprite stored in I with height N to coordinates (VX, VY)
            byte x = 0x0;
            byte y = 0x1;

            foreach (byte[] sprite in TestSprites)
            {
                byte height = (byte)(sprite.Length);
                foreach (ushort address in TestAddresses)
                {
                    if (address + (height - 1) < Chip8Emu.RomStart || address > Chip8Emu.RomStart + 1)
                    {
                        foreach (byte dataX in TestData)
                        {
                            foreach (byte dataY in TestData)
                            {
                                ushort opcode = (ushort)(0xD000 | (x << 8) | (y << 4) | sprite.Length);

                                bool[,] expectedData = GetExpectedDisplayData(dataX, dataY, address, sprite);

                                bool[,] updateDisplayData = null;
                                bool updateDisplayCalled = false;

                                SingleOpCodeTest(opcode, (emu, view) =>
                                {
                                    emu.DataRegisters[x] = dataX;
                                    emu.DataRegisters[y] = dataY;

                                    emu.AddressRegister = address;

                                    for (int i = 0; i < sprite.Length; i++)
                                    {
                                        if (address + i < Chip8Emu.MemorySize)
                                        {
                                            emu.Memory[address + i] = sprite[i];
                                        }
                                    }

                                    view.UpdateDisplayEvent += (sender, args) =>
                                    {
                                        updateDisplayCalled = true;
                                        updateDisplayData = args.DisplayData;
                                    };
                                }, (emu, view) =>
                                {
                                    Assert.IsTrue(updateDisplayCalled, "Emu did not call View.UpdateDisplay().");
                                    Assert.IsNotNull(updateDisplayData, "Emu called View.UpdateDisplay() with null.");

                                    TestDisplayData(emu, expectedData, updateDisplayData);

                                    Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                                });
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void OpcodeEX9ETest()
        {
            // EX9E: Skip next instruction if key in VX is pressed
            for (byte x = 0; x <= 0xF; x++)
            {
                ushort opcode = (ushort)(0xE09E | (x << 8));
                
                foreach (byte data in TestData)
                {
                    byte key = (byte)(data & 0x0F);
                    bool getKeyStateCalled = false;

                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = data;
                        view.KeyState[key] = true;
                        view.GetKeyStateEvent += (sender, args) =>
                        {
                            getKeyStateCalled = true;
                        };
                    }, (emu, view) =>
                    {
                        Assert.IsTrue(getKeyStateCalled, "Emu did not call View.GetKeyState()");
                        Assert.IsTrue(emu.KeyState[key], "Emu did not update the key from the view.");
                        Assert.AreEqual(Chip8Emu.RomStart + 4, emu.ProgramCounter, "Emu did not skip the next instruction.");
                    });

                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = data;
                        view.KeyState[key] = false;
                        view.GetKeyStateEvent += (sender, args) =>
                        {
                            getKeyStateCalled = true;
                        };
                    }, (emu, view) =>
                    {
                        Assert.IsTrue(getKeyStateCalled, "Emu did not call View.GetKeyState()");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeEXA1Test()
        {
            // EXA1: Skip next instruction if key in VX is not pressed
            for (byte x = 0; x <= 0xF; x++)
            {
                ushort opcode = (ushort)(0xE0A1 | (x << 8));

                foreach (byte data in TestData)
                {
                    byte key = (byte)(data & 0x0F);
                    bool getKeyStateCalled = false;

                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = data;
                        view.KeyState[key] = false;
                        view.GetKeyStateEvent += (sender, args) =>
                        {
                            getKeyStateCalled = true;
                        };
                    }, (emu, view) =>
                    {
                        Assert.IsTrue(getKeyStateCalled, "Emu did not call View.GetKeyState()");
                        Assert.AreEqual(Chip8Emu.RomStart + 4, emu.ProgramCounter, "Emu did not skip the next instruction.");
                    });

                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = data;
                        view.KeyState[key] = true;
                        view.GetKeyStateEvent += (sender, args) =>
                        {
                            getKeyStateCalled = true;
                        };
                    }, (emu, view) =>
                    {
                        Assert.IsTrue(getKeyStateCalled, "Emu did not call View.GetKeyState()");
                        Assert.IsTrue(emu.KeyState[key], "Emu did not update the key from the view.");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not go to the next instruction.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeFX07Test()
        {
            // FX07: Set VX to delay timer
            for (byte x = 0; x <= 0xF; x++)
            {
                ushort opcode = (ushort)(0xF007 | (x << 8));

                foreach (byte data in TestData)
                {
                    SingleOpCodeTest(opcode, (emu, view) => {
                        emu.DelayTimer = data;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(data, emu.DataRegisters[x], "Emu did not set data register correctly.");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeFX0ATest()
        {
            // FX0A: Wait for keypress, store in VX
            for (byte x = 0; x <= 0xF; x++)
            {
                ushort opcode = (ushort)(0xF00A | (x << 8));
                bool getKeyStateCalled = false;

                SingleOpCodeTest(opcode, (emu, view) =>
                {
                    view.GetKeyStateEvent += (sender, args) =>
                    {
                        getKeyStateCalled = true;
                    };
                }, (emu, view) =>
                {
                    Assert.IsTrue(getKeyStateCalled, "Emu did not call View.GetKeyState()");
                    Assert.AreEqual(Chip8Emu.RomStart, emu.ProgramCounter, "Emu did not halt as expected.");
                });

                for (byte key = 0; key < Chip8Emu.NumKeys; key++)
                {
                    getKeyStateCalled = false;

                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        view.KeyState[key] = true;
                        view.GetKeyStateEvent += (sender, args) =>
                        {
                            getKeyStateCalled = true;
                        };
                    }, (emu, view) =>
                    {
                        Assert.IsTrue(getKeyStateCalled, "Emu did not call View.GetKeyState()");
                        Assert.IsTrue(emu.KeyState[key], "Emu did not update the key from the view.");
                        Assert.AreEqual(key, emu.DataRegisters[x], "Emu did not set the data register correctly.");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeFX15Test()
        {
            // FX15: Set delay timer to VX
            for (byte x = 0; x <= 0xF; x++)
            {
                ushort opcode = (ushort)(0xF015 | (x << 8));

                foreach (byte data in TestData)
                {
                    SingleOpCodeTest(opcode, (emu, view) => {
                        emu.DataRegisters[x] = data;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(data, emu.DelayTimer, "Emu did not set delay timer correctly.");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeFX18Test()
        {
            // FX18: Set sound timer to VX
            for (byte x = 0; x <= 0xF; x++)
            {
                ushort opcode = (ushort)(0xF018 | (x << 8));

                foreach (byte data in TestData)
                {
                    SingleOpCodeTest(opcode, (emu, view) => {
                        emu.DataRegisters[x] = data;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(data, emu.SoundTimer, "Emu did not set sound timer correctly.");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeFX1ETest()
        {
            // FX1E: Add VX to I
            for (byte x = 0; x <= 0xF; x++)
            {
                foreach (ushort i in TestAddresses)
                {
                    ushort opcode = (ushort)(0xF01E | (x << 8));

                    foreach (byte data in TestData)
                    {
                        ushort expected = (ushort)(i + data);
                        SingleOpCodeTest(opcode, (emu, view) =>
                        {
                            emu.AddressRegister = i;
                            emu.DataRegisters[x] = data;
                        }, (emu, view) =>
                        {
                            Assert.AreEqual(expected, emu.AddressRegister, "Emu did not set address register correctly.");
                            Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                        });
                    }
                }
            }
        }

        [TestMethod]
        public void OpcodeFX29Test()
        {
            // FX29: Set I to address for character in VX
            for (byte x = 0; x <= 0xF; x++)
            {
                ushort opcode = (ushort)(0xF029 | (x << 8));

                foreach (byte data in TestData)
                {
                    ushort expected = (ushort)(Chip8Emu.FontStart + Chip8Emu.FontHeight * (data & 0xF));
                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.DataRegisters[x] = data;
                    }, (emu, view) =>
                    {
                        Assert.AreEqual(expected, emu.AddressRegister, "Emu did not set address register correctly.");
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeFX33Test()
        {
            // FX33: Store binary coded decimal in VX to memory at I through I+2
            for (byte x = 0; x <= 0xF; x++)
            {
                ushort opcode = (ushort)(0xF033 | (x << 8));

                foreach (ushort address in TestAddresses)
                {
                    foreach (byte data in TestData)
                    {
                        byte expectedHundreds = (byte)(data / 100);
                        byte expectedTens = (byte)((data % 100) / 10);
                        byte expectedOnes = (byte)(data % 10);

                        SingleOpCodeTest(opcode, (emu, view) =>
                        {
                            emu.AddressRegister = address;
                            emu.DataRegisters[x] = data;
                        }, (emu, view) =>
                        {
                            TestMemory(emu, (ushort)(address + 0), expectedHundreds, "Emu did not set memory for the hundreds place correctly.");
                            TestMemory(emu, (ushort)(address + 1), expectedTens, "Emu did not set memory for the tens place correctly.");
                            TestMemory(emu, (ushort)(address + 2), expectedOnes, "Emu did not set memory for the ones place correctly.");
                            Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                        });
                    }
                }
            }
        }

        [TestMethod]
        public void OpcodeFX55Test()
        {
            // FX55: Store V0 through VX into memory at I
            for (byte x = 0; x <= 0xF; x++)
            {
                ushort opcode = (ushort)(0xF055 | (x << 8));

                foreach (ushort address in TestAddresses)
                {
                    SingleOpCodeTest(opcode, (emu, view) =>
                    {
                        emu.AddressRegister = address;
                        for (int i = 0; i <= x; i++)
                        {
                            emu.DataRegisters[i] = TestData[i % TestData.Length];
                        }
                    }, (emu, view) =>
                    {
                        for (int i = 0; i <= x; i++)
                        {
                            TestMemory(emu, (ushort)(address + i), TestData[i % TestData.Length], "Emu did not set memory correctly.");
                        }
                        Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                    });
                }
            }
        }

        [TestMethod]
        public void OpcodeFX65Test()
        {
            // FX65: Fill V0 through VX from memory at I
            for (byte x = 0; x <= 0xF; x++)
            {
                ushort opcode = (ushort)(0xF065 | (x << 8));

                foreach (ushort address in TestAddresses)
                {
                    if (address + x < Chip8Emu.RomStart || address > Chip8Emu.RomStart + 1)
                    {
                        SingleOpCodeTest(opcode, (emu, view) =>
                        {
                            emu.AddressRegister = address;
                            for (int i = 0; i <= x; i++)
                            {
                                if (address + i < Chip8Emu.MemorySize)
                                {
                                    emu.Memory[address + i] = TestData[i % TestData.Length];
                                }
                            }
                        }, (emu, view) =>
                        {
                            for (int i = 0; i <= x; i++)
                            {
                                if (address + i < Chip8Emu.MemorySize)
                                {
                                    TestDataRegister(emu, (byte)i, TestData[i % TestData.Length], "Emu did not set data register correctly.");
                                }
                            }
                            Assert.AreEqual(Chip8Emu.RomStart + 2, emu.ProgramCounter, "Emu did not increment program counter.");
                        });
                    }
                }
            }
        }

        private void SingleOpCodeTest(ushort opcode, Action<Chip8Emu, MockChip8EmuView> setup, Action<Chip8Emu, MockChip8EmuView> verify)
        {
            MockChip8EmuView view = new MockChip8EmuView();
            Chip8Emu emu = new Chip8Emu(view, new byte[] { (byte)((opcode & 0xFF00) >> 8), (byte)(opcode & 0x00FF) });

            setup?.Invoke(emu, view);
            emu.Step();
            verify?.Invoke(emu, view);
        }

        private void TestMemory(Chip8Emu emu, ushort address, byte expectedData, string message)
        {
            if (address < Chip8Emu.MemorySize)
            {
                Assert.AreEqual(expectedData, emu.Memory[address], message);
            }
        }

        private void TestDataRegister(Chip8Emu emu, byte register, byte expectedData, string message)
        {
            if (register < Chip8Emu.DataRegisterCount)
            {
                Assert.AreEqual(expectedData, emu.DataRegisters[register], message);
            }
        }

        private bool[,] GetExpectedDisplayData(byte startX, byte startY, ushort address, byte[] sprite)
        {
            bool[,] data = new bool[Chip8Emu.DisplayColumns, Chip8Emu.DisplayRows];

            for (int dy = 0; dy < sprite.Length; dy++)
            {
                if (address + dy < Chip8Emu.MemorySize)
                {
                    for (int dx = 0; dx < Chip8Emu.SpriteWidth; dx++)
                    {
                        int x = startX + dx;
                        int y = startY + dy;

                        bool spritePixel = (sprite[dy] & (0x80 >> dx)) != 0;

                        if (x < Chip8Emu.DisplayColumns && y < Chip8Emu.DisplayRows)
                        {
                            data[x, y] = spritePixel;
                        }
                    }
                }
            }

            return data;
        }

        private void TestDisplayData(Chip8Emu emu, bool[,] expectedDisplayData, bool[,] updateDisplayData)
        {
            for (int x = 0; x < Chip8Emu.DisplayColumns; x++)
            {
                for (int y = 0; y < Chip8Emu.DisplayRows; y++)
                {
                    Assert.AreEqual(expectedDisplayData[x, y], emu.DisplayData[x, y], $"Emu did not set display data properly at ({x},{y}).");
                    Assert.AreEqual(expectedDisplayData[x, y], updateDisplayData[x, y], $"Updated display params was not set properly at ({x},{y}).");
                }
            }
        }

        private static byte[] TestData = new byte[] {
            0x00,
            0x0F,
            0x01,
            0x55,
            0x80,
            0xAA,
            0xF0,
            0xFE,
            0xFF,
        };

        private static ushort[] TestAddresses = new ushort[] {
            0x000,
            0x001,
            0x1FF,
            0x200,
            0x201,
            0xFFE,
            0xFFF,
        };

        private static byte[][] TestSprites = new byte[][]
        {
            new byte[] { 0x00 },
            new byte[] { 0x00, 0x00 },
            new byte[] { 0xFF },
            new byte[] { 0xFF, 0xFF },
        };
    }
}
