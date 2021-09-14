using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class GeneralArithmetic
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

        // 0x20
        [Test]
        public void AddTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0x0001,
                0x1110, 0x5000,
                0x1120, 0xFFFF,
                // Add two numbers.
                0x2001,
                // Both ways.
                0x2010,
                // Add self.
                0x2011,
                // Add acc.
                0x2081,
                // Test carry + zero
                0x2002,
                // Test carry
                0x2012,
                // Invalid register
                0x20FF
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x5000);
            Assert.IsTrue(_cpu.Registers.C == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x5001);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x5000);
            Assert.IsTrue(_cpu.Registers.C == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x5001);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x5000);
            Assert.IsTrue(_cpu.Registers.C == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xA000);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x5000);
            Assert.IsTrue(_cpu.Registers.C == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xF000);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x5000);
            Assert.IsTrue(_cpu.Registers.C == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x5000);
            Assert.IsTrue(_cpu.Registers.C == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x4FFF);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0010);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x5000);
            Assert.IsTrue(_cpu.Registers.C == 0xFFFF);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x21
        [Test]
        public void AddRegisterAndi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0x0001,
                0x1110, 0x1000,
                // Add two numbers.
                0x2100, 0x8000,
                // Add Acc.
                0x2180, 0x2000,
                // Overflow + carry.
                0x2100, 0xFFFF,
                // Overflow.
                0x2110, 0xFFFE,
                // Invalid register.
                0x21FF, 0x1230
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x8001);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xA001);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0000);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0FFE);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0010);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        //0x22
        [Test]
        public void SubTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0x0001,
                0x1110, 0x1000,
                // Subtract two numbers.
                0x2210,
                // Sub from acc.
                0x2280,
                // Subtract other way (carry.)
                0x2201,
                // Zero.
                0x2200,
                // Invalid register.
                0x22FF
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0FFF);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0001);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0FFE);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0001);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xF001);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0011);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x23
        [Test]
        public void Subi16FromRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup register.
                0x1100, 0x1000,
                // Subtract 0x0800.
                0x2300, 0x0800,
                // Subtract acc.
                0x2380, 0x0200,
                // Zero flag.
                0x2300, 0x1000,
                // Carry flag.
                0x2300, 0x2000,
                // Invalid register.
                0x23FF, 0x1234
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0800);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0001);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0600);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0001);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xF000);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0011);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x24
        [Test]
        public void SubRegisterFromi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup register.
                0x1100, 0x1000,
                // Subtract.
                0x2400, 0x1800,
                // Subtract acc.
                0x2480, 0x0E00,
                // Zero flag.
                0x2400, 0x1000,
                // Carry flag.
                0x2400, 0x0000,
                // Invalid register.
                0x24FF, 0x1234
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0800);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0001);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0600);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0001);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xF000);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0011);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x25
        [Test]
        public void Increment()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0x1000,
                0x1110, 0xFFFF,
                // Inc register
                0x2500,
                // Inc acc from 0x2000
                0x2100, 0x1000,
                0x2580,
                // Carry and zero
                0x2510,
                // Inc same register
                0x2510,
                // Invalid register
                0x25FF
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x1001);
            Assert.IsTrue(_cpu.Registers.B == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x2001);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x1001);
            Assert.IsTrue(_cpu.Registers.B == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x2002);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x1001);
            Assert.IsTrue(_cpu.Registers.B == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x2002);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0110);
            Assert.IsTrue(_cpu.Registers.A == 0x1001);
            Assert.IsTrue(_cpu.Registers.B == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x2002);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x1001);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);

            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        // 0x26
        [Test]
        public void Decrement()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Setup registers.
                0x1100, 0x1000,
                0x1110, 0x0001,
                // Dec register
                0x2600,
                // Dec acc from 0x2000
                0x2100, 0x1000,
                0x2680,
                // Zero
                0x2610,
                // Carry
                0x2610,
                // Dec same register
                0x2610,
                // Invalid register
                0x26FF
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0001);
            Assert.IsTrue(_cpu.Registers.A == 0x0FFF);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1FFF);
            Assert.IsTrue(_cpu.Registers.Flags == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x0FFF);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1FFE);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0001);
            Assert.IsTrue(_cpu.Registers.A == 0x0FFF);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1FFE);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0101);
            Assert.IsTrue(_cpu.Registers.A == 0x0FFF);
            Assert.IsTrue(_cpu.Registers.B == 0x0000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1FFE);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0011);
            Assert.IsTrue(_cpu.Registers.A == 0x0FFF);
            Assert.IsTrue(_cpu.Registers.B == 0xFFFF);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1FFE);
            Assert.IsTrue(_cpu.Registers.Flags == 0b0001);
            Assert.IsTrue(_cpu.Registers.A == 0x0FFF);
            Assert.IsTrue(_cpu.Registers.B == 0xFFFE);

            
            // Test invalid register.
            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }
    }
}
