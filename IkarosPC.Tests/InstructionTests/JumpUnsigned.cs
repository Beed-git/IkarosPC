using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class JumpUnsigned
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

        // 0x90
        [Test]
        public void JumpToi16IfRegisterAboveRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                0x1110, 0x1000,
                0x1120, 0xF000,
                // Jump if greater than (should fail.)
                0x9011, 0x2000,
                // Jump if greater than (should fail.)
                0x9012, 0x2000,
                // Jump if greater than (should pass.)
                0x9021, 0x2000,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);
        }
        
        // 0x91
        [Test]
        public void JumpToi16IfRegisterBelowRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                0x1110, 0x1000,
                0x1120, 0xF000,
                // Jump if less than (should fail.)
                0x9111, 0x2000,
                // Jump if less than (should fail.)
                0x9120, 0x2000,
                // Jump if less than (should pass.)
                0x9112, 0x2000,

            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);
        }
        
        // 0x92
        [Test]
        public void JumpToi16IfRegisterAboveEqualRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                0x1110, 0x1000,
                0x1120, 0xF000,
                // Jump if greater than (should fail.)
                0x9212, 0x2000,
                // Jump if greater than (should pass.)
                0x9211, 0x2000,
            });

            _memory.SetSubroutineAtAddress(0x2000, new ushort[]
            {
                // Jump if greater than (should pass.)
                0x9221, 0x2000,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);
        }
        
        // 0x93
        [Test]
        public void JumpToi16IfRegisterBelowEqualRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                0x1110, 0x1000,
                0x1120, 0xF000,
                // Jump if less than (should fail.)
                0x9320, 0x2000,
                // Jump if less than (should pass.)
                0x9311, 0x2000,
            });

            _memory.SetSubroutineAtAddress(0x2000, new ushort[]
            {
                // Jump if less than (should pass.)
                0x9312, 0x2000,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);
        }
    }
}
