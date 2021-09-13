using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.InstructionTests
{
    class MoveInstructionsTests
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
        // 0x13
        [Test]
        public void TestMoveLiteralToRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Store 0x1234 in Register A
                0x1300, 0x1234,
                // Store 0x4321 in Register Z
                0x1370, 0x4321,
                // Store 0xFEDC in Register B
                // 0x000F at end which shouldn't affect the execution.
                0x131F, 0xFEDC,
                // Store 0x5432 in Register A
                0x1300, 0x5432
            });

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 2);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Z == 0);
            Assert.IsTrue(_cpu.Registers.B == 0);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Z == 0x4321);
            Assert.IsTrue(_cpu.Registers.B == 0);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.Z == 0x4321);
            Assert.IsTrue(_cpu.Registers.B == 0xFEDC);

            _cpu.Step();
            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x5432);
            Assert.IsTrue(_cpu.Registers.Z == 0x4321);
            Assert.IsTrue(_cpu.Registers.B == 0xFEDC);
            
        }

        // 0x14
        [Test]
        public void TestMoveLiteralToMemoryAddress()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Store 0x0010 into Register A, 0x1234 into Register B.
                0x1300, 0x0010,
                0x1310, 0x1234,
                // Store 0xF1F2 into memory specified by Register A
                0x1400, 0xF1F2,
                // Store 0x1515 into memory specified by Register B
                0x1419, 0x1515,
            });

            Assert.IsTrue(_memory[0x0010] == 0);
            Assert.IsTrue(_memory[0x1234] == 0);

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x0010);
            Assert.IsTrue(_cpu.Registers.B == 0x1234);
            Assert.IsTrue(_memory[0x0010] == 0);
            Assert.IsTrue(_memory[0x1234] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x0010);
            Assert.IsTrue(_cpu.Registers.B == 0x1234);
            Assert.IsTrue(_memory[0x0010] == 0xF1F2);
            Assert.IsTrue(_memory[0x1234] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x0010);
            Assert.IsTrue(_cpu.Registers.B == 0x1234);
            Assert.IsTrue(_memory[0x0010] == 0xF1F2);
            Assert.IsTrue(_memory[0x1234] == 0x1515);
        }

        [Test]
        public void MoveLiteralToLiteral()
        {
            _memory.SetInitialMemory(new ushort[] {
                // Store 0x1234 at address 0x2000.
                0x1500, 0x1234, 0x2000,
                // Switch to vram
                0x1500, 0x0002, 0xFFFF,
                // Store in vram
                0x1500, 0x4321, 0x1000,
            });

            // Manually set vram at 0x1000 to zero, since it could aready contain a value. 
            _memory.Vram[0x1000] = 0;

            Assert.IsTrue(_memory.Ram[0x2000] == 0);
            Assert.IsTrue(_memory.Vram[0x1000] == 0);
            Assert.IsTrue(_memory[0xFFFF] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 3);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x1234);
            Assert.IsTrue(_memory.Vram[0x1000] == 0);
            Assert.IsTrue(_memory[0xFFFF] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x1234);
            Assert.IsTrue(_memory.Vram[0x1000] == 0);
            Assert.IsTrue(_memory[0xFFFF] == 0x0002);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_memory.Ram[0x2000] == 0x1234);
            Assert.IsTrue(_memory.Vram[0x1000] == 0x4321);
            Assert.IsTrue(_memory[0xFFFF] == 0x0002);
        }

        // 0x10
        [Test]
        public void MoveFromRegisterToRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Set Register A to 0x1234, Register B to 0x5432.
                0x1300, 0x1234,
                0x1310, 0x5432,
                // Move from Register A to Register C
                0x1002,
                // Move from Register B to register A
                0x1010,
                // Move from Register C to register B
                0x1021
            });

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x5432);
            Assert.IsTrue(_cpu.Registers.C == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x5432);
            Assert.IsTrue(_cpu.Registers.C == 0x1234);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x5432);
            Assert.IsTrue(_cpu.Registers.B == 0x5432);
            Assert.IsTrue(_cpu.Registers.C == 0x1234);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.A == 0x5432);
            Assert.IsTrue(_cpu.Registers.B == 0x1234);
            Assert.IsTrue(_cpu.Registers.C == 0x1234);
        }

        // 0x11
        [Test]
        public void MoveFromMemoryToRegister()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers
                0x1300, 0x0007,
                0x1310, 0x0008,
                // Move from memory[7] to C
                0x1102,
                // Move from memory[8] to D
                0x1113,
                // Move from memory[7] to A
                0x1100,
                // Data
                0x1234, 0x5432
            });

            Assert.IsTrue(_memory[0x0007] == 0x1234);
            Assert.IsTrue(_memory[0x0008] == 0x5432);

            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 4);
            Assert.IsTrue(_cpu.Registers.A == 0x0007);
            Assert.IsTrue(_cpu.Registers.B == 0x0008);
            Assert.IsTrue(_cpu.Registers.C == 0);
            Assert.IsTrue(_cpu.Registers.D == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 5);
            Assert.IsTrue(_cpu.Registers.A == 0x0007);
            Assert.IsTrue(_cpu.Registers.B == 0x0008);
            Assert.IsTrue(_cpu.Registers.C == 0x1234);
            Assert.IsTrue(_cpu.Registers.D == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 6);
            Assert.IsTrue(_cpu.Registers.A == 0x0007);
            Assert.IsTrue(_cpu.Registers.B == 0x0008);
            Assert.IsTrue(_cpu.Registers.C == 0x1234);
            Assert.IsTrue(_cpu.Registers.D == 0x5432);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 7);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x0008);
            Assert.IsTrue(_cpu.Registers.C == 0x1234);
            Assert.IsTrue(_cpu.Registers.D == 0x5432);
        }

        // 0x12
        [Test]
        public void MoveFromRegisterToMemory()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Init registers.
                0x1300, 0x1234,
                0x1310, 0x5432,
                0x1320, 0x0010,
                0x1330, 0x0011,
                // Move from Register A to memory specified by Register C.
                0x1202,
                // Move from Register B to memory specified by Register D.
                0x1213,
                // Move from Register C to memory specified by Register D.
                0x1223
            });

            Assert.IsTrue(_memory[0x0010] == 0);
            Assert.IsTrue(_memory[0x0011] == 0);

            _cpu.Step();
            _cpu.Step();
            _cpu.Step();
            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 8);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x5432);
            Assert.IsTrue(_cpu.Registers.C == 0x0010);
            Assert.IsTrue(_cpu.Registers.D == 0x0011);
            Assert.IsTrue(_memory[0x0010] == 0);
            Assert.IsTrue(_memory[0x0011] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 9);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x5432);
            Assert.IsTrue(_cpu.Registers.C == 0x0010);
            Assert.IsTrue(_cpu.Registers.D == 0x0011);
            Assert.IsTrue(_memory[0x0010] == 0x1234);
            Assert.IsTrue(_memory[0x0011] == 0);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 10);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x5432);
            Assert.IsTrue(_cpu.Registers.C == 0x0010);
            Assert.IsTrue(_cpu.Registers.D == 0x0011);
            Assert.IsTrue(_memory[0x0010] == 0x1234);
            Assert.IsTrue(_memory[0x0011] == 0x5432);

            _cpu.Step();

            Assert.IsTrue(_cpu.Registers.PC == 11);
            Assert.IsTrue(_cpu.Registers.A == 0x1234);
            Assert.IsTrue(_cpu.Registers.B == 0x5432);
            Assert.IsTrue(_cpu.Registers.C == 0x0010);
            Assert.IsTrue(_cpu.Registers.D == 0x0011);
            Assert.IsTrue(_memory[0x0010] == 0x1234);
            Assert.IsTrue(_memory[0x0011] == 0x0010);
        }
    }
}
