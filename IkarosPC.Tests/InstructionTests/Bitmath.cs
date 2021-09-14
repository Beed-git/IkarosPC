using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class Bitmath
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

        // 0x50
        [Test]
        public void AndTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b0101_1111_1111_0101,
                0x1110, 0b0101_0101_0101_1111,
                0x1120, 0b1111_1111_0000_0000,
                0x1130, 0x0000,
                // And two registers.
                0x5001,
                // Check other way.
                0x5010,
                // And with $ACC.
                0x5082,
                // Check zero flag.
                0x5083,
                // Invalid registers.
                0x50FF
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0101_0101_0101_0101);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);
            
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0101_0101_0101_0101);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0101_0101_0000_0000);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x51
        [Test]
        public void AndRegisterWithi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b0101_1111_1111_0101,
                // And two registers.
                0x5100, 0xFF00,
                // And with $ACC.
                0x5180, 0x0FF0,
                // Check zero flag.
                0x5100, 0x0000,
                // Invalid registers.
                0x51FF
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0101_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0F00);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            
            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x52
        [Test]
        public void OrTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b0101_1111_1111_0101,
                0x1110, 0b0101_0101_0101_1111,
                0x1120, 0b1111_1111_0000_0000,
                0x1130, 0x0000,
                // Or two registers.
                0x5201,
                // Check other way.
                0x5210,
                // Or with $ACC.
                0x5282,
                // Check zero flag.
                0x5233,
                // Invalid registers.
                0x52FF
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0101_1111_1111_1111);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0101_1111_1111_1111);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x53
        [Test]
        public void OrRegisterWithi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b0101_1111_1111_0101,
                0x1110, 0x0000,
                // Or two registers.
                0x5300, 0xFF00,
                // Or with $ACC.
                0x5380, 0x000F,
                // Check zero flag.
                0x5311, 0x0000,
                // Invalid registers.
                0x53FF
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1111_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x54
        [Test]
        public void XorTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b1101_1111_1111_0101,
                0x1110, 0b0101_0101_0101_1111,
                0x1120, 0b1111_1111_0000_0000,
                0x1130, 0x0000,
                // Xor two registers.
                0x5401,
                // Check other way.
                0x5410,
                // Xor with $ACC.
                0x5482,
                // Check zero flag.
                0x5433,
                // Invalid registers.
                0x54FF
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1000_1010_1010_1010);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1000_1010_1010_1010);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0111_0101_1010_1010);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b1101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0b0101_0101_0101_1111);
            Assert.IsTrue(_cpu.Registers.C == 0b1111_1111_0000_0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x55
        [Test]
        public void XorRegisterWithi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b0101_1111_1111_0101,
                0x1110, 0x0000,
                // Xor two registers.
                0x5500, 0xFF00,
                // Xor with $ACC.
                0x5580, 0x000F,
                // Check zero flag.
                0x5511, 0x0000,
                // Invalid registers.
                0x55FF
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);
            Assert.IsTrue(_cpu.Registers.B == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1010_0000_1111_0101);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1010_0000_1111_1010);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b0101_1111_1111_0101);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x56
        [Test]
        public void NotRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b1111_0000_1010_1001,
                0x1110, 0xFFFF,
                // Not the first register.
                0x5600,
                // Not $ACC.
                0x5680,
                // Check zero flag.
                0x5610,
                // Invalid registers.
                0x56FF
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0000_1111_0101_0110);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0xFFFF);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x57
        [Test]
        public void LeftLogicalShiftTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0b1111_0000_1010_1001,
                0x1110, 0x0001,
                0x1120, 0x0004,
                0x1130, 0xFFFF,
                0x1140, 0x0010,
                // Left shift once.
                0x5701,
                // Left shift $ACC.
                0x5781,
                // Left shift by more than one.
                0x5702,
                // Check zero.
                0x5703,
                // Check zero and carry.
                0x5704,
                // Invalid register.
                0x57FF
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.E == 0x0010);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1110_0001_0101_0010);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0010);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.E == 0x0010);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b1100_0010_1010_0100);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0010);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.E == 0x0010);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 13);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0b0000_1010_1001_0000);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0010);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.E == 0x0010);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 14);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0100);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.E == 0x0010);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 15);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.A == 0b1111_0000_1010_1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x0004);
            Assert.IsTrue(_cpu.Registers.D == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.E == 0x0010);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
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
