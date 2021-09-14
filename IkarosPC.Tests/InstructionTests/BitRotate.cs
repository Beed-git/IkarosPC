using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class BitRotate
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

        // 0x5B
        [Test]
        public void RotateLeftTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b1101_1100_1001_1110,
                0x1110, 0x0001,
                0x1120, 0x0003,
                0x1130, 0x00FC,
                // Set flags register to random value.
                0x2110, 0xFFFF,
                // Rotate left once.
                0x5B01,
                // Rotate left 3x.
                0x5B02,
                // Rotate $ACC 3x.
                0x5B82,
                // Rotate by more than 0x10 times.
                0x5B03,
                // Invalid register.
                0x5BFF, 0x1234
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x00FC);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1011_1001_0011_1101);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x00FC);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1110_0100_1111_0110);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x00FC);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 13);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0010_0111_1011_0111);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x00FC);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 14);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1110_1101_1100_1001);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x00FC);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x5C
        [Test]
        public void RotateLeftRegisterByi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b1101_1100_1001_1110,
                // Set flags register to random value.
                0x2200,
                // Rotate left once.
                0x5C00, 0x0001,
                // Rotate left 3x.
                0x5C00, 0x0003,
                // Rotate $ACC 3x.
                0x5C80, 0x0003,
                // Rotate by more than 0x10 times.
                0x5C03, 0x00FC,
                // Invalid register.
                0x5CFF, 0x1234
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 3);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1011_1001_0011_1101);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1110_0100_1111_0110);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0010_0111_1011_0111);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1110_1101_1100_1001);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x5D
        [Test]
        public void RotateRightTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b1101_1100_1001_1110,
                0x1110, 0x0001,
                0x1120, 0x0003,
                0x1130, 0x00FC,
                // Set flags register to random value.
                0x2110, 0xFFFF,
                // Rotate right once.
                0x5D01,
                // Rotate right 3x.
                0x5D02,
                // Rotate $ACC 3x.
                0x5D82,
                // Rotate by more than 0x10 times.
                0x5D03,
                // Invalid register.
                0x5DFF, 0x1234
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x00FC);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0110_1110_0100_1111);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x00FC);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1101_1011_1001_0011);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x00FC);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 13);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0111_1011_0111_0010);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x00FC);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 14);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1100_1001_1110_1101);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x00FC);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x5E
        [Test]
        public void RotateRightRegisterByi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b1101_1100_1001_1110,
                // Set flags register to random value.
                0x2200,
                // Rotate right once.
                0x5E00, 0x0001,
                // Rotate right 3x.
                0x5E00, 0x0003,
                // Rotate $ACC 3x.
                0x5E80, 0x0003,
                // Rotate by more than 0x10 times.
                0x5E03, 0x00FC,
                // Invalid register.
                0x5EFF, 0x1234
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 3);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0110_1110_0100_1111);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1101_1011_1001_0011);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0111_1011_0111_0010);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1100_1001_1110_1101);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1100_1001_1110);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }
    }
}
