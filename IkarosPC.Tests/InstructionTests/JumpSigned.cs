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
                0x8011, 0x4000,
                // Jump if greater than (should fail.)
                0x8010, 0x4000,
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

            Assert.IsTrue(_cpu.Registers.PC == 10);
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

        // 0x83
        [Test]
        public void JumpToi16IfRegisterLessThanRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                // -500
                0x1110, 0xFB00,
                // -1000
                0x1120, 0xF000,
                // Jump if less than (should fail.)
                0x8311, 0x2000,
                // Jump if less than (should fail.)
                0x8301, 0x2000,
                // Jump if less than (should pass.)
                0x8321, 0x4000,
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

            Assert.IsTrue(_cpu.Registers.PC == 10);
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

        // 0x84
        [Test]
        public void JumpToRegisterIfRegisterLessThanZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                // 1000
                0x1110, 0x1000,
                // -1000
                0x1120, 0xF000,
                // Jump if less than (should fail.)
                0x8411,
                // Jump if less than (should fail.)
                0x8401,
                // Jump if less than (should pass.)
                0x8421,
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

        // 0x85
        [Test]
        public void JumpToi16IfRegisterLessThanZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                // 1000
                0x1110, 0x1000,
                // -1000
                0x1120, 0xF000,
                // Jump if less than zero (should fail.)
                0x8510, 0x1000,
                // Jump if less than zero (should fail.)
                0x8500, 0x1000,
                // Jump if less than zero (should pass.)
                0x8520, 0x1000,
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

        // 0x86
        [Test]
        public void JumpToi16IfRegisterGreaterThanEqualRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0500,
                // 1000
                0x1110, 0x1000,
                // -1000
                0x1120, 0xF000,
                // Jump if greater than equal register (should fail.)
                0x8620, 0x1000,
                // Jump if greater than equal register (should pass.)
                0x8600, 0x1000,
            });

            _memory.SetSubroutineAtAddress(0x1000, new ushort[]
            {
                // Jump if greater than equal register (should pass.)
                0x8610, 0x2000,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x0500);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x0500);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
            Assert.IsTrue(_cpu.Registers.A == 0x0500);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0x0500);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

        }

        // 0x87
        [Test]
        public void JumpToRegisterIfRegisterGreaterThanEqualZero()
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
                0x8721,
                // Jump if greater than (should pass.)
                0x8701,

            });

            _memory.SetSubroutineAtAddress(0x1000, new ushort[] 
            { 
                // Jump if greater than (should pass.)
                0x8711,
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

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);
        }

        // 0x88
        [Test]
        public void JumpToi16IfRegisterGreaterThanEqualZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                // 1000
                0x1110, 0x1000,
                // -1000
                0x1120, 0xF000,
                // Jump if greater than equal zero (should fail.)
                0x8820, 0x1000,
                // Jump if greater than equal zero (should pass.)
                0x8800, 0x1000,

            });

            _memory.SetSubroutineAtAddress(0x1000, new ushort[]
            {
                // Jump if greater than equal zero (should pass.)
                0x8810, 0x1000,
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

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);
        }

        // 0x89
        [Test]
        public void JumpToi16IfRegisterLessThanEqualRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0500,
                // 1000
                0x1110, 0x1000,
                // -1000
                0x1120, 0xF000,
                // Jump if less than equal register (should fail.)
                0x8910, 0x1000,
                // Jump if less than equal register (should pass.)
                0x8900, 0x1000,
            });

            _memory.SetSubroutineAtAddress(0x1000, new ushort[]
            {
                // Jump if less than equal register(should pass.)
                0x8920, 0x2000,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x0500);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x0500);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
            Assert.IsTrue(_cpu.Registers.A == 0x0500);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x2000);
            Assert.IsTrue(_cpu.Registers.A == 0x0500);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

        }

        // 0x8A
        [Test]
        public void JumpToRegisterIfRegisterLessThanEqualZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                // 1000
                0x1110, 0x1000,
                // -1000
                0x1120, 0xF000,
                // Jump if less than equal (should fail.)
                0x8A11,
                // Jump if less than equal (should pass.)
                0x8A01,

            });

            _memory.SetSubroutineAtAddress(0x1000, new ushort[]
            { 
                // Jump if less than equal (should pass.)
                0x8A21,
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

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
            Assert.IsTrue(_cpu.Registers.A == 0);
            Assert.IsTrue(_cpu.Registers.B == 0x1000);
            Assert.IsTrue(_cpu.Registers.C == 0xF000);
        }

        // 0x8B
        [Test]
        public void JumpToi16IfRegisterLessThanEqualZero()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x0000,
                // 1000
                0x1110, 0x1000,
                // -1000
                0x1120, 0xF000,
                // Jump if less than equal zero (should fail.)
                0x8B10, 0x1000,
                // Jump if less than equal zero (should pass.)
                0x8B00, 0x1000,

            });

            _memory.SetSubroutineAtAddress(0x1000, new ushort[]
            {
                // Jump if less than equal zero (should pass.)
                0x8B20, 0x1000,
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

            Assert.IsTrue(_cpu.Registers.PC == 0x1000);
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
