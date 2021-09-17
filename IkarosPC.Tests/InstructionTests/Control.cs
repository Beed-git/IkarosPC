using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class Control
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
        public void GeneralToSpecial()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1100, 0x2000,
                0x1110, 0x4000,
                0x1120, 0x8000,
                0x1130, 0xF000,
                // Move into SP.
                0x0801,
                // Move into FP.
                0x0812,
                // Move into MBC.
                0x0823,
                // Move into PC.
                0x0830,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.SP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.FP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.MBC == 0);

            Assert.IsTrue(_cpu.Registers.A == 0x2000);
            Assert.IsTrue(_cpu.Registers.B == 0x4000);
            Assert.IsTrue(_cpu.Registers.C == 0x8000);
            Assert.IsTrue(_cpu.Registers.D == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.SP == 0x2000);
            Assert.IsTrue(_cpu.Registers.FP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.MBC == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.SP == 0x2000);
            Assert.IsTrue(_cpu.Registers.FP == 0x4000);
            Assert.IsTrue(_cpu.Registers.MBC == 0);

            Assert.IsTrue(_cpu.Registers.A == 0x2000);
            Assert.IsTrue(_cpu.Registers.B == 0x4000);
            Assert.IsTrue(_cpu.Registers.C == 0x8000);
            Assert.IsTrue(_cpu.Registers.D == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.SP == 0x2000);
            Assert.IsTrue(_cpu.Registers.FP == 0x4000);
            Assert.IsTrue(_cpu.Registers.MBC == 0x8000);

            Assert.IsTrue(_cpu.Registers.A == 0x2000);
            Assert.IsTrue(_cpu.Registers.B == 0x4000);
            Assert.IsTrue(_cpu.Registers.C == 0x8000);
            Assert.IsTrue(_cpu.Registers.D == 0xF000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 0xF000);
            Assert.IsTrue(_cpu.Registers.SP == 0x2000);
            Assert.IsTrue(_cpu.Registers.FP == 0x4000);
            Assert.IsTrue(_cpu.Registers.MBC == 0x8000);

            Assert.IsTrue(_cpu.Registers.A == 0x2000);
            Assert.IsTrue(_cpu.Registers.B == 0x4000);
            Assert.IsTrue(_cpu.Registers.C == 0x8000);
            Assert.IsTrue(_cpu.Registers.D == 0xF000);
            
        }
        
        [Test]
        public void SpecialToGeneral()
        {
            _memory.SetInitialMemory(new ushort[]
{
                // Init registers.
                0x1100, 0x0000,
                0x1110, 0x0000,
                0x1120, 0x0000,
                0x1130, 0x0000,
                0x1140, 0x1000,
                // Set MBC.
                0x0843,
                // Move PC to register.
                0x0901,
                // Move SP to register.
                0x0912,
                // Move FP to register.
                0x0923,
                // Move MBC to register.
                0x0930,
            });

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.SP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.FP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.MBC == 0x1000);

            Assert.IsTrue(_cpu.Registers.A == 0x0000);
            Assert.IsTrue(_cpu.Registers.B == 0x0000);
            Assert.IsTrue(_cpu.Registers.C == 0x0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);
            Assert.IsTrue(_cpu.Registers.E == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 12);
            Assert.IsTrue(_cpu.Registers.SP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.FP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.MBC == 0x1000);

            Assert.IsTrue(_cpu.Registers.A == 0x0000);
            Assert.IsTrue(_cpu.Registers.B == 12);
            Assert.IsTrue(_cpu.Registers.C == 0x0000);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);
            Assert.IsTrue(_cpu.Registers.E == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 13);
            Assert.IsTrue(_cpu.Registers.SP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.FP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.MBC == 0x1000);

            Assert.IsTrue(_cpu.Registers.A == 0x0000);
            Assert.IsTrue(_cpu.Registers.B == 12);
            Assert.IsTrue(_cpu.Registers.C == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.D == 0x0000);
            Assert.IsTrue(_cpu.Registers.E == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 14);
            Assert.IsTrue(_cpu.Registers.SP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.FP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.MBC == 0x1000);

            Assert.IsTrue(_cpu.Registers.A == 0x0000);
            Assert.IsTrue(_cpu.Registers.B == 12);
            Assert.IsTrue(_cpu.Registers.C == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.D == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.E == 0x1000);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 15);
            Assert.IsTrue(_cpu.Registers.SP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.FP == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.MBC == 0x1000);

            Assert.IsTrue(_cpu.Registers.A == 0x1000);
            Assert.IsTrue(_cpu.Registers.B == 12);
            Assert.IsTrue(_cpu.Registers.C == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.D == 0xBFFF);
            Assert.IsTrue(_cpu.Registers.E == 0x1000);
        }
    }
}
