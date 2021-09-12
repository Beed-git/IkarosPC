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
        private CPU _cpu;
        private Memory _memory;

        [SetUp]
        public void Setup()
        {
            _memory = new Memory();
            _cpu = new CPU(_memory);

            _cpu.Reset();

            SetupHelpers.CheckInitialCPUState(_cpu);
        }

        [Test]
        public void SwapValuesTest()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Load values into register.
                0x1300, 0xFFEE,
                0x1310, 0xAABB,
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
