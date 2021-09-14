using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class BitShift
    {
        Memory _memory;
        CPU _cpu;

        [SetUp]
        public void Setup()
        {
            _memory = new Memory();
            _cpu = new CPU(_memory);

            _cpu.Reset();

            // Check initial state of all registers and flags.
            SetupHelpers.CheckInitialCPUState(_cpu);
        }

        // 0x58
        [Test]
        public void LeftLogicalShiftRegisterWithi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b1111_0000_1010_1001,
                // Left shift once.
                0x5800, 0x0001,
                // Left shift $ACC.
                0x5880, 0x0001,
                // Left shift by more than one.
                0x5800, 0x0004,
                // Check zero.
                0x5800, 0xFFFF,
                // Check zero and carry.
                0x5800, 0x0010,
                // Invalid register.
                0x58FF, 0x1234
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1110_0001_0101_0010);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0010);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1100_0010_1010_0100);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0010);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0000_1010_1001_0000);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0010);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x59
        [Test]
        public void RightLogicalShiftTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b1111_0000_1010_1001,
                0x1110, 0x0001,
                0x1120, 0x0004,
                0x1130, 0xFFFF,
                // Right shift once.
                0x5901,
                // Right shift $ACC.
                0x5981,
                // Set carry then right shift by 4.
                0x2100, 0xFFFF,
                0x5902,
                // Check zero.
                0x5903,
                // Invalid register.
                0x59FF
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0111_1000_0101_0100);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0011_1100_0010_1010);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 13);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0001_1111_0000_1010);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 14);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x5A
        [Test]
        public void RightLogicalShiftRegisterWithi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b1111_0000_1010_1001,
                // Right shift once.
                0x5A00, 0x0001,
                // Right shift $ACC.
                0x5A80, 0x0001,
                // Set carry then right shift by 4.
                0x2100, 0xFFFF,
                0x5A00, 0x0004,
                // Check zero.
                0x5A03, 0xFFFF,
                // Invalid register.
                0x5AFF, 0x1234
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0111_1000_0101_0100);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0011_1100_0010_1010);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0001_1111_0000_1010);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }
    }
}
