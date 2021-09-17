using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class Jump
    {
        Registers _registers;

        Memory _memory;
        CPU _cpu;

        [SetUp]
        public void Setup()
        {
            _registers = new Registers();

            _memory = new Memory(_registers);
            _cpu = new CPU(_memory, _registers);

            _cpu.Reset();

            SetupHelpers.CheckInitialCPUState(_cpu);
        }
        // 0x70
        [Test]
        public void JumpToRegister()
        {
            _memory.SetInitialMemory(new ushort[] 
            {
                // Init registers.
                0x1100, 0x4000,
                0x1110, 0xE000,
                // Jump to first register.
                0x7000,
            });

            _memory.SetSubroutineAtAddress(0x4000, new ushort[]
            {
                // Jump to second register.
                0x7010,
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x4000);
            Assert.IsTrue(_cpu.Registers.B == 0xE000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x4000);
            Assert.IsTrue(_cpu.Registers.A == 0x4000);
            Assert.IsTrue(_cpu.Registers.B == 0xE000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0xE000);
            Assert.IsTrue(_cpu.Registers.A == 0x4000);
            Assert.IsTrue(_cpu.Registers.B == 0xE000);
        }

        // 0x71
        [Test]
        public void JumpToi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Jump to first register.
                0x7100, 0x4000
            });

            _memory.SetSubroutineAtAddress(0x4000, new ushort[]
            {
                // Jump to second register.
                0x7100, 0xE000
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x4000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0xE000);
        }

        // 0x72
        [Test]
        public void TestToi16WithOffset()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1000,
                0x1110, 0x2000,
                // Jump to first register.
                0x7200, 0x3000,
            });

            _memory.SetSubroutineAtAddress(0x4000, new ushort[]
            {
                // Jump to second register.
                0x7210, 0xC000,
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x4000);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0xE000);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
        }

        // 0x73
        [Test]
        public void JumpToi16IfTwoRegistersEqual()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1234,
                0x1110, 0x1235,
                0x1120, 0x1235,
                // Jump if equal (should fail.)
                0x7301, 0x2000,
                // Jump if equal (should pass.)
                0x7312, 0x4000,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x1235);
            Assert.IsTrue(_cpu.Registers.C == 0x1235);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x1235);
            Assert.IsTrue(_cpu.Registers.C == 0x1235);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x4000);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x1235);
            Assert.IsTrue(_cpu.Registers.C == 0x1235);
        }

        // 0x74
        [Test]
        public void JumpToRegisterIfRegisterIsZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1234,
                0x1110, 0x2000,
                0x1120, 0x0000,
                // Jump if equal (should fail.)
                0x7401,
                // Jump if equal (should pass.)
                0x7421,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.C == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.C == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.C == 0x0000);
        }

        // 0x75
        [Test]
        public void JumpToi16IfRegisterIsZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1234,
                0x1110, 0x0000,
                // Jump if equal (should fail.)
                0x7501, 0x1000,
                // Jump if equal (should pass.)
                0x7521, 0x2000,
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0000);
        }

        // 0x76
        [Test]
        public void JumpToi16IfTwoRegistersNotEqual()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1234,
                0x1110, 0x1235,
                0x1120, 0x1235,
                // Jump if not equal (should fail.)
                0x7612, 0x2000,
                // Jump if not equal (should pass.)
                0x7601, 0x4000,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x1235);
            Assert.IsTrue(_cpu.Registers.C == 0x1235);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x1235);
            Assert.IsTrue(_cpu.Registers.C == 0x1235);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x4000);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x1235);
            Assert.IsTrue(_cpu.Registers.C == 0x1235);
        }

        // 0x77
        [Test]
        public void JumpToRegisterIfRegisterIsNotZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1234,
                0x1110, 0x2000,
                0x1120, 0x0000,
                // Jump if equal (should fail.)
                0x7721,
                // Jump if equal (should pass.)
                0x7701,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.C == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.C == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.C == 0x0000);
        }

        // 0x78
        [Test]
        public void JumpToi16IfRegisterIsNotZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1234,
                0x1110, 0x0000,
                // Jump if equal (should fail.)
                0x7821, 0x1000,
                // Jump if equal (should pass.)
                0x7801, 0x2000,
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0000);
        }

        // 0x79
        [Test]
        public void JumpToRegisterIfFlagSet()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1234,
                0x1110, 0x2000,
                // Set zero bit.
                0x2200,
                // Jump if carry set (should fail.)
                0x7911,
                // Jump if zero set (should pass.)
                0x7921,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
        }

        // 0x7A
        [Test]
        public void JumpToi16IfFlagSet()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1234,
                // Set zero bit.
                0x2200,
                // Jump if carry set (should fail.)
                0x7A10, 0x2000,
                // Jump if zero set (should pass.)
                0x7A20, 0x2000
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 3);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
        }

        // 0x7B
        [Test]
        public void JumpToRegisterIfFlagNotSet()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1234,
                0x1110, 0x2000,
                // Set zero bit.
                0x2200,
                // Jump if zero set (should fail.)
                0x7B21,
                // Jump if carry set (should pass.)
                0x7B11,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
        }

        // 0x7A
        [Test]
        public void JumpToi16IfFlagNotSet()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1234,
                // Set zero bit.
                0x2200,
                // Jump if zero set (should fail.)
                0x7C20, 0x2000,
                // Jump if carry set (should pass.)
                0x7C10, 0x2000,
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 3);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
        }
    }
}
