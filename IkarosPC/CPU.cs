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

            Execute(opcode);
        }

        protected void Execute(ushort opcode)
        {
            byte nibble = (byte)((opcode & 0xFF00) >> 8);

            switch (nibble)
            {
                case var n when (n >= 0x00 && n < 0x01):
                    HandleControlFunctions(opcode); 
                    break;
                case var n when (n >= 0x10 && n < 0x15):
                    HandleMoveFunctions(opcode); 
                    break;
                case var n when (n >= 0x20 && n < 0x30):
                    HandleMathFunctions(opcode); 
                    break;
                case var n when (n >= 0x30 && n < 0x40):
                    HandleJumpFunctions(opcode);
                    break;

                default: throw new NotImplementedException($"Opcode: { opcode } not implemented or does not exist.");
            }

        }
    }
}
