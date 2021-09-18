using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
    public partial class CPU
    {
        private readonly Memory _memory;
        private readonly Registers _registers;

        private bool _stopped;

        public bool Stopped => _stopped;

        public CPU(Registers registers, Memory memory)
        {
            _registers = registers;
            _memory = memory;

            _registers.Reset();

            _stopped = false;
        }

        public void Reset()
        {
            _registers.Reset();
            // _memory.Reset();
        }

        public void Step()
        {
            if (_stopped)
                return;

            var opcode = GetImmediate16();

            HandleOpcode(opcode);
        }

        // Instruction helpers
        public ushort GetImmediate16()
        {
            // Switch to memory access mode and temp save the value.
            var saveRSC = _registers.RSC;
            _registers.RSC = 0;

            var immediate = _memory[_registers.PC];
            _registers.PC++;

            // Restore RSC.
            _registers.RSC = saveRSC;

            return immediate;
        }

        public void Push(ushort value)
        {
            // Switch to stack access mode and temp save the value.
            var saveRSC = _registers.RSC;
            _registers.RSC = 1;

            _memory[_registers.SP] = value;
            _registers.SP--;

            _registers.StackFrameSize++;

            // Restore RSC.
            _registers.RSC = saveRSC;
        }

        public ushort Pop()
        {
            // Switch to stack access mode and temp save the value.
            var saveRSC = _registers.RSC;
            _registers.RSC = 1;

            _registers.StackFrameSize--;

            _registers.SP++;
            var stack = _memory[_registers.SP];

            // Restore RSC.
            _registers.RSC = saveRSC;

            return stack;
        }

        public void SaveStackState()
        {
            Push(_registers.A);
            Push(_registers.B);
            Push(_registers.C);
            Push(_registers.D);
            Push(_registers.E);
            Push(_registers.X);
            Push(_registers.Y);
            Push(_registers.Z);
            
            Push(_registers.Flags);
            Push(_registers.PC);
            Push((ushort)(_registers.StackFrameSize + 1));

            _registers.FP = _registers.SP;
            _registers.StackFrameSize = 0;
        }

        public void LoadStackState()
        {
            _registers.SP = _registers.FP;

            _registers.StackFrameSize = Pop();
            var previousStackFrameSize = _registers.StackFrameSize;

            _registers.PC = Pop();
            _registers.Flags = Pop();

            _registers.Z = Pop();
            _registers.Y = Pop();
            _registers.X = Pop();
            _registers.E = Pop();
            _registers.D = Pop();
            _registers.C = Pop();
            _registers.B = Pop();
            _registers.A = Pop();

            var numberOfArguments = Pop();
            for (int i = 0; i < numberOfArguments; i++)
            {
                Pop();
            }

            _registers.FP += previousStackFrameSize;
        }
    }
}
