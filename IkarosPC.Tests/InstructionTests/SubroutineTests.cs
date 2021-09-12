using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class SubroutineTests
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

        [Test]
        public void CallFromLiteral()
        {
            _memory.SetInitialMemory(new ushort[] {
                // Set values for general registers.
                0x1300, 0x0001,
                0x1310, 0x0002,
                0x1320, 0x0003,
                0x1330, 0x0004,
                0x1340, 0x0005,
                0x1350, 0x0006,
                0x1360, 0x0007,
                0x1370, 0x0008,
                // Push random values to move stack
                0x0300, 0xFFFF,
                0x0300, 0x1234,
                0x0300, 0x4321,
                0x0300, 0xD9D9,
                // Subtract register A from itself to set flags register
                0x2200,
                // Store random value into accumulator (Shouldn't be saved)
                0x1380, 0xF123,
                // Push 0 arguments to stack.
                0x0300, 0x0000,
                // Call subroutine
                0x0500, 0x3000,
                // Finished

            });
            // Returns 3 + 5
            _memory.SetSubroutineAtAddress(0x3000, new ushort[] {
                // Push random values to stack
                0x0300, 0x9191,
                0x0300, 0xF4F4,
                // Store random values in registers (including accumulator)
                0x1300, 0x11FF,
                0x1310, 0x22FF,
                0x1320, 0x33FF,
                0x1330, 0x44FF,
                0x1380, 0xFFFF,
                // Push 0 arguments to stack.
                0x0300, 0x0000,
                // Nested subroutine call. (Gets the value 5)
                0x0500, 0x5000,
                // Move value from accumulator into register A
                0x1080,
                // Add 3 to value
                0x2100, 0x0003,
                // Return from subroutine.
                0x0700
            });
            // Returns the number 5.
            _memory.SetSubroutineAtAddress(0x5000, new ushort[]
            {
                // Push random values to stack.
                0x0300, 0x5468,
                0x0300, 0x898A,
                // Store numbers in register A and B.
                0x1300, 0x0003,
                0x1310, 0x0002,
                // Add the two numbers together (storing the value in the accumulator)
                0x2010,
                // Return
                0x0700
            });

            // Load registers with random values.
            for (int i = 0; i < 15; i++)
            {
                _cpu.Step();
            }

            // Check state
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x0002);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x0004);
            Assert.IsTrue(_cpu.Registers.E == 0x0005);
            Assert.IsTrue(_cpu.Registers.X == 0x0006);
            Assert.IsTrue(_cpu.Registers.Y == 0x0007);
            Assert.IsTrue(_cpu.Registers.Z == 0x0008);

            Assert.IsTrue(_cpu.Registers.Flags == 0b00000101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xF123);

            Assert.IsTrue(_cpu.Registers.PC == 29);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 5);

            // First subroutine call
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x3000);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 16);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 16);

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            // Check state with values replaced
            Assert.IsTrue(_cpu.Registers.PC == 0x3010);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 16);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 19);

            Assert.IsTrue(_cpu.Registers.A == 0x11FF);
            Assert.IsTrue(_cpu.Registers.B == 0x22FF);
            Assert.IsTrue(_cpu.Registers.C == 0x33FF);
            Assert.IsTrue(_cpu.Registers.D == 0x44FF);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xFFFF);

            // Nested call
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x5000);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 30);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 30);

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            // Check state
            Assert.IsTrue(_cpu.Registers.PC == 0x5009);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 30);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 32);

            Assert.IsTrue(_cpu.Registers.A == 0x0003);
            Assert.IsTrue(_cpu.Registers.B == 0x0002);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0005);

            // Return from nested function
            _cpu.Step();

            // Check state is back to previous
            Assert.IsTrue(_cpu.Registers.PC == 0x3012);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 16);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 18);

            Assert.IsTrue(_cpu.Registers.A == 0x11FF);
            Assert.IsTrue(_cpu.Registers.B == 0x22FF);
            Assert.IsTrue(_cpu.Registers.C == 0x33FF);
            Assert.IsTrue(_cpu.Registers.D == 0x44FF);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0005);

            _cpu.Step();
            _cpu.Step();

            // Check state
            Assert.IsTrue(_cpu.Registers.PC == 0x3015);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 16);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 18);

            Assert.IsTrue(_cpu.Registers.A == 0x0005);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0008);

            // Return
            _cpu.Step();

            // Check state is back to original.
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x0002);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x0004);
            Assert.IsTrue(_cpu.Registers.E == 0x0005);
            Assert.IsTrue(_cpu.Registers.X == 0x0006);
            Assert.IsTrue(_cpu.Registers.Y == 0x0007);
            Assert.IsTrue(_cpu.Registers.Z == 0x0008);

            Assert.IsTrue(_cpu.Registers.Flags == 0b00000101);
            // 8 should be the return value from the two routine calls.
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0008);

            Assert.IsTrue(_cpu.Registers.PC == 31);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 4);
        }

        [Test]
        public void TestCallFromRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Set values for general registers.
                0x1300, 0x0001,
                0x1310, 0x0002,
                0x1320, 0x0003,
                0x1330, 0x4000,
                0x1340, 0x0005,
                0x1350, 0x0006,
                0x1360, 0x0007,
                0x1370, 0x0008,
                // Push random values to move stack
                0x0300, 0xFFFF,
                0x0300, 0x1234,
                0x0300, 0x4321,
                0x0300, 0xD9D9,
                // Subtract register A from itself to set flags register
                0x2200,
                // Store random value into accumulator (Shouldn't be saved)
                0x1380, 0xF123,
                // Push 0 arguments to stack.
                0x0300, 0x0000,
                // Call subroutine
                0x0630,
                // Psuh random value to stack
                0x0300, 0x1F1F,
            });

            _memory.SetSubroutineAtAddress(0x4000, new ushort[]
            {
                // Push random values to stack
                0x0300, 0x9191,
                0x0300, 0xF4F4,
                // Store random values in registers (including accumulator)
                0x1300, 0x11FF,
                0x1310, 0x22FF,
                0x1320, 0x33FF,
                0x1330, 0x44FF,
                0x1380, 0xFFFF,
                // Store address of next subroutine
                0x1340, 0x2000,
                // Push 0 arguments to stack.
                0x0300, 0x0000,
                // Nested subroutine call. (Gets the value 5)
                0x0640,
                // Move value from accumulator into register A
                0x1080,
                // Add 3 to value
                0x2100, 0x0003,
                // Return from subroutine.
                0x0700
            });

            // Returns the number 5.
            _memory.SetSubroutineAtAddress(0x2000, new ushort[]
            {
                // Push random values to stack.
                0x0300, 0x5468,
                0x0300, 0x898A,
                // Store numbers in register A and B.
                0x1300, 0x0003,
                0x1310, 0x0002,
                // Add the two numbers together (storing the value in the accumulator)
                0x2010,
                // Return
                0x0700
            });

            // Load registers with random values.
            for (int i = 0; i < 15; i++)
            {
                _cpu.Step();
            }

            // Check state
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x0002);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x4000);
            Assert.IsTrue(_cpu.Registers.E == 0x0005);
            Assert.IsTrue(_cpu.Registers.X == 0x0006);
            Assert.IsTrue(_cpu.Registers.Y == 0x0007);
            Assert.IsTrue(_cpu.Registers.Z == 0x0008);

            Assert.IsTrue(_cpu.Registers.Flags == 0b00000101);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xF123);

            Assert.IsTrue(_cpu.Registers.PC == 29);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 5);

            // First subroutine
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x4000);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 16);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 16);

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            // Check state with values replaced
            Assert.IsTrue(_cpu.Registers.PC == 0x4012);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 16);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 19);

            Assert.IsTrue(_cpu.Registers.A == 0x11FF);
            Assert.IsTrue(_cpu.Registers.B == 0x22FF);
            Assert.IsTrue(_cpu.Registers.C == 0x33FF);
            Assert.IsTrue(_cpu.Registers.D == 0x44FF);
            Assert.IsTrue(_cpu.Registers.E == 0x2000);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0xFFFF);

            // Nested call
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 30);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 30);

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            // Check state
            Assert.IsTrue(_cpu.Registers.PC == 0x2009);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 30);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 32);

            Assert.IsTrue(_cpu.Registers.A == 0x0003);
            Assert.IsTrue(_cpu.Registers.B == 0x0002);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0005);

            // Return from nested function
            _cpu.Step();

            // Check state is back to previous
            Assert.IsTrue(_cpu.Registers.PC == 0x4013);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 16);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 18);

            Assert.IsTrue(_cpu.Registers.A == 0x11FF);
            Assert.IsTrue(_cpu.Registers.B == 0x22FF);
            Assert.IsTrue(_cpu.Registers.C == 0x33FF);
            Assert.IsTrue(_cpu.Registers.D == 0x44FF);
            Assert.IsTrue(_cpu.Registers.E == 0x2000);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0005);

            _cpu.Step();
            _cpu.Step();

            // Check state
            Assert.IsTrue(_cpu.Registers.PC == 0x4016);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue - 16);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 18);

            Assert.IsTrue(_cpu.Registers.A == 0x0005);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0008);

            // Return
            _cpu.Step();

            // Check state is back to original.
            Assert.IsTrue(_cpu.Registers.A == 0x0001);
            Assert.IsTrue(_cpu.Registers.B == 0x0002);
            Assert.IsTrue(_cpu.Registers.C == 0x0003);
            Assert.IsTrue(_cpu.Registers.D == 0x4000);
            Assert.IsTrue(_cpu.Registers.E == 0x0005);
            Assert.IsTrue(_cpu.Registers.X == 0x0006);
            Assert.IsTrue(_cpu.Registers.Y == 0x0007);
            Assert.IsTrue(_cpu.Registers.Z == 0x0008);

            Assert.IsTrue(_cpu.Registers.Flags == 0b00000101);
            // 8 should be the return value from the two routine calls.
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x0008);

            Assert.IsTrue(_cpu.Registers.PC == 30);
            Assert.IsTrue(_cpu.Registers.FP == ushort.MaxValue);
            Assert.IsTrue(_cpu.Registers.SP == ushort.MaxValue - 4);
        }

        [Test]
        public void TestReturnWithoutCall()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                0x0700
            });

            // Honestly idk what should happen.
            // Probably cause an exception?

            _cpu.Step();

            Assert.IsTrue(1 == 2);
        }

    }
}
