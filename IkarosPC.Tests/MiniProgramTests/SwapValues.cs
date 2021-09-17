using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.MiniProgramTests
{
    class SwapValues
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

            // Check initial state of all registers and flags.
            SetupHelpers.CheckInitialCPUState(_cpu);
        }

        [Test]
        public void SwapValuesTest()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Load values into register.
                0x1100, 0xFFEE,
                0x1110, 0xAABB,
                // Push both values to stack.
                0x0200, 
                0x0210,
                // Pop both values in reverse order.
                0x0400,
                0x0410,
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.A == 0xFFEE);
            Assert.IsTrue(_cpu.Registers.B == 0xAABB);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack] == 0);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack - 1] == 0);

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.SP == _cpu.Registers.TopOfStack - 2);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack] == 0xFFEE);
            Assert.IsTrue(_memory.Stack[_cpu.Registers.TopOfStack - 1] == 0xAABB);

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.SP == _cpu.Registers.TopOfStack);
            Assert.IsTrue(_cpu.Registers.A == 0xAABB);
            Assert.IsTrue(_cpu.Registers.B == 0xFFEE);
        }
    }
}
