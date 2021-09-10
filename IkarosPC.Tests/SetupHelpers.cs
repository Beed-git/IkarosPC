using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests
{
    static class SetupHelpers
    {
        public static void CheckInitialCPUState(CPU cpu)
        {
            // Special registers.
            Assert.IsTrue(cpu.Registers.PC == 0);
            Assert.IsTrue(cpu.Registers.Accumulator == 0);
            Assert.IsTrue(cpu.Registers.Flags == 0);

            // Flags
            Assert.IsFalse(cpu.Registers.Zero);
            Assert.IsFalse(cpu.Registers.Carry);
            Assert.IsFalse(cpu.Registers.Negative);

            // General Registers
            Assert.IsTrue(cpu.Registers.A == 0);
            Assert.IsTrue(cpu.Registers.B == 0);
            Assert.IsTrue(cpu.Registers.C == 0);
            Assert.IsTrue(cpu.Registers.D == 0);
            Assert.IsTrue(cpu.Registers.E == 0);
            Assert.IsTrue(cpu.Registers.X == 0);
            Assert.IsTrue(cpu.Registers.Y == 0);
            Assert.IsTrue(cpu.Registers.Z == 0);
        }
    }
}
