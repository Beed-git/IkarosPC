using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.MiniProgramTests
{
    class BSMCompilation
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
        public void TestSubroutineCall()
        {
            _memory.SetInitialMemory(new ushort[] {
                0x1100, 0x1234, 0x1110, 0x4321, 0x0300, 0x0000, 0x0600, 0x0009, 0x0100, 0x1100, 0x1200, 0x1110, 0x1000, 0x2001, 0x0700
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x4321);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.A == 0x1200);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.Accumulator == 0x2200);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);

            _cpu.Step();

            Assert.IsTrue(_cpu.Stopped == true);
            
        }

    }
}
