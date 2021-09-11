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
            
        }

        [Test]
        public void TestCanPopOffStack()
        {

        }

        [Test]
        public void TestFlagsRegister()
        {
            throw new NotImplementedException("Haven't figured out how to do this yet.");
        }
    }
}