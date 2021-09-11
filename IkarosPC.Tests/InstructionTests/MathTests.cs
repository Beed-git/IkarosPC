using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class MathTests
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

        [Test]
        public void TestAddTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[] {
                // Setup registers.
                0x1300, 0x1234,
                0x1310, 0x4321,
                // Add two registers.
                0x2001,
                // Check both ways.
                0x2010,
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x4321);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x4321);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x5555);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x4321);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x5555);
        }

        [Test]
        public void TestAddTwoRegistersFlags()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers
                0x1300, 0xFFFF,
                0x1310, 0x0001,
                0x1320, 0x1234,
                // Test no flags
                0x2012,
                // Test 0 & carry
                0x2001,
                // Test just carry
                0x2020
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0xFFFF);
            Assert.IsTrue(_cpu.Registers.B == 0x0001);
            Assert.IsTrue(_cpu.Registers.C == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1235);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0000);

            Assert.IsTrue(_cpu.Registers.Zero == true);
            Assert.IsTrue(_cpu.Registers.Carry == true);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1233);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == true);
            Assert.IsTrue(_cpu.Registers.Negative == false);
        }

        [Test]
        public void TestAddRegisterAndLiteral()
        {
            _memory.SetInitialMemory(new ushort[] { 
                // Init register.
                0x1300, 0x1234,
                // Add literal.
                0x2100, 0x4321,
                // Test with 0x000D
                0x210D, 0x5432
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x5555);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x6666);
        }

        [Test]
        public void TestAddRegisterAndLiteralFlags()
        {
            _memory.SetInitialMemory(new ushort[] { 
                // Init register.
                0x1300, 0x1234,
                // Test no flags.
                0x2100, 0x4321,
                // Test carry & zero.
                0x2100, 0xEDCC,
                // Test just carry.
                0x2100, 0xFFFF
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x5555);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            Assert.IsTrue(_cpu.Registers.Zero == true);
            Assert.IsTrue(_cpu.Registers.Carry == true);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1233);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == true);
            Assert.IsTrue(_cpu.Registers.Negative == false);
        }

        [Test]
        public void TestSubtractTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[] { 
                // Init registers
                0x1300, 0x1234,
                0x1310, 0x0100,
                // Subtract rX from rY
                0x2201,
                // Subtract rY from rX
                0x2210,
                // Subtract from self.
                0x2211
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0100);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0100);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1134);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0100);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xEECC);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0100);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
        }

        [Test]
        public void TestSubtractTwoRegistersFlags()
        {
            _memory.SetInitialMemory(new ushort[] { 
                // Init registers.
                0x1300, 0x1234,
                0x1310, 0x0100,
                // No flags. (Except negative)
                0x2201,
                // carry, and negative.
                0x2210,
                // Zero and negative
                0x2211
                
                // Note: Can zero and carry occur at same time?
            });

            _cpu.Step();
            _cpu.Step();


            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0100);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1134);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == true);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xEECC);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == true);
            Assert.IsTrue(_cpu.Registers.Negative == true);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            Assert.IsTrue(_cpu.Registers.Zero == true);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == true);
        }

        [Test]
        public void TestSubtractLiteralFromRegister()
        {
            _memory.SetInitialMemory(new ushort[] {
                // Init register.
                0x1300, 0x1234,
                // Test subtract literal.
                0x2300, 0x0100,
                // Test subtract literal with 0x000D
                0x230D, 0x5151
            });

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1134);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xC0E3);
        }

        [Test]
        public void TestSubtractLiteralFromRegisterFlags()
        {
            _memory.SetInitialMemory(new ushort[] {
                // Init register.
                0x1300, 0x1234,
                // Test just subtract.
                0x2300, 0x0100,
                // Test carry and subtract.
                0x230D, 0x5151,
                // Test zero and subtract.
                0x2300, 0x1234
            });

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x1134);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == true);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xC0E3);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == true);
            Assert.IsTrue(_cpu.Registers.Negative == true);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            Assert.IsTrue(_cpu.Registers.Zero == true);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == true);
        }

        [Test]
        public void TestSubtractRegisterFromLiteral()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init register.
                0x1300, 0x1234,
                // Test subtract literal.
                0x2400, 0x1111,
                // Test subtract literal with 0x000E.
                0x240E, 0xFF00
            });

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xFEDD);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xECCC);
        }

        [Test]
        public void TestSubtractRegisterFromLiteralFlags()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init register.
                0x1300, 0x1234,
                // Test only negative.
                0x2400, 0x2000,
                // Test negative and carry.
                0x2401, 0x0100,
                // Test negative and zero.
                0x2400, 0x1234
            });

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0DCC);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == true);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xEECC);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == true);
            Assert.IsTrue(_cpu.Registers.Negative == true);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            Assert.IsTrue(_cpu.Registers.Zero == true);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == true);
        }

        [Test]
        public void TestMultiplyTwoRegisters()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1300, 0x1000,
                0x1310, 0x000A,
                // Multiply two registers.
                0x2501,
                // Other way
                0x2510
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x000A);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x000A);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xA000);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x000A);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xA000);
        }

        [Test]
        public void TestMultiplyTwoRegistersFlags()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1300, 0x1000,
                0x1310, 0x0010,
                0x1320, 0xFEFE,
                // Check no flags.
                0x2511,
                // Check just carry.
                0x2512,
                // Check carry & zero.
                0x2501
            });
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x0010);
            Assert.IsTrue(_cpu.Registers.C == 0xFEFE);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0100);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == false);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xEFE0);

            Assert.IsTrue(_cpu.Registers.Zero == false);
            Assert.IsTrue(_cpu.Registers.Carry == true);
            Assert.IsTrue(_cpu.Registers.Negative == false);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);

            Assert.IsTrue(_cpu.Registers.Zero == true);
            Assert.IsTrue(_cpu.Registers.Carry == true);
            Assert.IsTrue(_cpu.Registers.Negative == false);
        }

        [Test]
        public void TestAddFromAccumulator()
        {
            // Add with accumulator as input.
            Assert.IsTrue(1 == 2);
        }

        [Test]
        public void TestSubFromAccumulator()
        {
            Assert.IsTrue(1 == 2);
        }

        [Test]
        public void TestDivideByZero()
        {
            Assert.IsTrue(1 == 2);
        }
    }
}
