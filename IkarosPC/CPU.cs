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

        private bool _stopped;

        public Registers Registers => _registers;

        public bool Stopped => _stopped;

        public CPU(Memory memory)
        {
            _memory = memory;
            _registers = new Registers();

            _registers.Reset();

            _stopped = false;
        }


        public void Reset()
        {
            _registers.Reset();
        }

        public void Step()
        {
            if (_stopped)
                return;

            var opcode = _memory[_registers.PC];
            _registers.PC++;

            HandleOpcode(opcode);
        }

        // Instruction helpers
        public void Push(ushort value)
        {
            _memory.Stack[_registers.SP] = value;
            _registers.SP--;

            _registers.StackFrameSize++;
        }

        public ushort Pop()
        {
            _registers.StackFrameSize--;

            _registers.SP++;
            return _memory.Stack[_registers.SP];
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
