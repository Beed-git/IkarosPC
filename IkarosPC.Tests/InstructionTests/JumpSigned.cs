using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class JumpSigned
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
        // 0x80
        [Test]
        public void JumpToi16IfRegisterGreaterThanRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                // -500
                0x1110, 0xFB00,
                // -1000
                0x1120, 0xF000,
                // Jump if greater than (should fail.)
                0x8011, 0x2000,
                // Jump if greater than (should pass.)
                0x8012, 0x4000,
            });

            _memory.SetSubroutineAtAddress(0x4000, new ushort[]
            {
                0x8001, 0x2000
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0xFB00);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0xFB00);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x4000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0xFB00);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0xFB00);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);
        }

        // 0x81
        [Test]
        public void JumpToRegisterIfRegisterGreaterThanZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                // 1000
                0x1110, 0x1000,
                // -1000
                0x1120, 0xF000,
                // Jump if greater than (should fail.)
                0x8120,
                // Jump if greater than (should fail.)
                0x8100,
                // Jump if greater than (should pass.)
                0x8111,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);
        }

        // 0x82
        [Test]
        public void JumpToi16IfRegisterGreaterThanZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                // 1000
                0x1110, 0x1000,
                // -1000
                0x1120, 0xF000,
                // Jump if greater than zero (should fail.)
                0x8220, 0x1000,
                // Jump if greater than zero (should fail.)
                0x8200, 0x1000,
                // Jump if greater than zero (should pass.)
                0x8210, 0x1000,
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

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);
        }
    }
}
