using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class MoveInstructionsTests
    {
        Memory _memory;
        CPU _cpu;

        [SetUp]
        public void Setup()
        {
            _memory = new Memory();
            _cpu = new CPU(_memory);

            _cpu.Reset();

            SetupHelpers.CheckInitialCPUState(_cpu);
        }
        // 0x10
        [Test]
        public void MoveRegisterToRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Load values into registers.
                0x1100, 0x1234,
                0x1110, 0x4321,
                // Move into empty register.
                0x1002,
                // Overwrite register.
                0x1010,
                // Test invalid registers.
                0x10FF,
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x4321);
            Assert.IsTrue(_cpu.Registers.C == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x4321);
            Assert.IsTrue(_cpu.Registers.C == 0x1234);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x4321);
            Assert.IsTrue(_cpu.Registers.B == 0x4321);
            Assert.IsTrue(_cpu.Registers.C == 0x1234);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x11
        [Test]
        public void Movei16ToRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Put 0x1234 in A.
                0x1100, 0x1234,
                // Put 0x1212 in B.
                0x1110, 0x1212,
                // Put 0x4321 in A.
                0x1100, 0x4321,
                // Put 0x1111 in C.
                0x112F, 0x1111,
                // Invalid instruction call.
                0x11FF, 0x1111,
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0);
            Assert.IsTrue(_cpu.Registers.C == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x1212);
            Assert.IsTrue(_cpu.Registers.C == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x4321);
            Assert.IsTrue(_cpu.Registers.B == 0x1212);
            Assert.IsTrue(_cpu.Registers.C == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x4321);
            Assert.IsTrue(_cpu.Registers.B == 0x1212);
            Assert.IsTrue(_cpu.Registers.C == 0x1111);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x12
        [Test]
        public void MoveFromMemoryAtRegisterToRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1000,
                0x1110, 0x2000,
                // Move from (0x1000) into register C
                0x1202,
                // Overwrite value.
                0x1212,
                // Overwrite self.
                0x1211,
                // Invalid registers.
                0x12FF
            });

            _memory.Ram[0x1000] = 0x1234;
            _memory.Ram[0x2000] = 0xFEFE;

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.C == 0);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0xFEFE);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.C == 0x1234);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0xFEFE);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.C == 0xFEFE);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0xFEFE);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0xFEFE);
            Assert.IsTrue(_cpu.Registers.C == 0xFEFE);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0xFEFE);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);

        }
        // 0x13
        [Test]
        public void MoveFromMemoryAti16ToRegister()
        {
            _memory.SetInitialMemory(new ushort[]
{
                // Init register with random values.
                0x1100, 0x1234,
                // Move from (0x1000) into register B
                0x1310, 0x1000,
                // Overwrite value.
                0x1300, 0x2000,
                // Invalid register.
                0x13FF
            });

            _memory.Ram[0x1000] = 0xDDDD;
            _memory.Ram[0x2000] = 0x8383;

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0);
            Assert.IsTrue(_memory.Ram[0x1000] == 0xDDDD);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x8383);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0xDDDD);
            Assert.IsTrue(_memory.Ram[0x1000] == 0xDDDD);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x8383);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x8383);
            Assert.IsTrue(_cpu.Registers.B == 0xDDDD);
            Assert.IsTrue(_memory.Ram[0x1000] == 0xDDDD);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x8383);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x14
        [Test]
        public void MoveFromRegisterToMemoryAtRegister()
        {
            _memory.SetInitialMemory(new ushort[] {
                // Init registers.
                0x1100, 0x1234,
                0x1110, 0x1000,
                // Store 0x1234 at (0x1000).
                0x1401,
                // Overwrite with self.
                0x1411,
                // Invalid registers.
                0x14FF
            });

            _memory.Ram[0x1000] = 0;

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1000);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x15
        [Test]
        public void Movei16IntoMemoryAtRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Load values into registers.
                0x1100, 0x1000,
                0x1110, 0x2000,
                // Store 0x1234 at 0x1000.
                0x1500, 0x1234,
                // Store 0x4321 at 0x2000.
                0x1510, 0x4321,
                // Overwrite 0x1000.
                0x1500, 0xFFFF,
                // Invalid register.
                0x15FF, 0x1234
            });

            // Ensure memory starts at 0.
            _memory.Ram[0x1000] = 0;
            _memory.Ram[0x2000] = 0;

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0);
            Assert.IsTrue(_memory.Ram[0x2000] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x4321);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0xFFFF);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x4321);
            
            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x16
        [Test]
        public void MoveRegisterIntoMemoryAti16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Load values into registers.
                0x1100, 0xFFFF,
                0x1110, 0x2000,
                // Store value at (0x1000)
                0x1600, 0x1000,
                // Overwrite value.
                0x1610, 0x1000,
                // Invalid register.
                0x16FF, 0xFEFE
            });

            _memory.Ram[0x1000] = 0;

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x2000);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x17
        [Test]
        public void Movei16IntoMemoryAti16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Store 0xFEFE at (0x1000).
                0x1700, 0xFEFE, 0x1000,
                // Overwrite value.
                0x1700, 0x1111, 0x1000,
                // Test with 0x17FF (should be ignored.)
                0x17FF, 0x1234, 0x1000
            });

            _memory.Ram[0x1000] = 0;

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 3);
            Assert.IsTrue(_memory.Ram[0x1000] == 0xFEFE);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1111);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
        }

        // 0x18
        [Test]
        public void MoveFromMemoryAtRegisterToMemoryAtRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Load values into registers.
                0x1100, 0x1000,
                0x1110, 0x2000,
                // Store (0x1000) at (0x2000).
                0x1801,
                // Overwrite with self (No change should occur.)
                0x1800,
                // Invalid registers.
                0x18FF
            });

            _memory.Ram[0x1000] = 0x1010;
            _memory.Ram[0x2000] = 0;

            _cpu.Step(); 
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1010);
            Assert.IsTrue(_memory.Ram[0x2000] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1010);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x1010);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1010);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1010);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x19
        [Test]
        public void MoveFromMemoryAtRegisterToMemoryAti16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Load value into register.
                0x1100, 0x1000,
                //Move from (0x1000) to (0x2000).
                0x1900, 0x2000,
                // Overwrite with self.
                0x1900, 0x1000,
                // Invalid register.
                0x19FF, 0x1234
            });

            _memory.Ram[0x1000] = 0x1234;
            _memory.Ram[0x2000] = 0;

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x1234);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x1234);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x1A
        [Test]
        public void MoveFromMemoryAti16ToMemoryAtRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Load value into register.
                0x1100, 0x2000,
                //Move from (0x1000) to (0x2000).
                0x1A00, 0x1000,
                // Overwrite with self.
                0x1A00, 0x1000,
                // Invalid register.
                0x1AFF, 0x1234
            });

            _memory.Ram[0x1000] = 0x1234;
            _memory.Ram[0x2000] = 0;

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x1234);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x2000);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x1234);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x1B
        [Test]
        public void MoveFromMemoryAti16ToMemoryAti16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                //Move from (0x1000) to (0x2000).
                0x1B00, 0x1000, 0x2000,
                // Overwrite with self.
                0x1B00, 0x1000, 0x1000
            });

            _memory.Ram[0x1000] = 0x9999;
            _memory.Ram[0x2000] = 0;

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 3);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x9999);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x9999);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x9999);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x9999);
        }

        // 0x1C
        [Test]
        public void MoveFromRegisterToi16PlusOffset()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Load value into registers.
                0x1100, 0x0999,
                0x1110, 0x0001,
                0x1120, 0x0002,
                // Move into i16 plus offset.
                0x1C01, 0x1000,
                // Move into i16 plus different offset.
                0x1C02, 0x1000,
                // Replace value
                0x1C21, 0x1000,
                // Invalid registers.
                0x1CFF, 0x1234
            });

            _memory.Ram[0x1000] = 0;
            _memory.Ram[0x1001] = 0;
            _memory.Ram[0x1002] = 0;

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x0999);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0002);
            Assert.IsTrue(_memory.Ram[0x1000] == 0);
            Assert.IsTrue(_memory.Ram[0x1001] == 0);
            Assert.IsTrue(_memory.Ram[0x1002] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x0999);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0002);
            Assert.IsTrue(_memory.Ram[0x1000] == 0);
            Assert.IsTrue(_memory.Ram[0x1001] == 0x0999);
            Assert.IsTrue(_memory.Ram[0x1002] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.A == 0x0999);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0002);
            Assert.IsTrue(_memory.Ram[0x1000] == 0);
            Assert.IsTrue(_memory.Ram[0x1001] == 0x0999);
            Assert.IsTrue(_memory.Ram[0x1002] == 0x0999);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.A == 0x0999);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0002);
            Assert.IsTrue(_memory.Ram[0x1000] == 0);
            Assert.IsTrue(_memory.Ram[0x1001] == 0x0002);
            Assert.IsTrue(_memory.Ram[0x1002] == 0x0999);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        //0x1D
        [Test]
        public void MoveFromMemoryAti16PlusOffsetToRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Load value into registers.
                0x1100, 0x1234,
                0x1110, 0x0001,
                0x1120, 0x0002,
                // Move (0x1001) into A.
                0x1D10, 0x1000,
                // Move (0x1002) into A.
                0x1D20, 0x1000,
                // Invalid registers.
                0x1DFF, 0x1234
            });

            _memory.Ram[0x1000] = 0x1234;
            _memory.Ram[0x1001] = 0x1111;
            _memory.Ram[0x1002] = 0x2222;

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0002);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x1001] == 0x1111);
            Assert.IsTrue(_memory.Ram[0x1002] == 0x2222);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x1111);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0002);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x1001] == 0x1111);
            Assert.IsTrue(_memory.Ram[0x1002] == 0x2222);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.A == 0x2222);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0002);
            Assert.IsTrue(_memory.Ram[0x1000] == 0x1234);
            Assert.IsTrue(_memory.Ram[0x1001] == 0x1111);
            Assert.IsTrue(_memory.Ram[0x1002] == 0x2222);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }
    }

}
