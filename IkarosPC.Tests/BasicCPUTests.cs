using NUnit.Framework;
using IkarosPC;
using System;

namespace IkarosPC.Tests
{    
    public class BasicCPUTests
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
        public void TestCpuCanExecuteNOPInstruction()
        {
            // Sets first 16 bytes to 0.
            _memory.SetInitialMemory(new ushort[16]);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 1);
        }

        [Test]
        public void TestGetInvalidRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Store 0xFFFF into invalid register. (0xF)
                0x13F0, 0xFFFF
            });

            Assert.Throws<IndexOutOfRangeException>(_cpu.Step);
        }

        [Test]
        public void TestCanPushOntoStack()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Store 0x1234 in Register A
                0x1300, 0x1234,
                // Push value onto stack.
                0x0200,
                // Push literal onto stack (Should ignore last two values)
                0x03FF, 0xFFEE
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.SP == _cpu.Registers.TopOfStack);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack] == 0);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack - 1] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 3);
            Assert.IsTrue(_cpu.Registers.SP == _cpu.Registers.TopOfStack - 1);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack] == 0x1234);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack - 1] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.SP == _cpu.Registers.TopOfStack - 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack] == 0x1234);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack - 1] == 0xFFEE);

        }

        [Test]
        public void TestCanPopOffStack()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Push literal onto stack
                0x0300, 0x1234,
                0x03FF, 0xFF00,
                // Pop value off stack and store in register B
                0x0410,
                // Pop value off stack and strore in register A
                0x040F,
                // Push literal back onto stack
                0x0300, 0xAAAA,
                // Push literal back onto stack
                0x0300, 0xBBBB
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.SP == _cpu.Registers.TopOfStack - 2);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack] == 0x1234);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack - 1] == 0xFF00);
            
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.SP == _cpu.Registers.TopOfStack - 1);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0xFF00);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack] == 0x1234);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack - 1] == 0xFF00);
            
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.SP == _cpu.Registers.TopOfStack);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0xFF00);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack] == 0x1234);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack - 1] == 0xFF00);
            
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.SP == _cpu.Registers.TopOfStack - 1);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0xFF00);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack] == 0xAAAA);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack - 1] == 0xFF00);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.SP == _cpu.Registers.TopOfStack - 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0xFF00);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack] == 0xAAAA);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack - 1] == 0xBBBB);

        }

        [Test]
        public void TestFlagsRegister()
        {
            // Get flags
            Assert.IsTrue(_cpu.Registers.Flags == 0);

            _cpu.Registers.Zero = true;
            _cpu.Registers.Carry = false;
            _cpu.Registers.Negative = false;

            Assert.IsTrue(_cpu.Registers.Flags == 0b00000100);

            _cpu.Registers.Zero = false;
            _cpu.Registers.Carry = true;
            _cpu.Registers.Negative = false;

            Assert.IsTrue(_cpu.Registers.Flags == 0b00000010);

            _cpu.Registers.Zero = false;
            _cpu.Registers.Carry = false;
            _cpu.Registers.Negative = true;

            Assert.IsTrue(_cpu.Registers.Flags == 0b00000001);

            _cpu.Registers.Zero = true;
            _cpu.Registers.Carry = false;
            _cpu.Registers.Negative = true;

            Assert.IsTrue(_cpu.Registers.Flags == 0b00000101);

            _cpu.Registers.Zero = true;
            _cpu.Registers.Carry = true;
            _cpu.Registers.Negative = true;

            Assert.IsTrue(_cpu.Registers.Flags == 0b00000111);

            _cpu.Registers.Zero = true;
            _cpu.Registers.Carry = true;
            _cpu.Registers.Negative = false;

            Assert.IsTrue(_cpu.Registers.Flags == 0b00000110);

            // Set flags
            _cpu.Registers.Flags = 0b00000101;
            Assert.IsTrue(_cpu.Registers.Flags == 0b00000101);

            _cpu.Registers.Flags = 0b00000111;
            Assert.IsTrue(_cpu.Registers.Flags == 0b00000111);

            _cpu.Registers.Flags = 0b00000100;
            Assert.IsTrue(_cpu.Registers.Flags == 0b00000100);

            _cpu.Registers.Flags = 0b00000001;
            Assert.IsTrue(_cpu.Registers.Flags == 0b00000001);

            _cpu.Registers.Flags = 0b10101010;
            Assert.IsTrue(_cpu.Registers.Flags == 0b00000010);

            _cpu.Registers.Flags = 0b11111111;
            Assert.IsTrue(_cpu.Registers.Flags == 0b00000111);

            _cpu.Registers.Flags = 0b11111000;
            Assert.IsTrue(_cpu.Registers.Flags == 0b00000000);
        }

        [Test]
        public void TestStopped()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Store initial value in A.
                0x1300, 0x1234,
                // Halt execution.
                0x0100,
                // Try replace value of A (shouldn't run)
                0x1300, 0xFFFF
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 3);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 3);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
        }
    }
}