using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
    public partial class CPU
    {
        Memory _memory;
        Registers _registers;

        public CPU(Memory memory)
        {
            _memory = memory;
            _registers = new Registers();

            _registers.Reset();
        }


        public Registers Registers => _registers;

        public void Reset()
        {
            _registers.Reset();
        }

        public void Step()
        {
            var opcode = _memory[_registers.PC];
            _registers.PC++;

            HandleOpcode(opcode);
        }
    }
}
