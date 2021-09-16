using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class Jump
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
        // 0x70
        [Test]
        public void JumpToRegister()
        {
            _memory.SetInitialMemory(new ushort[] 
            {
                // Init registers.
                0x1100, 0x4000,
                0x1110, 0xE000,
                // Jump to first register.
                0x7000,
            });

            _memory.SetSubroutineAtAddress(0x4000, new ushort[]
            {
                // Jump to second register.
                0x7010,
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x4000);
            Assert.IsTrue(_cpu.Registers.B == 0xE000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x4000);
            Assert.IsTrue(_cpu.Registers.A == 0x4000);
            Assert.IsTrue(_cpu.Registers.B == 0xE000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0xE000);
            Assert.IsTrue(_cpu.Registers.A == 0x4000);
            Assert.IsTrue(_cpu.Registers.B == 0xE000);
        }

        // 0x71
        [Test]
        public void JumpToi16()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Jump to first register.
                0x7100, 0x4000
            });

            _memory.SetSubroutineAtAddress(0x4000, new ushort[]
            {
                // Jump to second register.
                0x7100, 0xE000
            });

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x4000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0xE000);
        }

        // 0x72
        [Test]
        public void TestToi16WithOffset()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x1000,
                0x1110, 0x2000,
                // Jump to first register.
                0x7200, 0x3000,
            });

            _memory.SetSubroutineAtAddress(0x4000, new ushort[]
            {
                // Jump to second register.
                0x7210, 0xC000,
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x4000);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0xE000);
            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 0x2000);
        }

    }
}
